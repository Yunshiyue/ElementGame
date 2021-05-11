using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfExplosionSpell : Spell
{
    private DefencePlayer defencePlayer;
    private CanFight canFight;
    private CanBeFighted canBeFighted;
    public override void Initialize()
    {
        base.Initialize();
        playerAnim.SetSpell(this, SkillType.SelfExplosion);
        defencePlayer = player.GetComponent<DefencePlayer>();
        canFight = player.GetComponent<CanFight>();
        canBeFighted = player.GetComponent<CanBeFighted>();
    }

    public override void Cast()
    {
        //播放聚焦动画
        playerAnim.SetUseSkillType(SkillType.SelfExplosion);
    }

    public override void ReleaseSpell()
    {
        canFight.Attack(canBeFighted, defencePlayer.getHp());
        //启动爆炸特效
    }

    public override void Enable()
    {
    }

    public override void Disable()
    {
    }
}
