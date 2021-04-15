﻿/**
 * @Description: Meteorite类是陨石类，即长按火
 * 技能描述：召唤陨石，面前方向aoe，强硬直；对陨石经过路线上的敌人也造成伤害
 * 要素：起点
 * 移动轨迹：直线
 * 消失判定：飞行后阶段：墙体、飞行时间
 * 接触伤害
 * 消失效果：爆炸
 * 
 * 初始化说明：方向
 * 
 * @Author: CuteRed

 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : FlyingAbility
{
    public const string METEORITE = "Meteorite";

    [Header("时间参数")]
    private float timePhase1 = 0.5f;

    [Header("运动参数")]
    private Vector3 direction;

    [Header("消失检测参数")]
    private bool isTouchGround = false;

    protected new int priorityInType = 5;

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
    /// 爆炸并消失
    /// </summary>
    protected override void Disappear()
    {
        //爆炸，造成AOE伤害
        canFight.AttackArea(coll, damage);

        //消失
        poolManager.RemoveGameObject(METEORITE, gameObject);
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
        return isTimeOut || isTouchGround;

    }

    /// <summary>
    /// 移动，指定方向
    /// </summary>
    protected override void Movement()
    {
        //向指定方向移动
        gameObject.transform.position += direction * speed * Time.deltaTime;

        //检测碰到地图且处于后阶段
        if (Physics2D.Raycast(gameObject.transform.position, Vector2.down, 0.5f, ground) && existTime > timePhase1)
        {
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
                Debug.Log("Meteorite对" + beFought.name + "造成伤害");
            }
        }
    }

    protected override void Clear()
    {
        base.Clear();

        isTouchGround = false;
    }
}
