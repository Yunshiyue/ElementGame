/**
 * @Description: Hurricane是飓风类，即长按风
 * 技能描述：瞄准飓风：蓄力过程中通过wasd操作准星选定飓风生成地点，蓄力结束后在选定的地点生成飓风，造成持续伤害和螺旋升天效果。同时也会上推地图物体。
 * 要素：起点
 * 消失判定：时间
 * 接触伤害、范围持续伤害
 * 
 * 初始化说明：无
 * 
 * @Author: CuteRed

 *
 *

 * @Editor: CuteRed
 * @Edit: 飓风不可击飞玩家
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurricane : FlyingAbility
{
    public const string HURRICANE = "Hurricane";

    [Header("伤害参数")]
    private float pushUpForce = 10;
    private Vector2 pushUpVector = new Vector2(0, 0);

    [Header("伤害时间参数")]
    private const float ATTACK_INTERVAL = 0.5f;
    private float passTime = 0.0f;

    private LayerMask player;

    public override void Initialize()
    {
        base.Initialize();

        //设置目标层
        targetLayer = LayerMask.NameToLayer(targetLayerName);

        //设置上升力
        pushUpVector.y = pushUpForce;

        player = LayerMask.NameToLayer("Player");
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
        //AOE伤害
        else
        {
            AoeAttack();
        }
    }

    /// <summary>
    /// 消失
    /// </summary>
    protected override void Disappear()
    {
        poolManager.RemoveGameObject(HURRICANE, gameObject);

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
    /// 不移动
    /// </summary>
    protected override void Movement()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != player)
        {
            //向上推动敌人或物体
            Rigidbody2D beFought;
            if (collision.TryGetComponent<Rigidbody2D>(out beFought))
            {
                PushUp(beFought);
                Debug.Log("Hurricane对" + beFought.name + "击飞");
            }
        }
    }

    /// <summary>
    /// 对指定刚体造成击飞效果
    /// </summary>
    /// <param name="rigidbody"></param>
    private void PushUp(Rigidbody2D rigidbody)
    {
        //pushUpVector.x = rigidbody.velocity.x;
        rigidbody.velocity = pushUpVector;
    }

    /// <summary>
    /// 每隔一段时间造成AOE伤害
    /// </summary>
    private void AoeAttack()
    {
        if (passTime > ATTACK_INTERVAL)
        {
            canFight.AttackArea(coll, damage);
            passTime = 0f;
        }
        passTime += Time.deltaTime;
    }

    protected override void Clear()
    {
        base.Clear();

        passTime = 0.0f;
    }
}
