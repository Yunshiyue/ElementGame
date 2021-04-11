using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceEnemies : Defence
{
    public override void AttackCheck()
    {
        SetStatistic();
        Damage();
        //ChangeDebugInfo();
    }
}
