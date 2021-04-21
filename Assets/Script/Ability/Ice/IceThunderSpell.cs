using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceThunderSpell : Spell
{
    
    public const float ICE_THUNDER_TIME = 0.4f;

    
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceThunder);
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
