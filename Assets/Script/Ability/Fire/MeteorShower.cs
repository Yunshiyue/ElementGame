using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShower : FlyingAbility
{
    public const string METEORITE = "MeteorShower";

    [Header("运动参数")]
    private Vector3 direction;

    [Header("消失检测参数")]
    private bool isTouchCreature = false;

    private LayerMask playerLayer;

    public override void Initialize()
    {
        base.Initialize();

        maxExistTime = 3f;
        speed = 8f;
        playerLayer = LayerMask.NameToLayer("Player");
    }

    public override void MyUpdate()
    {
        //更新时间
        TimeUpdate();

        //消失检测
        if (DisappearDetect())
        {
            Disappear();
        }
        //运动
        else
        {
            Movement();
        }
    }

    /// <summary>
    /// 设置技能释放方向
    /// </summary>
    /// <param name="direction"></param>
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    /// <summary>
    /// 爆炸并消失
    /// </summary>
    protected override void Disappear()
    {
        //消失
        poolManager.RemoveGameObject(METEORITE, gameObject);

        //参数还原
        Clear();
    }

    /// <summary>
    /// 消失检测，超时、触碰敌人、触碰地图
    /// </summary>
    /// <returns></returns>
    protected override bool DisappearDetect()
    {
        return isTimeOut || isTouchCreature;

    }

    /// <summary>
    /// 移动，指定方向
    /// </summary>
    protected override void Movement()
    {
        //向指定方向移动
        gameObject.transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanBeFighted beFought;
        if (collision.TryGetComponent<CanBeFighted>(out beFought))
        {
            if (collision.gameObject.layer == targetLayer || collision.gameObject.layer == mechanism)
            {
                canFight.Attack(beFought, damage, AttackInterruptType.NONE, ElementAbilityManager.Element.Fire);
                isTouchCreature = true;
            }
            else if(collision.gameObject.layer == playerLayer)
            {
                canFight.Attack(beFought, damage + 1, AttackInterruptType.NONE, ElementAbilityManager.Element.Fire);
                isTouchCreature = true;
            }
        }
    }

    protected override void Clear()
    {
        base.Clear();

        isTimeOut = false;
        isTouchCreature = false;
    }
}
