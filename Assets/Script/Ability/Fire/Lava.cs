/**
 * @Description: Lava类为火山爆发技能类，即火+风
 * 技能描述：火山爆发
 * 随机向周围发射有重力效果的熔岩，碰到敌人造成直接伤害，碰到地图会生成小片熔岩，对途径的敌人造成少量灼烧伤害。
 * 要素：起点、rigidbody
 * 移动轨迹：抛物线
 * 消失判定：时间、墙体、敌人
 * 接触伤害
 * 接触墙体效果
 * 
 * 初始化说明：方向
 * 
 * @Author: CuteRed

 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : FlyingAbility
{
    public const string LAVA = "Lava";

    [Header("运动参数")]
    private Rigidbody2D rigid;
    private float direction;
    private float horizontalForce;
    private float verticalForce;
    private Vector2 force = new Vector2();

    [Header("消失检测参数")]
    private bool isTouchGround = false;

    public override void Initialize()
    {
        base.Initialize();

        //设置目标层
        targetLayer = LayerMask.NameToLayer(targetLayerName);

        //设置刚体
        rigid = GetComponent<Rigidbody2D>();
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
    /// 生成小范围岩浆，并消失
    /// </summary>
    protected override void Disappear()
    {
        GenerateLavaAoe();
        poolManager.RemoveGameObject(LAVA, gameObject);

        //参数还原
        Clear();
    }

    /// <summary>
    /// 消失检测，超时、触碰敌人、触碰地图
    /// </summary>
    /// <returns></returns>
    protected override bool DisappearDetect()
    {
        return isTimeOut || isTouchGround;

    }

    /// <summary>
    /// 移动（只检测碰撞墙体）
    /// </summary>
    protected override void Movement()
    {
        //检测碰到地图
        if (coll.IsTouchingLayers(ground))
        {
            Debug.Log(gameObject.name + "碰到墙");
            isTouchGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanBeFighted beFought;
        if (collision.TryGetComponent<CanBeFighted>(out beFought) && 
            (collision.gameObject.layer == targetLayer || collision.gameObject.layer == mechanism))
        {
            //不可对同一目标造成2次伤害
            if (!fought.Contains(beFought))
            {
                canFight.Attack(beFought, damage, AttackInterruptType.NONE, ElementAbilityManager.Element.Fire);
                fought.Add(beFought);
                Debug.Log("FireBallAbility对" + beFought.name + "造成伤害");
            }
        }
    }

    /// <summary>
    /// 设置方向
    /// </summary>
    /// <param name="direction"></param>
    public void SetDirection(float direction)
    {
        this.direction = direction;

        Init();
    }

    /// <summary>
    /// 初始化运动参数
    /// </summary>
    private void Init()
    {
        //加随机力
        horizontalForce = Random.Range(-5f, 5.0f);
        verticalForce = Random.Range(4.0f, 8.0f);
        force.x = horizontalForce;
        force.y = verticalForce;
        rigid.velocity = force;
    }

    /// <summary>
    /// 生成小范围岩浆
    /// </summary>
    private void GenerateLavaAoe()
    {
        GameObject lavaAoe = poolManager.GetGameObject(LavaAoe.LAVA_AOE, transform.position);
        LavaAoe lava = lavaAoe.GetComponent<LavaAoe>();
        lava.SetStartPosition(transform.position);
        lava.SetThrower(thrower);
        lava.SetTargetLayerName("Enemy");
        Debug.Log(gameObject.name + "爆炸，生成小范围岩浆");
    }

    protected override void Clear()
    {
        base.Clear();

        isTouchGround = false;
    }
}
