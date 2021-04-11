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
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class myUpdate : MonoBehaviour
{
    public enum UpdateType { GUI, Map, PoolThing, Player, Enemy }

    private UpdateManager manager;

    private void Awake()
    {
        manager = GameObject.Find("UpdateManager").GetComponent<UpdateManager>();
        manager.Register(this);
    }

    public void LogOut()
    {
        manager.LogOut(this);
    }

    abstract public void Initialize();

    abstract public void MyUpdate();

    abstract public UpdateType GetUpdateType();

    abstract public int GetPriorityInType();

}
