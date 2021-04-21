using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceEnemies : Defence
{
    private GameObject bloodEffect;

    private void Awake()
    {
        base.Awake();
        bloodEffect = (GameObject)Resources.Load("Prefabs/BloodEffect");
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
