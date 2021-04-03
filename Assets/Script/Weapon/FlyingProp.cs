/**
 * @Description: FlyingProp类是飞行道具抽象类，包含抽象方法移动和超时检测
 * @Author: CuteRed

 *     
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingProp : MonoBehaviour
{
    public Collider2D coll;

    [Header("时间参数")]

    protected const float MAX_EXIST_TIME = 5f;

    /// <summary>
    /// 道具生成时间
    /// </summary>
    protected float startTime;

    /// <summary>
    /// 移动
    /// </summary>
    protected abstract void Movement();

    void Start()
    {
        startTime = Time.time;
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取collider错误");
        }
    }

    /// <summary>
    /// 超时检测，超过最大时间后，销毁
    /// </summary>
    protected virtual void TimeOutDetect()
    {
        if (startTime + MAX_EXIST_TIME > Time.time)
        {
            Disappear();
        }
    }
    protected void Disappear()
    {
        gameObject.SetActive(false);
    }
}
