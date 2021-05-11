using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBallSpell : FlyingSpell
{
    private Vector2 flyingDirection = new Vector2();
    public override void Initialize()
    {
        base.Initialize();
        spellName = ThunderBall.Thunder_BALL;
        playerAnim.SetSpell(this,SkillType.ThunderBall);

    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.ThunderBall);
    }
    public override void ReleaseSpell()
    {
        GameObject thunderBall = poolManager.GetGameObject("ThunderBall");
        ThunderBall a = thunderBall.GetComponent<ThunderBall>();
        a.SetThrower(player);
        a.SetStartPosition(player.transform.position);

        flyingDirection.x = player.transform.localScale.x;
        flyingDirection.y = 0;
        a.SetDirection(flyingDirection);

        a.SetMaxExistTime(10.0f);

        a.SetTargetLayerName("Enemy");
    }

   
}
