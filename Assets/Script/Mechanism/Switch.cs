/**
 * @Description: Switch为开关类，被雷属性攻击后可触发可移动平台生效
 * @Author: CuteRed

 *     
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Mechanism
{
    [Header("触发参数")]
    private CanBeFoughtMachanism canBeFought;
    private bool isAttackedByThunder = false;
    private TriggerablePlatform platform;

    protected override void Start()
    {
        base.Start();

        canBeFought = GetComponent<CanBeFoughtMachanism>();
        if (canBeFought == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanBeFoughtMachanism组件");
        }

        platform = GameObject.Find("TriggerablePlatform").GetComponent<TriggerablePlatform>();
        if (platform == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取MovablePlatform失败");
        }

    }

    public override void Trigger(TiggerType triggerType)
    {
        if (triggerType == TiggerType.Thunder)
        {
            isAttackedByThunder = true;
        }

        if (TriggerDetect())
        {
            platform.Trigger(TiggerType.Other);

            //机关失效
            enabled = false;
        }
    }

    protected override bool TriggerDetect()
    {
        return isAttackedByThunder;
    }
}
