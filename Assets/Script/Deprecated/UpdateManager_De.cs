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
 */

using System.Collections.Generic;
using UnityEngine;

public class UpdateManager_De: MonoBehaviour
{
    private IList<int> listKeys;
    private myUpdate temp;

    private IList<int> objectListKeys;
    private SortedList<int, myUpdate> componentList;

    private void Initialize(SortedList<int, myUpdate> theList)
    {
        listKeys = theList.Keys;
        for (int i = 0; i < listKeys.Count; i++)
        {
            if (PlayerUpdates.TryGetValue(listKeys[i], out temp))
            {
                temp.Initialize();
            }
        }
    }
    private void Initialize(Dictionary<int, SortedList<int, myUpdate>> theObjectList)
    {
        foreach (KeyValuePair<int, SortedList<int, myUpdate>> enemyUpdateList in EnemyUpdates)
        {
            Initialize(enemyUpdateList.Value);
        }
    }
    private void Start()
    {
        foreach (KeyValuePair<int, myUpdate> a in GUIUpdates)
        {
            a.Value.Initialize();
        }
        foreach (KeyValuePair<int, myUpdate> a in MapUpdates)
        {
            a.Value.Initialize();
        }
        foreach (KeyValuePair<int, SortedList<int, myUpdate>> poolThingList in PoolThingUpdates)
        {
            foreach (KeyValuePair<int, myUpdate> poolThing in poolThingList.Value)
            {
                poolThing.Value.Initialize();
            }
        }
        //for(int i = 0; i < PlayerUpdates.Count; i ++)
        //{
        //    PlayerUpdates.
        //}



        //foreach (KeyValuePair<int, myUpdate> a in PlayerUpdates)
        //{
        //    a.Value.Initialize();
        //}
        foreach (KeyValuePair<int, SortedList<int, myUpdate>> enemyUpdateList in EnemyUpdates)
        {
            foreach (KeyValuePair<int, myUpdate> enemyComponent in enemyUpdateList.Value)
            {
                enemyComponent.Value.Initialize();
            }
        }
        print();
    }

    private void Update()
    {
        foreach (KeyValuePair<int, myUpdate> a in GUIUpdates)
        {
            a.Value.MyUpdate();
        }
        foreach (KeyValuePair<int, myUpdate> a in MapUpdates)
        {
            a.Value.MyUpdate();
        }
        foreach (KeyValuePair<int, SortedList<int, myUpdate>> poolThingList in PoolThingUpdates)
        {
            foreach (KeyValuePair<int, myUpdate> poolThing in poolThingList.Value)
            {
                poolThing.Value.MyUpdate();
            }
        }
        foreach (KeyValuePair<int, myUpdate> a in PlayerUpdates)
        {
            a.Value.MyUpdate();
        }
        foreach (KeyValuePair<int, SortedList<int, myUpdate>> enemyUpdateList in EnemyUpdates)
        {
            foreach(KeyValuePair<int, myUpdate> enemyComponent in enemyUpdateList.Value)
            {
                enemyComponent.Value.MyUpdate();
            }
        }
    }

    public bool Register(myUpdate update)
    {
        switch(update.GetUpdateType())
        {
            case myUpdate.UpdateType.GUI:
                GUIUpdates.Add(update.GetPriorityInType(), update);
                return true;
            case myUpdate.UpdateType.Map:
                MapUpdates.Add(update.GetPriorityInType(), update);
                return true;
            case myUpdate.UpdateType.PoolThing:
                //储存引用
                SortedList<int, myUpdate> temp;
                //看看有没有这个id
                if (PoolThingUpdates.TryGetValue(update.gameObject.GetInstanceID(), out temp))
                {
                    //如果有，则将这个组件加入到对应敌人的更新队列中去
                    temp.Add(update.GetPriorityInType(), update);
                }
                else
                {
                    //如果没有，增加这个敌人的队列，并且把这个组件加入进去
                    temp = new SortedList<int, myUpdate>();
                    temp.Add(update.GetPriorityInType(), update);
                    PoolThingUpdates.Add(update.gameObject.GetInstanceID(), temp);
                }

                return true;
            case myUpdate.UpdateType.Player:
                PlayerUpdates.Add(update.GetPriorityInType(), update);
                return true;
            case myUpdate.UpdateType.Enemy:
                //储存引用
                SortedList<int, myUpdate> temp1;
                //看看有没有这个id
                if(EnemyUpdates.TryGetValue(update.gameObject.GetInstanceID(), out temp1))
                {
                    //如果有，则将这个组件加入到对应敌人的更新队列中去
                    temp1.Add(update.GetPriorityInType(), update);
                }
                else
                {
                    //如果没有，增加这个敌人的队列，并且把这个组件加入进去
                    temp1 = new SortedList<int, myUpdate>();
                    temp1.Add(update.GetPriorityInType(), update);
                    EnemyUpdates.Add(update.gameObject.GetInstanceID(), temp1);
                }

                return true;
        }
        return false;
    }

