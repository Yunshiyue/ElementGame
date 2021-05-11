/**
 * @Description: ProtectiveFireBall是保护性火球，即火+冰
 * 技能描述：在自己周围生成若干个保护性火球，火球围绕自己旋转，碰到火球的敌人造成伤害，可以产生持续性明亮视野和点燃效果
 * 要素：起点
 * 移动轨迹：旋转
 * 消失判定：飞行时间
 * 接触伤害
 * 
 * 初始化说明：无
 * 
 * @Author: CuteRed

 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveFireBall : FlyingAbility
{
    public const string PROTECTIVE_FIRE_BALL = "ProtectiveFireBall";

    [Header("运动参数")]
    private float rotateSpeed = 20.0f;

    public override void Initialize()
    {
        base.Initialize();

        speed = 700.0f;
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
    }

    /// <summary>
    /// 消失
    /// </summary>
    protected override void Disappear()
    {
        poolManager.RemoveGameObject(PROTECTIVE_FIRE_BALL, gameObject);

        //参数还原
        Clear();
    }

    /// <summary>
    /// 消失检测，超时、触碰敌人、触碰地图
    /// </summary>
    /// <returns></returns>
    protected override bool DisappearDetect()
    {
        return isTimeOut;

    }

    /// <summary>
    /// 移动，即围绕玩家旋转和自旋
    /// </summary>
    protected override void Movement()
    {
        //围绕玩家旋转
        transform.RotateAround(thrower.transform.position, Vector3.forward, speed * Time.deltaTime);

        //自旋
        transform.Rotate(-Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanBeFighted beFought;
        if (collision.TryGetComponent<CanBeFighted>(out beFought) && (collision.gameObject.layer == targetLayer || collision.gameObject.layer == mechanism))
        {
            //不可对同一目标造成2次伤害
            if (!fought.Contains(beFought))
            {
                canFight.Attack(beFought, damage, AttackInterruptType.NONE, ElementAbilityManager.Element.Fire);
                fought.Add(beFought);
                Debug.Log("ProtectiveFireBall对" + beFought.name + "造成伤害");
            }
        }
    }
}
