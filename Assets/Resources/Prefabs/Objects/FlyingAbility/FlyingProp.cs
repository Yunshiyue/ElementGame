/**
 * @Description: FlyingProp类是飞行道具抽象类，包含抽象方法移动和超时检测
 * @Author: CuteRed

 *     
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingProp : myUpdate
{
    protected Collider2D coll;

    protected int priorityInType = 0;
    protected UpdateType updateType = UpdateType.PoolThing;

    [Header("时间参数")]
    protected const float MAX_EXIST_TIME = 5f;
    protected float existTime = 0f;

    /// <summary>
    /// 移动
    /// </summary>
    protected abstract void Movement();

    public override void Initialize()
    {
        coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取collider错误");
        }
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }


    /// <summary>
    /// 超时检测，超过最大时间后，销毁
    /// </summary>
    protected virtual void TimeOutDetect()
    {
        if (existTime > MAX_EXIST_TIME)
        {
            Disappear();
        }
    }
    abstract protected void Disappear();
}
