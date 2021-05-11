using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSpell : FlyingSpell
{
    private Vector2 flyingDirection = new Vector2(1, 0);
    public override void Initialize()
    {
        base.Initialize();
        base.spellName = FireBallAbility.FIRE_BALL_ABILITY;
        playerAnim.SetSpell(this, SkillType.FireBall);
    }


    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.FireBall);
    }
    public override void ReleaseSpell()
    {
        GameObject fireBallAbility = poolManager.GetGameObject("FireBallAbility");
        FireBallAbility a = fireBallAbility.GetComponent<FireBallAbility>();
        a.SetThrower(player);
        a.SetTargetLayerName("Enemy");
        a.SetStartPosition(player.transform.position);
        a.SetMaxExistTime(0.7f);

        if (movementComponent.IsFacingLeft())
        {
            flyingDirection.x = -1;
        }
        else
        {
            flyingDirection.x = 1;
        }

        a.SetDirection(flyingDirection);
    }
}
