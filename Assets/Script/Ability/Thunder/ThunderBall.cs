/**
 * @Description: FireBallAbility是技能火球类，即短按火
 * 技能描述：释放缓慢移动的雷球，碰到墙壁回弹，碰到敌人消失。
 * 要素：起点
 * 移动规则：直线、碰到墙壁反弹
 * 消失规则：敌人、时间、与主角的距离
 * 接触伤害
 * 接触墙体效果
 * 
 * 初始化说明：方向
 * 
 * @Author: CuteRed

 * 

 * @Editor: CuteRed
 * @Edit: 新增了反弹效果
 *    

 * @Editor: CuteRed
 * @Edit: 以前设置scale时，会直接根据发射方向的x确定，但如果x=0会出错，现对此bug进行了修复
 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBall : FlyingAbility
{
    public const string Thunder_BALL = "ThunderBall";

    [Header("运动参数")]
    private Vector3 direction;

    [Header("消失检测参数")]
    private bool isTouchEnemy = false;
    private const float MAX_DISTANCE_TO_PLAYER = 1000.0f;
    private float distanceToPlayer = 0.0f;

    public override void Initialize()
    {
        base.Initialize();

        //设置目标层
        targetLayer = LayerMask.NameToLayer(targetLayerName);
        speed = 5f;
    }

    public override void MyUpdate()
    {
        //更新时间
        TimeUpdate();

        //更新与主角的距离
        distanceToPlayer = Vector2.Distance(gameObject.transform.position, thrower.transform.position);

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

        //改变朝向
        int x = 1;
        int y = 1;
        if (direction.x < 0)
        {
            x = -1;
        }
        if (direction.y < 0)
        {
            y = -1;
        }
        transform.localScale = new Vector3(x, y, 1);
    }

    /// <summary>
    /// 消失
    /// </summary>
    protected override void Disappear()
    {
        poolManager.RemoveGameObject(Thunder_BALL, gameObject);
        //参数还原
        Clear();
    }

    /// <summary>
    /// 消失检测，超时、触碰敌人、触碰地图
    /// </summary>
    /// <returns></returns>
    protected override bool DisappearDetect()
    {
        return isTimeOut || isTouchEnemy || (distanceToPlayer >= MAX_DISTANCE_TO_PLAYER);
    }

    /// <summary>
    /// 移动，指定方向
    /// </summary>
    protected override void Movement()
    {
        //向指定方向移动
        gameObject.transform.position += direction * speed * Time.deltaTime;

        //检测碰到地图，反弹
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, 0.2f, ground);
        if (hit)
        {
            Vector2 normal = hit.normal;
            Vector2 reflect = Vector2.Reflect(direction, normal);
            direction = reflect;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanBeFighted beFought;
        if (collision.TryGetComponent<CanBeFighted>(out beFought) 
            && (collision.gameObject.layer == targetLayer || collision.gameObject.layer == mechanism))
        {
            //不可对同一目标造成2次伤害
            if (!fought.Contains(beFought))
            {
                canFight.Attack(beFought, damage, AttackInterruptType.NONE, ElementAbilityManager.Element.Thunder);
                fought.Add(beFought);
                Debug.Log("ThunderBall对" + beFought.name + "造成伤害");

                //碰到敌人消失
                isTouchEnemy = true;
            }
        }
    }

    protected override void Clear()
    {
        base.Clear();

        isTouchEnemy = false;
        distanceToPlayer = 0.0f;
    }
}
