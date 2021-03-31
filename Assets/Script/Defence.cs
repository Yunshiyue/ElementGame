/**
 * @Description: 
 * @Author: ridger
 * @Date: 2020-1-28 15:14
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

    //统计相关
    protected int attackNum = 0;
    protected int damageSum = 0;
    protected AttackInterruptType maxInterrupt = AttackInterruptType.NONE;
    protected bool hasSetStatistic = false;

    protected int realDamage = 0;
    protected int hpReduction = 0;


    public void Start()
    {
        attackedCheck = GetComponent<CanBeFighted>();
        if(attackedCheck == null)
        {
            Debug.LogError("在" + gameObject.name + "中，Defence组件没有找到所依赖的CanBeFighted组件");
        }
    }

    public void Initialize(int hpMax, int hp)
    {
        this.hpMax = hpMax;
        this.hp = hp;
    }

    abstract public void AttackCheck();

    public int getHpMax() { return hpMax; }
    public int getHp() { return hp; }
    public bool getIsDead() { return isDead; }
    public int getAttackNum() { return attackNum; }
    public int getDamageSum() { return damageSum; }
    public AttackInterruptType getMaxInterrupt() { return maxInterrupt; }
    public int getRealDamage() { return realDamage; }


    public void Clear()
    {
        hasSetStatistic = false;
        attackedCheck.Clear();
    }

    protected void SetStatistic()
    {
        hasSetStatistic = true;

        AttackInterruptType localMaxInterrupt = AttackInterruptType.NONE;
        int localDamageSum = 0;

        if(attackedCheck.hasBeenAttacked())
        {
            foreach (AttackContent attack in attackedCheck.GetAttackedList())
            {
                if ((int)localMaxInterrupt < (int)attack.interruptType)
                {
                    localMaxInterrupt = attack.interruptType;
                    localDamageSum += attack.damage;
                }
            }
        }

        maxInterrupt = localMaxInterrupt;
        damageSum = localDamageSum;
        attackNum = attackedCheck.getAttackNum();
    }

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
    
    protected void SetHealthStatus()
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
