/**
 * @Description: Switch为开关类，被指定属性攻击后可触发指定机关（一对一）
 * @Author: CuteRed

 *  

 * @Editor: CuteRed
 * @Edit: 将部分变量public，该机关具有普适性
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Mechanism
{
    [Header("触发参数")]
    private CanBeFoughtMachanism canBeFought;
    private bool isAttacked = false;
    public Mechanism.TiggerType type;
    public GameObject mechanism;

    protected override void Start()
    {
        base.Start();

        canBeFought = GetComponent<CanBeFoughtMachanism>();
        if (canBeFought == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanBeFoughtMachanism组件");
        }

        if (mechanism == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取MovablePlatform失败");
        }

    }

    public override void Trigger(TiggerType triggerType)
    {
        if (triggerType == type)
        {
            isAttacked = true;
        }

        if (TriggerDetect())
        {
            mechanism.GetComponent<Mechanism>().Trigger(TiggerType.Switch);

            //机关失效
            enabled = false;
            isTriggered = true;
        }
    }

    protected override bool TriggerDetect()
    {
        return isAttacked;
    }
}
