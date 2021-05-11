/**
 * @Description: 主角的防御组件，继承自Defence类。改写了伤害计算方法，每次伤害都会-护甲值1。
 *               目前没有作用。
 * @Author: ridger

 * 
 * 
 * @Description: 添加了HpPanelUI  覆盖了defence的Initialize
 * @Author: 夜里猛

 * 
 * @Description: 修正了HPanelUI改变函数的位置，调整至player
 * @Author: 夜里猛

 * 

 * @Editor: ridger
 * @Edit: 1. 增加了护盾相关的逻辑，
 *           public void ShieldUp(int localShieldPoint)方法用于打开护盾，同时通知动画组件打开水盾动画
 *           public void ShieldDown()方法用于关闭护盾，同时通知动画组件关闭水盾动画
 *           public bool IsSieldUp()用于获得当前是否施加护盾
 *        2. 重写了父类的SetHealthStatus方法，增加了如果有护盾情况下伤害逻辑的判定
 *        

 * @Editor: ridger
 * @Edit: 1.当主角死亡时场景重置，通知movement脚本主角死亡
 *        2.主角挨打时，进入无敌状态并通过改变不透明度来实现闪烁效果
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefencePlayer : Defence, ClassSaver
{
    private int armor = 1;
    private bool isShieldUp = false;
    private int shieldPoint = 0;
    private int recoveredHp = 0;
    private MovementPlayer movementComponent;

    //挨打无敌参数
    private float immuneTotalTime = 2f;
    private float immuneCurTime = 0f;
    private bool isAttackedImmune = false;

    private AttackAnime attackAnime;
    protected override void Awake()
    {
        base.Awake();
        movementComponent = GetComponent<MovementPlayer>();
        attackAnime = GetComponent<AttackAnime>();
    }

    public void ShieldUp(int localShieldPoint)
    {
        isShieldUp = true;
        shieldPoint = shieldPoint > localShieldPoint ? shieldPoint : localShieldPoint;
    }

    public void ShieldDown()
    {
        shieldPoint = 0;
        isShieldUp = false;
    }

    public bool IsSieldUp()
    {
        return isShieldUp;
    }

    public void Heal(int healPoint)
    {
        //hp += healPoint;
        //hp = hp > hpMax ? hpMax : hp;
        if ((hp+healPoint)> hpMax)
        {
            recoveredHp = hpMax - hp;
            hp = hpMax;
        }
        else
        {
            recoveredHp = healPoint;
            hp += healPoint;
        }
    }

    public override void Clear()
    {
        base.Clear();
        recoveredHp = 0;
    }

    //实现了抽象方法，在这个方法中包含你想通过这个组件做到的全部事情，在Player控制脚本中逐帧调用该方法
    public override void AttackCheck()
    {
        SetStatistic();
        AttackImmuneCheck();
        Damage();
        GetStatisticCollector();
    }

    private void GetStatisticCollector()
    {
        AttackContent[] attacks = attackedCheck.GetAttackedList();
        if(hp > 0)
        {
            for (int i = 0; i < attackNum; i++)
            {
                switch(attacks[i].who.tag)
                {
                    case "Slimer": StatisticsCollector.hitBySlimer++; break;
                    case "Witcher": StatisticsCollector.hitByWitch++; break;
                    case "Minotaur": StatisticsCollector.hitByMinotaur++; break;
                    //default: Debug.LogError(attacks[i].who.tag + " 没有录入统计信息中！"); break;
                }
            }
        }
        else
        {
            for (int i = 0; i < attackNum; i++)
            {
                switch (attacks[i].who.tag)
                {
                    case "Slimer": StatisticsCollector.deadBySlimer++; break;
                    case "Witcher": StatisticsCollector.deadByWitch++; break;
                    case "Minotaur": StatisticsCollector.deadByMinotaur++; break;
                    //default: Debug.LogError(attacks[i].who.tag + " 没有录入统计信息中！"); break;
                }
            }
        }
    }

    public override void Initialize(int hpMax)
    {
        base.Initialize(hpMax);
    }

    private void AttackImmuneCheck()
    {
        if(attackedCheck.hasBeenAttacked())
        {
            Debug.Log("挨打，进入无敌状态");
            isAttackedImmune = true;
            attackedCheck.SetImmune(true);
            //无敌闪烁动画
            attackAnime.StartAnime();
            return;
        }
        if(isAttackedImmune)
        {
            immuneCurTime += Time.deltaTime;
            if(immuneCurTime >= immuneTotalTime)
            {
                immuneCurTime = 0;
                isAttackedImmune = false;
                attackedCheck.SetImmune(false);
                //无敌闪烁动画结束
                attackAnime.EndAnime();
            }
        }
    }

    //改写父类的伤害计算方法，每次伤害都会减少1点护甲值的伤害。
    protected override void Damage()
    {
        if (hasSetStatistic)
        {   
            realDamage = damageSum - attackNum * armor;
            SetHealthStatus();
        }
        else
        {
            Debug.LogError("在" + gameObject.name + "中，setHealthStatus发生在了setStatistic之前");
        }

        if (realDamage > 0)
        {
            Debug.Log("player受到了" + realDamage + "点伤害");
        }
    }

    protected override void SetHealthStatus()
    {
        if(isShieldUp)
        {
            if (realDamage <= shieldPoint)
            {
                shieldPoint -= realDamage;
                hpReduction = 0;
            }
            else if (realDamage < hp + shieldPoint)
            {
                hp = hp + shieldPoint - realDamage;
                hpReduction = realDamage - shieldPoint;
                shieldPoint = 0;
            }
            else
            {
                hpReduction = hp;
                hp = 0;
                shieldPoint = 0;
            }
        }
        else
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
        }

        if (shieldPoint == 0)
        {
            ShieldDown();
        }
        if (hp == 0)
        {
            isDead = true;
            movementComponent.SetIsDead(true);
        }
    }

    public int getArmor() { return armor; }
    public int getRecoverdHp() { return recoveredHp; }

    public void LoadClass(string content)
    {
        ClassSaveHelper helper = new ClassSaveHelper();
        helper.LoadClassJsonString(content);
        helper.LoadValue(nameof(hp), out hp);
    }

    public string SaveClass()
    {
        ClassSaveHelper helper = new ClassSaveHelper();
        helper.SaveValue(nameof(hp), hp);
        return helper.GetJsonString();
    }

    public string GetID()
    {
        return nameof(DefencePlayer);
    }


}
