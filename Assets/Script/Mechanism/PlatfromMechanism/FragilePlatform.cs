/**
 * @Description: FragilePlatform是易碎平台类，触碰后，经过一定时间，会销毁
 * @Author: CuteRed

 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragilePlatform : Mechanism
{
    [Header("时间参数")]
    public float disappearTime = 2.0f;

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 触发（消失）
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        StartCoroutine(nameof(Disappear));
    }

    /// <summary>
    /// 检测机关是否被触发
    /// </summary>
    /// <returns></returns>
    protected override bool TriggerDetect()
    {
        return isTriggered;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        Trigger(TiggerType.Other);
    }


    /// <summary>
    /// 消失
    /// </summary>
    /// <returns></returns>
    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(disappearTime);

        Destroy(gameObject);
    }
}