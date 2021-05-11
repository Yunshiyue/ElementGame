/**
 * @Description: CanFight类是所有能够进行“攻击”的单位所拥有的组件。提供范围攻击和召唤飞行道具的组件。
 * @Author: ridger

 * 

 * @Editor: ridger
 * @Edit: 1. CanFight类不再具有“父类”的语义，而是作为一个可以被添加的组件添加到可以进行攻击的单位上。
 *           这样做更加切合unity中单位-组件的设计模式，有攻击需求的单位只需要添加该脚本即可。
 *        2. 同时，CanFight不再具有“CanBeFighted”的语义，因为考虑到某些地图交互元素可能只有攻击模块(如机关、地刺等)，
 *           某些地图元素可能只有被攻击的效果(如宝箱、开启式机关等)。所以将两种不同语义的模块分开设计。
 *

 * @Editor: ridger
 * @Edit: 1. 在控制脚本使用该组件的时候需要显式调用Initialize来初始化filter，确定哪些Layer是我们想要攻击的
 * 
 * 

 * @Editor: ridger
 * @Edit: 1. 范围攻击函数返回值，修改为返回攻击到的敌人对象的CanBeFighted组件的数组
 *           
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanFight : MonoBehaviour
{
    /// <summary>
    /// CanFight类中范围攻击最多一次可以攻击多少个目标
    /// </summary>
    public const int ENMEIES_MAX_NUM_ONEATTACK = 32;
    /// <summary>
    /// 检测敌人碰撞体的筛选器，作为Collider2D.OverlapLayer()的第一个参数。具体作用见Unity API。
    /// </summary>
    private ContactFilter2D filter = new ContactFilter2D();

    private bool isInitialized = false;

    /// <summary>
    /// 构造函数，初始化筛选器的配置
    /// </summary>
    public void Initiailize(string[] layerNames)
    {
        filter.useNormalAngle = false;
        filter.useDepth = false;
        filter.useOutsideDepth = false;
        filter.useOutsideNormalAngle = false;
        filter.useTriggers = false;

        filter.useLayerMask = true;

        LayerMask targetLayer = 0;
        foreach(string layername in layerNames)
        {
            targetLayer ^= 1 << LayerMask.NameToLayer(layername);
        }
        //Debug.Log("在" + gameObject.name + "中，可攻击到的层为" + System.Convert.ToString(targetLayer,2));

        filter.layerMask = targetLayer;
        //32个bit表示32个层，左移表示筛选需要哪个层

        isInitialized = true;
    }

    /// <summary>
    /// 对一个能够战斗的目标造成伤害，作为底层私有函数被调用
    /// </summary>
    /// <param name="target">造成伤害的目标</param>
    /// <param name="damage">造成输入数值的伤害</param>
    /// <param name="interruptType">攻击打断类型，默认为无打断</param>
    /// <returns>返回造成了多少伤害，具体用法有待进一步讨论</returns>
    public int Attack(CanBeFighted target, int damage, AttackInterruptType interruptType = AttackInterruptType.NONE, ElementAbilityManager.Element element = ElementAbilityManager.Element.NULL)
    {
        if(!isInitialized)
        {
            Debug.LogError("在" + gameObject.name + "物体中，CanFight组件未初始化！");
        }
        return target.BeAttacked(gameObject, damage, interruptType, element);
    }

    /// <summary>
    /// 范围性攻击，实现方法为输入表示范围的Collier2D，检测范围内的每个拥有CanBeFighted的敌人，调用BeAttacked
    /// </summary>
    /// <param name="area">表示攻击范围的collier2d, 应该为trigger态</param>
    /// <param name="damage">该次范围攻击造成了多少伤害</param>
    /// <param name="interruptType">该次攻击为何种打断类型</param>
    /// <returns>返回攻击到的敌人对象的CanBeFighted组件的数组</returns>
    public CanBeFighted[] AttackArea(Collider2D area, int damage, AttackInterruptType interruptType = AttackInterruptType.NONE, ElementAbilityManager.Element element = ElementAbilityManager.Element.NULL)
    {
        //输入范围需要Trigger才行
        if(!area.isTrigger)
        {
            Debug.LogError("在" + gameObject.name + "释放范围攻击时,输入的collider2d并不是trigger态");
            return null;
        }

        Collider2D[] enemies = new Collider2D[ENMEIES_MAX_NUM_ONEATTACK];
        int enemiesNumber = area.OverlapCollider(filter, enemies);

        if(enemiesNumber != 0)
        {
            Debug.Log("攻击碰到敌人");

            CanBeFighted[] enemiesAttacked = new CanBeFighted[enemiesNumber];
            CanBeFighted enemyBody;
            //对碰到的敌人进行以下操作，如果敌人有CanBeFighted组件，则施加攻击，否则报错

            for(int i = 0; i < enemiesNumber; i++)
            {

                if (enemies[i].TryGetComponent<CanBeFighted>(out enemyBody))
                {
                    Attack(enemyBody, damage, AttackInterruptType.NONE, element);
                    enemiesAttacked[i] = enemyBody;
                }
                else
                {
                    Debug.LogError("在" + gameObject.name +
                        "释放范围攻击时,这些物体被检测为敌人，但是没有CanBeFighted组件" + enemies[i].gameObject.name);
                }
            }
            return enemiesAttacked;
        }

        return null;
    }

    public CanBeFighted[] AttackArea(Collider2D area, int damage, CanBeFighted[] hasAttacked, AttackInterruptType interruptType = AttackInterruptType.NONE, ElementAbilityManager.Element element = ElementAbilityManager.Element.NULL)
    {
        //输入范围需要Trigger才行
        if (!area.isTrigger)
        {
            Debug.LogError("在" + gameObject.name + "释放范围攻击时,输入的collider2d并不是trigger态");
            return null;
        }

        Collider2D[] enemies = new Collider2D[ENMEIES_MAX_NUM_ONEATTACK];
        int enemiesNumber = area.OverlapCollider(filter, enemies);

        if (enemiesNumber != 0)
        {
            Debug.Log("攻击碰到敌人");

            CanBeFighted[] enemiesAttacked = new CanBeFighted[enemiesNumber];
            CanBeFighted enemyBody;
            //对碰到的敌人进行以下操作，如果敌人有CanBeFighted组件，则施加攻击，否则报错

            for (int i = 0; i < enemiesNumber; i++)
            {
                if (enemies[i].TryGetComponent<CanBeFighted>(out enemyBody))
                {
                    Attack(enemyBody, damage, AttackInterruptType.NONE, element);
                    enemiesAttacked[i] = enemyBody;
                }
                else
                {
                    Debug.LogError("在" + gameObject.name +
                        "释放范围攻击时,这些物体被检测为敌人，但是没有CanBeFighted组件" + enemies[i].gameObject.name);
                }
            }
            return enemiesAttacked;
        }

        return null;
    }

    /// <summary>
    /// 生成飞行道具攻击，实现方法为生成某个含有脚本的攻击物体
    /// </summary>
    /// TODO
    public void AttackGenarate()
    {

    }
}
