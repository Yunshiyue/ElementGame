/**
 * @Description: DestructiblePlatform为可破坏的地形，可被攻击破坏
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlatform : Mechanism
{
    private CanBeFoughtMachanism canBefought;
    public override void Trigger(TiggerType triggerType)
    {
        GameObject.Destroy(gameObject);
    }

    protected override bool TriggerDetect()
    {
        return true;
    }
}
