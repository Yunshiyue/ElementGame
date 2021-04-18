/**
 * @Description: 继承自MonoBehaviour，控制update顺序和优化执行效率的组件类。
 *               那些应该执行update的组件应该继承自这个类。updateManager类中规定了若干个优先级队列，分别对应不同的update元素，
 *               比如地图元素、player、怪物等。在每个队列中，按照自身的PriorityInType升序调用update。
 *               在awake方法中，会将自己注册进updatemanager中。
 * @Author: ridger

 * 

 * @Editor: ridger
 * @Edit: 增加了注销方法，以供myUpdate方法从update队列中移除；
 * 

 * @Editor: ridger
 * @Edit: 增加了UpdateType.PoolThing的枚举，从对象池中拿出来的物体type应该设为这个值
 * 

 * @Editor: ridger
 * @Edit: 修复了无法注销的bug，myUpdate类在注册时就会调用Initialize函数，而不是在Start中调用。
 *        现在取消注册的实现方式改为将一个叫做isActive的bool值设为false，在每次调用update时会检查isActive，
 *        如果为true则调用，否则不执行myUpdate。重新调用Register将此值置为true。
 *        

 * @Editor: ridger
 * @Edit: UpdateManager重写，该类的接口也随之微调：
 *          1.注册和注销移动到MonoBehavior的OnEnable和OnDisable中，无需手动调用，故不在设置此public方法
 *          2.取消了isActive成员变量，由于现在可以从根本上实现动态注销和注册，无需设置改变量以表示是否关闭
 *          3.UpdateManager新的数据结构要求MyUpdate类的Priority为正，尽可能的小，
 *            同时同一物体/层中的不同Priority尽量紧密排列
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class myUpdate : MonoBehaviour
{
    public enum UpdateType { GUI, Map, PoolThing, Player, Enemy }

    public bool hasInitialize = false;

    private UpdateManager manager;

    protected virtual void OnEnable()
    {
        manager = GameObject.Find("UpdateManager").GetComponent<UpdateManager>();
        manager.Register(this);
    }

    protected virtual void OnDisable()
    {
        manager.LogOut(this);
    }

    abstract public void Initialize();

    abstract public void MyUpdate();

    abstract public UpdateType GetUpdateType();

    abstract public int GetPriorityInType();

}
