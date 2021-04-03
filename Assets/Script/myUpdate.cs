/**
 * @Description: 继承自MonoBehaviour，控制update顺序和优化执行效率的组件类。
 *               那些应该执行update的组件应该继承自这个类。updateManager类中规定了若干个优先级队列，分别对应不同的update元素，
 *               比如地图元素、player、怪物等。在每个队列中，按照自身的PriorityInType升序调用update。
 *               在awake方法中，会将自己注册进updatemanager中。
 * @Author: ridger

 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class myUpdate : MonoBehaviour
{
    private void Awake()
    {
        GameObject.Find("UpdateManager").GetComponent<UpdateManager>().Register(this);
    }
    abstract public void Initialize();

    abstract public void MyUpdate();

    public enum UpdateType { GUI, Map, Player, Enemy }

    abstract public UpdateType GetUpdateType();

    abstract public int GetPriorityInType();

}
