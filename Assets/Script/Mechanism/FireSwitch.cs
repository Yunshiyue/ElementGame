/**
 * @Description: FireSwitch为可被火元素触发的开关类，被火属性攻击后可触发多个可移动平台生效，并生成巫师
 * @Author: CuteRed

 *     
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSwitch : Mechanism
{
    [Header("触发参数")]
    public Mechanism.TiggerType acceptTriggerType;
    private CanBeFoughtMachanism canBeFought;
    private bool isAttackedByFire = false;
    private bool hasTriggered = false;
    public GameObject[] platforms;
    private int triggerNum = 0;

    [Header("时间参数")]
    private float intervalTime = 2.0f;
    private float passTime = 2.0f;

    public GameObject wizard;

    protected override void Start()
    {
        base.Start();

        canBeFought = GetComponent<CanBeFoughtMachanism>();
        if (canBeFought == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanBeFoughtMachanism组件");
        }
        if(wizard != null)
        {
            wizard.SetActive(false);
        }

        enabled = false;
    }

    private void Update()
    {
        //判断时间间隔
        if (passTime > intervalTime)
        {
            platforms[triggerNum].GetComponent<Mechanism>().Trigger(Mechanism.TiggerType.Other);
            triggerNum++;
            passTime = 0.0f;
        }
        //机关触发完成
        else if (triggerNum == platforms.Length)
        {
            if (wizard != null)
            {
                wizard.SetActive(true);
            }

            //机关失效
            enabled = false;
        }
        passTime += Time.deltaTime;
    }

    /// <summary>
    /// 被火元素触发，触发相应地形
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        //判断火元素
        if (triggerType == acceptTriggerType)
        {
            isAttackedByFire = true;
        }
        

        if (TriggerDetect())
        {
            //机关触发生效
            enabled = true;

        }
    }

    protected override bool TriggerDetect()
    {
        return isAttackedByFire;
    }

    /// <summary>
    /// 等待一定时间后，跳出函数
    /// </summary>
    private void waitTime()
    {
        while (passTime < intervalTime)
        {
            passTime += Time.deltaTime;
        }
        passTime = 0.0f;
    }
}
