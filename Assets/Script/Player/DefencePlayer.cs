/**
 * @Description: 主角的防御组件，继承自Defence类。改写了伤害计算方法，每次伤害都会-护甲值1。
 *               目前没有作用。
 * @Author: ridger

 * 
 * 
 * @Description: 添加了HpPanelUI  覆盖了defence的Initialize
 * @Author: 夜里猛

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefencePlayer : Defence
{
    public int armor = 1;

    private HPItem[] hpArray;
    //实现了抽象方法，在这个方法中包含你想通过这个组件做到的全部事情，在Player控制脚本中逐帧调用该方法

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

    public override void AttackCheck()
    {
        SetStatistic();
        Damage();
    }

    public override void Initialize(int hpMax)
    {
        base.Initialize(hpMax);
        GameObject HpPanel = GameObject.Find("HP Panel");

        hpArray = new HPItem[hpMax];
        //初始化心心数
        for (int i = 0; i < hp; i++)
        {
            Transform hpItem = HpPanel.transform.GetChild(i);
            hpArray[i] = hpItem.GetComponent<HPItem>();
            hpArray[i].Getting();
        }
    }


    //改写父类的伤害计算方法，每次伤害都会减少1点护甲值的伤害。
    protected override void Damage()
    {
        if (hasSetStatistic)
        {   
            realDamage = damageSum - attackNum * armor;
            SetHealthStatus();
            ChangeHpUI();
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

    public int getArmor() { return armor; }

    public void ChangeHpUI()
    {
        for(int i =0; i < realDamage; i++)
        {
            int index = hp - i ;
            Debug.Log("HP:" + hp + ";index:" + index);
            //Transform hpItem = HpPanel.transform.GetChild(index);//找到指定的心心
            //hpItem.GetComponent<HPItem>().Lost();
            hpArray[index].Lost();
        }
    }

    
     
}
