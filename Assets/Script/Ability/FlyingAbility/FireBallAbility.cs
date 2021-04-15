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

    protected new int priorityInType = 1;
    

    public override void Initialize()
    {
        base.Initialize();

        //设置目标层
        targetLayer = LayerMask.NameToLayer(targetLayerName);
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
        Debug.Log(poolManager.gameObject.name);
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
        LogOut();

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
        //if (coll.IsTouchingLayers(ground))
        //{
        //    Debug.Log(gameObject.name + "碰到墙");
        //    isTouchGround = true;
        //}
        if (Physics2D.Raycast(gameObject.transform.position, direction, 0.5f, ground))
        {
            Debug.Log(gameObject.name + "碰到墙");
            isTouchGround = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanBeFighted beFought;
        if (collision.TryGetComponent<CanBeFighted>(out beFought) && collision.gameObject.layer == targetLayer)
        {
            //不可对同一目标造成2次伤害
            if (!fought.Contains(beFought))
            {
                canFight.Attack(beFought, damage);
                fought.Add(beFought);
                Debug.Log("FireBallAbility对" + beFought.name + "造成伤害");

                //碰到敌人消失
                isTouchEnemy = true;
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
