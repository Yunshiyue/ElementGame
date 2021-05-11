using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceHealSpell : Spell
{
    private DefencePlayer defencePlayer;
    public override void Initialize()
    {
        base.Initialize();
        playerAnim.SetSpell(this, Spell.SkillType.IceHeal);
        defencePlayer = player.GetComponent<DefencePlayer>();
    }
    public override void Cast()
    {

        Debug.Log("进入冰疗cast！");
        playerAnim.SetUseSkillType(SkillType.IceHeal);
    }
    public override void ReleaseSpell()
    {
        //启动动画特效
        //TODO
        defencePlayer.Heal(2);
    }

    public override void Disable()
    {
    }

    public override void Enable()
    {
    }

}
