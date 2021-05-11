using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShotSpell : FlyingSpell
{
    private Vector2 backVector = new Vector2(-4, 0);
    private float backTime = 0.3f;

    private bool isOn = false;
    private float IntervalTime = 0.1f;
    private float curShotTime = 0f;
    private int shotNumber = 3;
    private int curShotNumber = 0;

    private BoxCollider2D iceShotFreezingZone;
    private IceAbility iceAbility;
    public override void Initialize()
    {
        base.Initialize();
        spellName = IceArrow.ICE_ARROW;
        playerAnim.SetSpell(this, SkillType.IceShot);

        iceAbility = player.GetComponent<IceAbility>();
        iceShotFreezingZone = GameObject.Find("IceShotFreezingZone").GetComponent<BoxCollider2D>();
        if(iceShotFreezingZone == null)
        {
            Debug.LogError("iceShotFreezingZone为空");
        }
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceShot);
    }

    public override void ReleaseSpell()
    {
        movementComponent.RequestMoveByTime(backVector, backTime, MovementPlayer.MovementMode.Ability);
        isOn = true;
        iceAbility.FreezingZone(iceShotFreezingZone);
    }
    public void IceShotClock()
    {
        if(isOn)
        {
            curShotTime += Time.deltaTime;
            if(curShotTime >= IntervalTime)
            {
                curShotTime = 0f;
                Shot();
                curShotNumber++;
                if(curShotNumber >= shotNumber)
                {
                    isOn = false;
                    curShotNumber = 0;
                }
            }
        }
    }
    private void Shot()
    {
        //Debug.Log("射出冰箭");
        GameObject iceArrow = poolManager.GetGameObject(spellName);
        IceArrow script = iceArrow.GetComponent<IceArrow>();
        script.SetThrower(player);
        Vector2 startDirection = player.transform.position;
        startDirection.x += movementComponent.IsFacingLeft() ? -1 : 1;
        script.SetStartPosition(startDirection);
        script.SetDirection(movementComponent.IsFacingLeft() ? Vector2.left : Vector2.right);
    }
}
