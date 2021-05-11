using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceHammerSpell : Spell
{
    public const float ICE_HAMMER_TIME = 0.8f;
    private BoxCollider2D iceShotFreezingZone;
    private IceAbility iceAbility;
    public override void Initialize()
    {
        base.Initialize();
        iceAbility = player.GetComponent<IceAbility>();
        iceShotFreezingZone = GameObject.Find("IceShotFreezingZone").GetComponent<BoxCollider2D>();
        if (iceShotFreezingZone == null)
        {
            Debug.LogError("iceShotFreezingZone为空");
        }
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceHammer);
        iceAbility.FreezingZone(iceShotFreezingZone);
    }

    public override void Disable()
    {
       
    }

    public override void Enable()
    {
       
    }

    public override void ReleaseSpell()
    {
       
    }
}
