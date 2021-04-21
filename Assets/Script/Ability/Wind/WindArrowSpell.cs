using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArrowSpell : FlyingSpell
{
    private Vector2 flyingStartPositon = new Vector2();
    private WindAbility windAbility;
    public override void Initialize()
    {
        base.Initialize();
        spellName = WindArrow.WIND_ARROW;
        windAbility = player.GetComponent<WindAbility>();
        playerAnim.SetSpell(this,SkillType.WindIce);
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.WindIce);
    }
    public void ReleaseCast()
    {
        GameObject windArrow = poolManager.GetGameObject("WindArrow");
        WindArrow a = windArrow.GetComponent<WindArrow>();
        a.SetThrower(player);

        //根据风瞄准镜设置生成位置
        flyingStartPositon.y = player.transform.position.y;
        flyingStartPositon.x = player.transform.position.x + 1.0f * player.transform.localScale.x;
        a.SetStartPosition(flyingStartPositon);

        //设置方向
        a.SetDirection(windAbility.GetSightHeadPosition());
    }

    public override void ReleaseSpell()
    {
        
    }
}
