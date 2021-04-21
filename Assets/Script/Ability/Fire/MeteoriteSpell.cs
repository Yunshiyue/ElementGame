using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteSpell : FlyingSpell
{
    private Vector2 flyingStartPositon = new Vector2(0, 0);
    private Vector2 flyingDirection = new Vector2(1, 0);
    public override void Initialize()
    {
        base.Initialize();
        spellName = Meteorite.METEORITE;
        playerAnim.SetSpell(this, SkillType.Meteorite);
    }

    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.Meteorite);
    }

    public override void ReleaseSpell()
    {
        GameObject meteorite = poolManager.GetGameObject(spellName);
        Meteorite a = meteorite.GetComponent<Meteorite>();
        a.SetThrower(player);

        //设置生成位置
        flyingStartPositon.x = player.transform.position.x;
        flyingStartPositon.y = player.transform.position.y + 5.0f;
        a.SetStartPosition(flyingStartPositon);

        //设置方向
        if (movementComponent.IsFacingLeft())
        {
            flyingDirection.x = -1;
        }
        else
        {
            flyingDirection.x = 1;
        }
        flyingDirection.y = -1;
        a.SetDirection(flyingDirection);
    }
}
