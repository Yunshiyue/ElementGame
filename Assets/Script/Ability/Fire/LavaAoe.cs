/**
 * @Description: LavaAoe类为火山爆发造成的范围伤害类，即火+风
 * 
 * 初始化说明：无
 * 
 * @Author: CuteRed

 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaAoe : FlyingAbility
{
    public const string LAVA_AOE = "LavaAoe";

    [Header("伤害时间参数")]
    private const float ATTACK_INTERVAL = 0.5f;
    private float passTime = 0.0f;


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
        poolManager.RemoveGameObject(LAVA_AOE, gameObject);

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

    /// <summary>
    /// 每隔一段时间造成AOE伤害
    /// </summary>
    private void AoeAttack()
    {
        if (passTime > ATTACK_INTERVAL)
        {
            canFight.AttackArea(coll, damage, AttackInterruptType.NONE, ElementAbilityManager.Element.Fire);
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
