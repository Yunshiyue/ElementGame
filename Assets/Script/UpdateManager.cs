/**
 * @Description: 该类负责管理继承自myUpdate的有更新需求组件类，通过注册到该唯一类，updateManager根据myUpdate中的
 *               UpdateType将不同类型的myUpdate放入不同的优先级队列中，并根据PriorityInType升序更新。
 * @Author: ridger

 * 

 * @Editor: ridger
 * @Edit: 增加了注销方法，以供myUpdate方法从update队列中移除；请调用myUpdate中的LogOut方法来实现注销，而不是直接调用
 *        这里的LogOut
 *        

 * @Editor: ridger
 * @Edit: 修正了敌人组件的队列逻辑，避免了不同敌人的同一个组件类的Priority相同的情况：
 *        现在不同的敌人组件会根据gameObject.GetInstanceId进入不同的排序队列，每个id对应一个排序队列。
 *        这些队列储存在以gameObject.GetInstanceId为Key，以sortedList为value的字典中
 *        

 * @Editor: ridger
 * @Edit: 增加了PoolThing队列，专门用于更新类似飞行道具等从对象池中拿出来的物体，具体逻辑与敌人相同。
 *        更新顺序介于Map和Player之间，目的是在敌人和主角扔出这些道具时，能在下一帧直接对目标造成效果，而不是更多帧以后。
 *        

 * @Editor: ridger
 * @Edit: 修复了无法注销的bug，myUpdate类在注册时就会调用Initialize函数，而不是在Start中调用。
 *        现在取消注册的实现方式改为将一个叫做isActive的bool值设为false，在每次调用update时会检查isActive，
 *        如果为true则调用，否则不执行myUpdate。重新调用Register将此值置为true。
 *        

 * @Editor: ridger
 * @Edit: 1.重写了此类的内部逻辑，接口不变
 *        2.采用独立的数据结构替换了原先的SortedList，从根本上解决了update中不能动态增删物体的问题，同时update效率
 *          也有提高
 *        3.现在使用update数组+valid数组+maxIndex来组成一个更新队列，如果有myUpdate进行注册，则将udpate数组对应priority
 *          位置赋值为myUpdate，同时valid数组的相同位置设为true，进行更新。注销则相反。
 *        4.使用List+3中的更新队列来组成物体+物体下的组件的逻辑
 *        

 * @Editor: ridger
 * @Edit: 改写了Initialize的逻辑，使用硬补丁，使得Initialize只被调用一次
 */

