/**
 * @Description: CounterRotationPlatform类事计数旋转平台类，会根据跳跃次数改变角度
 * 奇数改变为angle1，偶数改变为angle2
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterRotationPlatform : Mechanism
{
    [Header("跳跃参数")]
    /// <summary>
    /// 跳跃次数的奇偶
    /// </summary>
    private int jumpNum = 0;

    [Header("角度参数")]
    public float angle1 = 0.0f;
    public float angle2 = 0.0f;

    private Vector3 rotationAngle = Vector3.zero; 

    private EventManager eventManager;

    protected override void Start()
    {
        base.Start();

        //初始化事件管理器
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        if (eventManager == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到EventManager");
        }

        //添加事件监听
        eventManager.AddEvent(EventManager.OnJumpNumChange, (object[] o) => {
            jumpNum = (int)o[0];
            Trigger(TiggerType.Other);
            return null;
        });

        //初始化状态（改变角度）
        rotationAngle.z = angle1;
        transform.rotation = Quaternion.Euler(rotationAngle);
    }

    /// <summary>
    /// 此机关中不用
    /// </summary>
    /// <returns></returns>
    protected override bool TriggerDetect()
    {
        return true;
    }

    /// <summary>
    /// 触发机关，根据跳跃次数改变角度
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        //跳跃次数为奇数
        if (jumpNum == 1)
        {
            rotationAngle.z = angle1;
        }
        //跳跃次数为偶数
        else if (jumpNum == 2)
        {
            rotationAngle.z = angle2;
        }

        //改变角度
        transform.rotation = Quaternion.Euler(rotationAngle);
    }
}
