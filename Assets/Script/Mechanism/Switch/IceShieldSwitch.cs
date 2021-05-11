/**
 * @Description: IceShieldSwitch是冰盾开关类，由冰盾触发
 * @Author: CuteRed

 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShieldSwitch : Mechanism
{
    public Mechanism mechanism;
    protected override void Start()
    {
        base.Start();

        if (mechanism == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到mechanism");
        }
    }

    public override void Trigger(TiggerType triggerType)
    {
        if (TriggerDetect())
        {
            isTriggered = true;
            mechanism.Trigger(Mechanism.TiggerType.Switch);
        }
    }

    protected override bool TriggerDetect()
    {
        return !isTriggered;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //冰盾触发
        if (collision.gameObject.name == "IceShield")
        {
            Trigger(TiggerType.Other);
        }
    }
}
