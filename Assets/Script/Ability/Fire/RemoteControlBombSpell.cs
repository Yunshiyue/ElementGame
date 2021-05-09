using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControlBombSpell : FlyingSpell
{
    private RemoteControlBomb currentBomb;
    private Vector2 throwForce = new Vector2(2, 2);
    private bool isBroughtout = false;
    private float bombCurTime = 0f;
    private float bombTotalTme = 5f;
    private Quaternion originRotation = Quaternion.identity;
    //private Vector2 generatePosition = new Vector2();
    private enum SpellStatus { BeforeBringout, BeforeThrow, BeforeBomb }
    private SpellStatus status;
    public override void Initialize()
    {
        base.Initialize();
        spellName = RemoteControlBomb.REMOTE_CONTROL_BOMB_NAME;
        playerAnim.SetSpell(this, SkillType.RemoteControlBomb);
        status = SpellStatus.BeforeBringout;
    }
    public override void Cast()
    {
        if(status == SpellStatus.BeforeBringout)
        {
            //申请动画组件
            playerAnim.SetUseSkillType(SkillType.RemoteControlBomb);
            status = SpellStatus.BeforeThrow;
        }
        else if(status == SpellStatus.BeforeThrow)
        {
            currentBomb.transform.SetParent(null);
            throwForce.x = player.GetComponent<MovementPlayer>().IsFacingLeft() ? -4 : 4;
            currentBomb.GetComponent<Rigidbody2D>().isKinematic = false;
            currentBomb.GetComponent<Rigidbody2D>().AddForce(throwForce, ForceMode2D.Impulse);
            status = SpellStatus.BeforeBomb;
        }
        else
        {
            currentBomb.Bomb();
            status = SpellStatus.BeforeBringout;
            isBroughtout = false;
            bombCurTime = 0f;
        }
    }
    public void BombClock()
    {
        if(isBroughtout)
        {
            bombCurTime += Time.deltaTime;
            if(bombCurTime >= bombTotalTme)
            {
                currentBomb.Bomb();
                bombCurTime = 0f;
                status = SpellStatus.BeforeBringout;
                isBroughtout = false;
            }
        }
    }
    public override void ReleaseSpell()
    {
        Debug.Log("召唤出来炸弹！");
        GameObject bomb = poolManager.GetGameObject(spellName);
        bomb.transform.SetParent(player.transform);
        bomb.transform.localPosition = new Vector2(0, 1.5f);
        bomb.transform.rotation = originRotation;
        bomb.GetComponent<Rigidbody2D>().isKinematic = true;

        currentBomb = bomb.GetComponent<RemoteControlBomb>();
        currentBomb.SetThrower(player);
        isBroughtout = true;
    }

    public int GetNextSpellCost()
    {
        switch(status)
        {
            case SpellStatus.BeforeThrow:
                return 1;
            default:
                return 0;
        }
    }
    public override void Disable()
    {
    }

    public override void Enable()
    {
    }

}
