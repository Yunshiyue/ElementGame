using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrowSpell : FlyingSpell
{
    public const float ICE_ARROW_TIME = 0.6f;
    public override void Initialize()
    {
        base.Initialize();
        spellName = IceArrow.ICE_ARROW;
        playerAnim.SetSpell(this, SkillType.IceArrow);
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceArrow);
    }
    public override void ReleaseSpell()
    {
        GameObject iceArrow = poolManager.GetGameObject(spellName);
        IceArrow script = iceArrow.GetComponent<IceArrow>();
        script.SetThrower(player);
        Vector2 startDirection = player.transform.position;
        startDirection.x += movementComponent.IsFacingLeft() ? -1 : 1;
        script.SetStartPosition(startDirection);
        script.SetMaxExistTime(0.8f);
        script.SetDirection(movementComponent.IsFacingLeft() ? Vector2.left : Vector2.right);
    }

    public override void Disable()
    {
    }

    public override void Enable()
    {
    }

}
