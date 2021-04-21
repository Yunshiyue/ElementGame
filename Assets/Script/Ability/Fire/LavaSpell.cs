using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSpell : FlyingSpell
{
    private Vector2 flyingStartPositon = new Vector2();
    public override void Initialize()
    {
        base.Initialize();
        spellName = Lava.LAVA;
        playerAnim.SetSpell(this,SkillType.Lava);
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.Lava);
    }
    public override void ReleaseSpell()
    {
        GameObject lava = poolManager.GetGameObject("Lava");
        Lava a = lava.GetComponent<Lava>();
        a.SetThrower(player);

        //设置生成位置
        flyingStartPositon.x = player.transform.position.x;
        flyingStartPositon.y = player.transform.position.y + 1;
        a.SetStartPosition(flyingStartPositon);

        //设置方向
        a.SetDirection(player.transform.localScale.x);
    }
}
