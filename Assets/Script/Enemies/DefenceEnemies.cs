using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceEnemies : Defence
{
    private GameObject bloodEffect;

    protected override void Awake()
    {
        base.Awake();
        bloodEffect = (GameObject)Resources.Load("Prefabs/Effect/BloodEffect");
    }
    public override void AttackCheck()
    {      
        SetStatistic();
        Damage();
        if (hpReduction > 0)
        {
            //掉血粒子特效
            Instantiate(bloodEffect, transform.position, Quaternion.identity);
        }
        //ChangeDebugInfo();
    }
}
