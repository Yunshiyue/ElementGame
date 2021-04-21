using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderIceSpell : Spell
{

    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.ThunderIce);
    }

    public override void Disable()
    {
        
    }

    public override void Enable()
    {
        
    }

    public override void ReleaseSpell()
    {
        throw new System.NotImplementedException();
    }
}