using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    GameObjectUpdate temp;
    private void Update()
    {
        for(int i = 0; i <= GUIMaxIndex; i++)
        {
            if(GUIValid[i])
            {
                GUIUpdate[i].MyUpdate();
            }
        }
        for (int i = 0; i <= MapMaxIndex; i++)
        {
            if (MapValid[i])
            {
                MapUpdate[i].MyUpdate();
            }
        }
        for(int i = 0; i < PoolThingUpdate.Count; i++)
        {
            temp = PoolThingUpdate[i];
            for (int j = 0; j <= temp.maxArrayIndex; j++)
            {
                if(temp.isValid[j])
                {
                    temp.components[j].MyUpdate();
                }
            }
        }
        for (int i = 0; i <= PlayerMaxIndex; i++)
        {
            if (PlayerValid[i])
            {
                PlayerUpdate[i].MyUpdate();
            }
        }
        for (int i = 0; i < EnemyUpdate.Count; i++)
        {
            temp = EnemyUpdate[i];
            for (int j = 0; j <= temp.maxArrayIndex; j++)
            {
                if (temp.isValid[j])
                {
                    temp.components[j].MyUpdate();
                }
            }
        }
    }



    public const int MAX_UPDATE_NUMBER = 32;

    //GUI、Map、PlayerUpdate:只需要注册一次，之后一般不进行变动，但是需要频繁调度，考虑使用数组+最大Priority进行组织
    //Enemy、PoolThing：涉及反复添加、注销，而且是基于物体的两级update，使用链表+数组进行组织

    private myUpdate[] GUIUpdate = new myUpdate[MAX_UPDATE_NUMBER];
    private bool[] GUIValid = new bool[MAX_UPDATE_NUMBER];
    private int GUIMaxIndex = -1;
    private myUpdate[] MapUpdate = new myUpdate[MAX_UPDATE_NUMBER];
    private bool[] MapValid = new bool[MAX_UPDATE_NUMBER];
    private int MapMaxIndex = -1;
    private List<GameObjectUpdate> PoolThingUpdate = new List<GameObjectUpdate>(MAX_UPDATE_NUMBER);
    private myUpdate[] PlayerUpdate = new myUpdate[MAX_UPDATE_NUMBER];
    private bool[] PlayerValid = new bool[MAX_UPDATE_NUMBER];
    private int PlayerMaxIndex = -1;
    private List<GameObjectUpdate> EnemyUpdate = new List<GameObjectUpdate>(MAX_UPDATE_NUMBER);

    //向给定数组中注册
    private bool RegisterIntoArray(myUpdate update, myUpdate[] updateList, bool[] isValidList, in int maxIndex, out int newMaxIndex)
    {
        int updatePriority = update.GetPriorityInType();

        if (updatePriority >= MAX_UPDATE_NUMBER)
        {
            newMaxIndex = maxIndex;
            Debug.LogError(update.gameObject.name + "注册失败，优先级太大超过了最大值" + MAX_UPDATE_NUMBER);
            return false;
        }

        if(isValidList[updatePriority])
        {
            Debug.LogError(update.gameObject.name + "注册失败，优先级重复" + updatePriority);
            newMaxIndex = maxIndex;
            return false;
        }
        else
        {
            isValidList[updatePriority] = true;
            updateList[updatePriority] = update;
            newMaxIndex = maxIndex > updatePriority ? maxIndex : updatePriority;
            return true;
        }
    }

    private bool RegisterIntoList(myUpdate update, List<GameObjectUpdate> theList)
    {
        GameObjectUpdate objectUpdate = null;
        for(int i = 0; i < theList.Count; i++)
        {
            if(theList[i].GameObjectID == update.GetInstanceID()) {
                objectUpdate = theList[i];
                return RegisterIntoArray(update, objectUpdate.components, objectUpdate.isValid,
                                            objectUpdate.maxArrayIndex, out objectUpdate.maxArrayIndex);
            }
        }
        objectUpdate = new GameObjectUpdate(update.GetInstanceID());
        theList.Add(objectUpdate);
        return RegisterIntoArray(update, objectUpdate.components, objectUpdate.isValid,
                                            objectUpdate.maxArrayIndex, out objectUpdate.maxArrayIndex);
    }

    public bool Register(myUpdate update)
    {
        if(!update.hasInitialize)
        {
            update.Initialize();
            update.hasInitialize = true;
        }

        switch (update.GetUpdateType())
        {
            case myUpdate.UpdateType.GUI:
                return RegisterIntoArray(update, GUIUpdate, GUIValid, GUIMaxIndex, out GUIMaxIndex);
            case myUpdate.UpdateType.Map:
                return RegisterIntoArray(update, MapUpdate, MapValid, MapMaxIndex, out MapMaxIndex);
            case myUpdate.UpdateType.PoolThing:
                return RegisterIntoList(update, PoolThingUpdate);
            case myUpdate.UpdateType.Player:
                return RegisterIntoArray(update, PlayerUpdate, PlayerValid, PlayerMaxIndex, out PlayerMaxIndex);
            case myUpdate.UpdateType.Enemy:
                return RegisterIntoList(update, EnemyUpdate);
        }
        return false;
    }

    private bool LogOutFromArray(myUpdate update, myUpdate[] updateList, bool[] isValidList, in int maxIndex, out int newMaxIndex)
    {
        int updatePriority = update.GetPriorityInType();
        if(!isValidList[updatePriority])
        {
            newMaxIndex = maxIndex;
            Debug.LogError(update.gameObject.name + "注销失败，没有注册过该优先级" + updatePriority);
            return false;
        }
        else
        {
            isValidList[updatePriority] = false;
            updateList[updatePriority] = null;
            newMaxIndex = maxIndex;
            if(updatePriority == maxIndex)
            {
                int i = maxIndex - 1;
                for (; i >= 0; i--)
                {
                    if(isValidList[i])
                    {
                        break;
                    }
                }
                newMaxIndex = i;
            }
            return true;
        }
    }
    private bool LogOutFromList(myUpdate update, List<GameObjectUpdate> theList)
    {
        GameObjectUpdate objectUpdate = null;
        for(int i = 0; i < theList.Count; i++)
        {
            if(theList[i].GameObjectID == update.GetInstanceID())
            {
                objectUpdate = theList[i];
                if(LogOutFromArray(update, objectUpdate.components, objectUpdate.isValid,
                                        objectUpdate.maxArrayIndex, out objectUpdate.maxArrayIndex))
                {
                    if(objectUpdate.maxArrayIndex == -1)
                    {
                        theList.Remove(objectUpdate);
                    }
                    return true;
                }
                else
                {
                    Debug.LogError(update.gameObject.name + "注销失败，没有注册过该优先级" + update.GetPriorityInType());
                    return false;
                }
            }
        }
        Debug.LogError(update.gameObject.name + "注销失败，没有注册过物体" + update.GetInstanceID());
        //如果没有这个ID，返回false
        return false;
    }

    public bool LogOut(myUpdate update)
    {
        switch (update.GetUpdateType())
        {
            case myUpdate.UpdateType.GUI:
                return LogOutFromArray(update, GUIUpdate, GUIValid, GUIMaxIndex, out GUIMaxIndex);
            case myUpdate.UpdateType.Map:
                return LogOutFromArray(update, MapUpdate, MapValid, MapMaxIndex, out MapMaxIndex);
            case myUpdate.UpdateType.PoolThing:
                return LogOutFromList(update, PoolThingUpdate);
            case myUpdate.UpdateType.Player:
                return LogOutFromArray(update, PlayerUpdate, PlayerValid, PlayerMaxIndex, out PlayerMaxIndex);
            case myUpdate.UpdateType.Enemy:
                return LogOutFromList(update, EnemyUpdate);
        }
        return false;
    }


    private class GameObjectUpdate
    {
        public static int MAX_COMPONENT_NUMBER = 32;
        public int GameObjectID;
        public myUpdate[] components = new myUpdate[MAX_COMPONENT_NUMBER];
        public bool[] isValid = new bool[MAX_COMPONENT_NUMBER];
        public int maxArrayIndex = -1;

        public GameObjectUpdate()
        {
            GameObjectID = -1;
        }
        public GameObjectUpdate(int GameObjectID)
        {
            this.GameObjectID = GameObjectID;
        }
    }


    public void print()
    {
        for (int i = 0; i <= GUIMaxIndex; i++)
        {
            if (GUIValid[i])
            {
                Debug.Log("在GUI列表里，存在" + GUIUpdate[i].gameObject.name);
            }
        }
        for (int i = 0; i <= MapMaxIndex; i++)
        {
            if (MapValid[i])
            {
                Debug.Log("在Map列表里，存在" + MapUpdate[i].gameObject.name);
            }
        }
        for (int i = 0; i < PoolThingUpdate.Count; i++)
        {
            temp = PoolThingUpdate[i];
            for (int j = 0; j <= temp.maxArrayIndex; j++)
            {
                if (temp.isValid[j])
                {
                    Debug.Log("在PoolThing列表里，存在" + temp.components[i].gameObject.name);
                }
            }
        }
        for (int i = 0; i <= PlayerMaxIndex; i++)
        {
            if (PlayerValid[i])
            {
                Debug.Log("在Player列表里，存在" + PlayerUpdate[i].gameObject.name);
            }
        }
        for (int i = 0; i < EnemyUpdate.Count; i++)
        {
            temp = EnemyUpdate[i];
            for (int j = 0; j <= temp.maxArrayIndex; j++)
            {
                if (temp.isValid[j])
                {
                    Debug.Log("在Enemy列表里，存在" + temp.components[i].gameObject.name);
                }
            }
        }
    }
}
