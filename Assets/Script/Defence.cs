/**
 * @Description: 防御组件的抽象父类，依赖CanBeFighted组件实现。提供血量、护甲等关于防御方面的结算方法和
 *               一系列针对受击队列的统计方法，诸如获得伤害值总和、找到最大打断类型等。
 *               通过public的get方法可以访问这些统计信息。
 *               需要在拥有此组件的控制组件的update函数上显示调用setStatistic和clear方法来获得数据/清理前一帧数据。
 * @Author: ridger

 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defence组件需要一个CanBeFighted组件，作为底层操作对象
[RequireComponent(typeof(CanBeFighted))]
abstract public class Defence : MonoBehaviour
{
    protected CanBeFighted attackedCheck;

    protected int hpMax = 5;
    protected int hp = 0;
    protected bool isDead = false;
    //保证该组件被初始化过，否则在调用方法时候报错
    private bool hasBeenInitialized = false;

    //统计相关
    protected bool hasSetStatistic = false;
    //受击次数
    protected int attackNum = 0;
    //毛伤害总和
    protected int damageSum = 0;
    //最大打断类型
    protected AttackInterruptType maxInterrupt = AttackInterruptType.NONE;
    //净伤害总和
    protected int realDamage = 0;
    //血量下降值
    protected int hpReduction = 0;

    protected virtual void Awake()
    {
        attackedCheck = GetComponent<CanBeFighted>();
        if (attackedCheck == null)
        {
            Debug.LogError("在" + gameObject.name + "中，Defence组件没有找到所依赖的CanBeFighted组件");
        }
    }
    public void SetImmune(bool isImmune)
    {
        attackedCheck.SetImmune(isImmune);
    }
    public virtual void Initialize(int hpMax)
    {
        hasBeenInitialized = true;
        this.hpMax = hpMax;
        this.hp = hpMax;
    }
    //子类中应该在这个方法中包含你想通过这个组件做到的全部事情，除了clear
    abstract public void AttackCheck();


    public int getHpMax() { return hpMax; }
    public int getHp() { return hp; }
    public bool getIsDead() { return isDead; }
    public int getAttackNum() { return attackNum; }
    public int getDamageSum() { return damageSum; }
    public AttackInterruptType getMaxInterrupt() { return maxInterrupt; }
    public int getRealDamage() { return realDamage; }
    public int getHpReduction() { return hpReduction; }

    //将下一帧的hasSetStatistic设置为false，并清除CanBeFighted的数据
    public virtual void Clear()
    {
        hasSetStatistic = false;
        attackedCheck.Clear();
    }
    //统计CanBeFighted中的信息，包括统计相关、受击次数、毛伤害总和、最大打断类型
    protected virtual void SetStatistic()
    {
        hasSetStatistic = true;
        if(!hasBeenInitialized)
        {
            Debug.LogError("在Defence中，没有初始化该组件！");
        }

        AttackInterruptType localMaxInterrupt = AttackInterruptType.NONE;
        int localDamageSum = 0;

        if(attackedCheck.hasBeenAttacked())
        {
            foreach (AttackContent attack in attackedCheck.GetAttackedList())
            {
                if ((int)localMaxInterrupt < (int)attack.interruptType)
                {
                    localMaxInterrupt = attack.interruptType;
                }
                localDamageSum += attack.damage;
            }
           
        }

        maxInterrupt = localMaxInterrupt;
        damageSum = localDamageSum;
        attackNum = attackedCheck.getAttackNum();
    }
    //定义了伤害计算方法，通过计算出的realDamage，SetHealthStatus方法对生命值进行扣除，并计算出hpReduction
    virtual protected void Damage()
    {
        if(hasSetStatistic)
        {
            realDamage = damageSum;
            SetHealthStatus();
        }
        else
        {
            Debug.LogError("在" + gameObject.name + "中，setHealthStatus发生在了setStatistic之前");
        }
    }
    //对生命值进行扣除，并计算出hpReduction；如果生命值为0，则isDead设置为true
    protected virtual void SetHealthStatus()
    {
        
        if (hp < realDamage)
        {
            hpReduction = hp;
            hp = 0;
        }
        else
        {
            hpReduction = realDamage;
            hp -= realDamage;
        }

        if (hp == 0)
        {
            isDead = true;
        }
    }

}
