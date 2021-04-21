using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveFireBallSpell : FlyingSpell
{
    private Vector2 flyingStartPositon = new Vector2();
    public override void Initialize()
    {
        base.Initialize();
        spellName = ProtectiveFireBall.PROTECTIVE_FIRE_BALL;
        playerAnim.SetSpell(this, SkillType.ProtectiveFireBall);
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.ProtectiveFireBall);
    }

    public override void ReleaseSpell()
    {
        /**第1个**/
        GameObject protectiveFireBall1 = poolManager.GetGameObject("ProtectiveFireBall");
        ProtectiveFireBall a = protectiveFireBall1.GetComponent<ProtectiveFireBall>();
        a.SetThrower(player);

        //设置生成位置
        flyingStartPositon.y = player.transform.position.y + 1.0f;
        flyingStartPositon.x = player.transform.position.x;
        a.SetStartPosition(flyingStartPositon);

        /**第2个**/
        GameObject protectiveFireBall2 = poolManager.GetGameObject("ProtectiveFireBall");
        ProtectiveFireBall b = protectiveFireBall2.GetComponent<ProtectiveFireBall>();
        b.SetThrower(player);

        //设置生成位置
        flyingStartPositon.y = player.transform.position.y - 1.0f;
        flyingStartPositon.x = player.transform.position.x - 1.0f;
        b.SetStartPosition(flyingStartPositon);

        /**第3个**/
        GameObject protectiveFireBall3 = poolManager.GetGameObject("ProtectiveFireBall");
        ProtectiveFireBall c = protectiveFireBall3.GetComponent<ProtectiveFireBall>();
        c.SetThrower(player);

        //设置生成位置
        flyingStartPositon.y = player.transform.position.y - 1.0f;
        flyingStartPositon.x = player.transform.position.x + 1.0f;
        c.SetStartPosition(flyingStartPositon);
    }
}
