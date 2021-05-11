/**
 * @Description: FireBallAbility是技能火球类，即短按火
 * 技能描述：火球，朝向方向直线火球，单体消失，墙体消失，距离消失
 * 要素：起点
 * 移动轨迹：直线
 * 消失判定：单体消失，墙体消失，飞行时间
 * 接触伤害
 * 
 * 初始化说明：方向
 * 
 * @Author: CuteRed

 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallAbility : FlyingAbility
{
    public const string FIRE_BALL_ABILITY = "FireBallAbility";

    [Header("运动参数")]
    private Vector3 direction;

    [Header("消失检测参数")]
    private bool isTouchEnemy = false;
    private bool isTouchGround = false;

    [Header("碰墙检测参数")]
    private ContactFilter2D filter = new ContactFilter2D();
    private Collider2D[] grounds = new Collider2D[1];

    public override void Initialize()
    {
        base.Initialize();

        //设置目标层
        targetLayer = LayerMask.NameToLayer(targetLayerName);

        //以下为碰墙检测参数的初始化
        filter.useNormalAngle = false;
        filter.useDepth = false;
        filter.useOutsideDepth = false;
        filter.useOutsideNormalAngle = false;
        filter.useTriggers = false;

        filter.useLayerMask = true;

        LayerMask layerMask = 0;
        layerMask ^= 1 << LayerMask.NameToLayer("Platform");

        filter.layerMask = layerMask;
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
        transform.localScale = new Vector3(direction.x, 1, 1);
    }

    /// <summary>
    /// 消失
    /// </summary>
    protected override void Disappear()
    {
        Debug.Log("FireBallAbility消失");
        poolManager.RemoveGameObject(FIRE_BALL_ABILITY, gameObject);

        //参数还原
        Clear();
    }

    /// <summary>
    /// 消失检测，超时、触碰敌人、触碰地图
    /// </summary>
    /// <returns></returns>
    protected override bool DisappearDetect()
    {
        return isTimeOut || isTouchEnemy || isTouchGround;
    }

    /// <summary>
    /// 移动，指定方向
    /// </summary>
    protected override void Movement()
    {
        //向指定方向移动
        transform.position += direction * speed * Time.deltaTime;

        //检测碰到地图
        int groundNumber = coll.OverlapCollider(filter, grounds);
        if (groundNumber != 0)
        {
            isTouchGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        CanBeFighted beFought;

        if (collision.TryGetComponent<CanBeFighted>(out beFought))
        {
            
            //不可对同一目标造成2次伤害
            if (collision.gameObject.layer == targetLayer && !fought.Contains(beFought))
            {             
                canFight.Attack(beFought, damage, AttackInterruptType.NONE, ElementAbilityManager.Element.Fire);
                fought.Add(beFought);
                Debug.Log("FireBallAbility对" + beFought.name + "造成伤害");

                //碰到敌人消失
                isTouchEnemy = true;
            }
            //机关
            else if (collision.gameObject.layer == mechanism)
            {
                Debug.Log("FireBallAbility对" + beFought.name + "造成伤害");
                canFight.Attack(beFought, damage, AttackInterruptType.NONE, ElementAbilityManager.Element.Fire);
            }
        }
    }

    protected override void Clear()
    {
        base.Clear();

        isTouchEnemy = false;
        isTouchGround = false;
    }
}
