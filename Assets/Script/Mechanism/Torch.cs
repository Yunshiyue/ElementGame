/**
 * @Description: Torch是火把类，是机关的一种，被火属性攻击后会被点燃
 * @Author: CuteRed

 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class Torch : Mechanism
{
    private CanBeFoughtMachanism canBeFought;
    private new Light2D light;
    private bool isAttackedByFire = false;

    protected override void Start()
    {
        base.Start();

        canBeFought = GetComponent<CanBeFoughtMachanism>();
        if (canBeFought == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanBeFoughtMachanism组件");
        }

        light = GetComponent<Light2D>();
        if (light == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到Light组件");
        }

        //初始化灯光
        light.enabled = false;
    }

    public override void Trigger(TiggerType triggerType)
    {
        //判断攻击类型是否为火
        if (triggerType == Mechanism.TiggerType.Fire)
        {
            isAttackedByFire = true;
        }

        //检测触发条件
        if (TriggerDetect())
        {
            light.enabled = true;
        }
    }

    protected override bool TriggerDetect()
    {
        return isAttackedByFire;
    }
}
