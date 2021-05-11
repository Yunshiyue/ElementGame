/**
 * @Description: IceCone为冰锥类，目前由冰锥发射器发射，碰到墙壁消失，可击碎冰墙
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCone : FlyingAbility
{
    public const string ICE_CONE = "IceCone";

    [Header("运动参数")]
    private Vector3 direction;

    [Header("消失检测参数")]
    private bool isTouchEnemy = false;
    private bool isTouchGround = false;

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
        poolManager.RemoveGameObject(ICE_CONE, gameObject);
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
        gameObject.transform.position += direction * speed * Time.deltaTime;

        //检测碰到地图
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, 0.2f, ground);
        if (hit)
        {
            isTouchGround = true;

            //遇到冰盾，摧毁
            if (hit.collider.gameObject.name == "IceShieldCollider")
            {
                hit.collider.gameObject.SetActive(false);
            }
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
                canFight.Attack(beFought, damage, AttackInterruptType.STRONG, ElementAbilityManager.Element.Ice);
                fought.Add(beFought);
                Debug.Log("IceCone对" + beFought.name + "造成伤害");

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
