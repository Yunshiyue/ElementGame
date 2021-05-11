using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceThunderSpell : Spell
{
    
    public const float ICE_THUNDER_TIME = 0.4f;

    private BoxCollider2D[] iceThunderFreezingZone = new BoxCollider2D[2];
    private IceAbility iceAbility;
    public override void Initialize()
    {
        base.Initialize();
        iceAbility = player.GetComponent<IceAbility>();
        iceThunderFreezingZone = GameObject.Find("IceThunderFreezingZone").GetComponents<BoxCollider2D>();
        if (iceThunderFreezingZone == null)
        {
            Debug.LogError("iceThunderFreezingZone为空");
        }
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceThunder);
        iceAbility.FreezingZone(iceThunderFreezingZone[0]);
        iceAbility.FreezingZone(iceThunderFreezingZone[1]);
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
