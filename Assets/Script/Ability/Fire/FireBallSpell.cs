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
    }


    public override void Cast()
    {

    }
    public void ReleaseSpell()
    {
        GameObject fireBallAbility = poolManager.GetGameObject("FireBallAbility");
        FireBallAbility a = fireBallAbility.GetComponent<FireBallAbility>();
        a.SetThrower(player);
        a.SetStartPosition(player.transform.position);

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