    public const int MAX_ENEMY_NUMBER = 128;
    private SortedList<int, myUpdate> GUIUpdates = new SortedList<int, myUpdate>();
    private SortedList<int, myUpdate> MapUpdates = new SortedList<int, myUpdate>();
    private Dictionary<int, SortedList<int, myUpdate>> PoolThingUpdates = new Dictionary<int, SortedList<int, myUpdate>>();
    private SortedList<int, myUpdate> PlayerUpdates = new SortedList<int, myUpdate>();
    private Dictionary<int, SortedList<int, myUpdate>> EnemyUpdates = new Dictionary<int, SortedList<int, myUpdate>>();


    public void print()
    {
        foreach (KeyValuePair<int, myUpdate> a in GUIUpdates)
        {
            Debug.Log("在updatemanager的GUI队列里，存在" + a.Value.name);
        }
        foreach (KeyValuePair<int, myUpdate> a in MapUpdates)
        {
            Debug.Log("在updatemanager的Map队列里，存在" + a.Value.name);
        }
        foreach (KeyValuePair<int, SortedList<int, myUpdate>> poolThingList in PoolThingUpdates)
        {
            var i = poolThingList.Value.GetEnumerator();
            if (i.MoveNext())
            {
                //string enemyName = i.Current.Value.gameObject.name;
                Debug.Log("在updatemanager的PoolThing队列里，存在" + i.Current.Value.gameObject.name);
            }
        }
        foreach (KeyValuePair<int, myUpdate> a in PlayerUpdates)
        {
            Debug.Log("在updatemanager的Player队列里，存在" + a.Value.name);
        }
        foreach (KeyValuePair<int, SortedList<int, myUpdate>> enemyUpdateList in EnemyUpdates)
        {
            var i = enemyUpdateList.Value.GetEnumerator();
            if(i.MoveNext())
            {
                //string enemyName = i.Current.Value.gameObject.name;
                Debug.Log("在updatemanager的Enemy队列里，存在" + i.Current.Value.gameObject.name + "这些敌人");
            }
        }
    }

    public bool LogOut(myUpdate logout)
    {
        switch (logout.GetUpdateType())
        {
            case myUpdate.UpdateType.GUI:
                return GUIUpdates.Remove(logout.GetPriorityInType());
            case myUpdate.UpdateType.Map:
                return MapUpdates.Remove(logout.GetPriorityInType());
            case myUpdate.UpdateType.PoolThing:
                //储存引用
                SortedList<int, myUpdate> temp;
                //看看有没有这个id
                if (PoolThingUpdates.TryGetValue(logout.gameObject.GetInstanceID(), out temp))
                {
                    //如果有，则将这个组件从对应敌人的更新队列中去除
                    bool result1 = temp.Remove(logout.GetPriorityInType());
                    bool result2 = true;
                    if(temp.Count == 0)
                    {
                        result2 = PoolThingUpdates.Remove(logout.gameObject.GetInstanceID());
                    }
                    return result1 && result2;
                }
                else
                {
                    //没有id直接false
                    return false;
                }
            case myUpdate.UpdateType.Player:
                return PlayerUpdates.Remove(logout.GetPriorityInType());
            case myUpdate.UpdateType.Enemy:
                //储存引用
                SortedList<int, myUpdate> temp1;
                //看看有没有这个id
                if (EnemyUpdates.TryGetValue(logout.gameObject.GetInstanceID(), out temp1))
                {
                    //如果有，则将这个组件从对应敌人的更新队列中去除
                    bool result1 = temp1.Remove(logout.GetPriorityInType());
                    bool result2 = true;
                    if (temp1.Count == 0)
                    {
                        result2 = EnemyUpdates.Remove(logout.gameObject.GetInstanceID());
                    }
                    return result1 && result2;
                }
                else
                {
                    //没有id直接false
                    return false;
                }
        }
        return false;
    }

    //使用二维数组实现敌人注册，由于涉及注销导致的数组移动问题，换用以链表为基础的字典实现
    //private int[] EnemyIds = new int[MAX_ENEMY_NUMBER];
    //private myUpdate[][] EnemyUpdates = new myUpdate[MAX_ENEMY_NUMBER][];
    //private int[] enemyUpdatesPointer = new int[MAX_ENEMY_NUMBER];
    //private int enemyPointer = 0;

    //private bool IsEnemyRegistered(int enemyId)
    //{
    //    for(int i = 0; i < enemyPointer; i ++)
    //    {
    //        if(EnemyIds[i] == enemyId)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    //private bool AddEnemy()
    //{

    //}
}
