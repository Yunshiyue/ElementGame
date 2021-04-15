/**
 * @Description: WindArrow类是风箭类，即风+冰
 * 技能描述：向瞄准方向射出一支风箭，路途上所碰到的敌人会被束缚到箭上跟随箭一起移动，直到敌人碰到墙体或者箭消失为止。
 * 要素：起点
 * 移动轨迹  直线
 * 消失判定：墙体消失、飞行时间
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

public class WindArrow : FlyingAbility
{
    public const string WIND_ARROW = "WindArrow";

    [Header("运动参数")]
    private Vector3 direction = new Vector3();

    [Header("消失检测参数")]
    private bool isTouchGround = false;

    protected new int priorityInType = 7;

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
        //运动并推动敌人
        else
        {
            Movement();
            Push();
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
        poolManager.RemoveGameObject(WIND_ARROW, gameObject);
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

        //检测碰到地图
        if (Physics2D.Raycast(gameObject.transform.position, direction, 0.5f, ground))
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
                Debug.Log("WindArrow对" + beFought.name + "造成伤害");
            }
        }
    }

    /// <summary>
    /// 推动敌人
    /// </summary>
    private void Push()
    {
        foreach (CanBeFighted beFought in fought)
        {
            beFought.gameObject.transform.position = gameObject.transform.position;
        }
    }

    protected override void Clear()
    {
        base.Clear();

        isTouchGround = false;
    }
}
