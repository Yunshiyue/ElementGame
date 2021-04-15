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
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefencePlayer : Defence
{
    private int armor = 1;
    private bool isShieldUp = false;
    private int shieldPoint = 0;

    private PlayerAnim playerAnim;

    public void ShieldUp(int localShieldPoint)
    {
        playerAnim.WaterShieldUp();
        isShieldUp = true;
        shieldPoint = shieldPoint > localShieldPoint ? shieldPoint : localShieldPoint;
    }

    public void ShieldDown()
    {
        playerAnim.WaterShieldDown();
        shieldPoint = 0;
        isShieldUp = false;
    }

    public bool IsSieldUp()
    {
        return isShieldUp;
    }

    public void Heal(int healPoint)
    {
        hp += healPoint;
        hp = hp > hpMax ? hpMax : hp;
    }

    //实现了抽象方法，在这个方法中包含你想通过这个组件做到的全部事情，在Player控制脚本中逐帧调用该方法
    public override void AttackCheck()
    {
        SetStatistic();
        Damage();
    }

    public override void Initialize(int hpMax)
    {
        base.Initialize(hpMax);

        playerAnim = GetComponent<PlayerAnim>();
        if (playerAnim == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到playerAnim组件！");
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
        }
    }

    public int getArmor() { return armor; }


    //protected override void Start()
    //{
    //    base.Start();

    //    Debug.Log("defenceplayer start")
    //    GameObject HpPanel = GameObject.Find("HP Panel");

    //    hpArray = new HPItem[hpMax];
    //    初始化心心数
    //    for (int i = 0; i < hp; i++)
    //    {
    //        Transform hpItem = HpPanel.transform.GetChild(i);
    //        hpArray[i] = hpItem.GetComponent<HPItem>();
    //        hpArray[i].Getting();
    //    }
    //}

}
