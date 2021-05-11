/**
 * @Description: SingleSwitch为可被任意元素触发的开关类，被指定属性攻击后可触发多个机关生效，并生成巫师（一对多）
 * @Author: CuteRed

 *     
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSwitch : Mechanism
{
    [Header("触发参数")]
    public Mechanism.TiggerType acceptTriggerType;
    private CanBeFoughtMachanism canBeFought;
    private bool isAttacked = false;
    public GameObject[] mechanisms;
    private int triggerNum = 0;

    [Header("时间参数")]
    public float triggerIntervalTime = 2.0f;
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

        if (mechanisms.Length == 0)
        {
            Debug.LogError("在" + gameObject.name + "中，未初始化mechanisms");
        }

        passTime = triggerIntervalTime;

        enabled = false;
    }

    private void Update()
    {
        //判断时间间隔
        if (passTime > triggerIntervalTime)
        {
            mechanisms[triggerNum].GetComponent<Mechanism>().Trigger(Mechanism.TiggerType.Switch);
            triggerNum++;
            passTime = 0.0f;
        }
        //机关触发完成
        else if (triggerNum == mechanisms.Length)
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
    /// 被指定元素触发，触发相应机关
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        //判断是否被指定类型条件触发
        if (triggerType == acceptTriggerType)
        {
            isAttacked = true;
        }
        
        //检查是否满足触发条件
        if (TriggerDetect())
        {
            //机关触发生效
            isTriggered = true;
            enabled = true;
        }
    }

    /// <summary>
    /// 触发检测（检测是否被指定元素攻击）
    /// </summary>
    /// <returns></returns>
    protected override bool TriggerDetect()
    {
        return isAttacked;
    }
}
