using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefencePlayer : Defence
{
    public int armor = 1;

    public override void AttackCheck()
    {
        SetStatistic();
        Damage();
    }

    public int getArmor() { return armor; }

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
    }
}
