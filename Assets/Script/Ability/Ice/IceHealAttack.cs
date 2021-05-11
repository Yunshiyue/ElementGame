using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceHealAttack : SpellAttackEvent
{

    private Collider2D iceHealAttackArea;
    private int iceHealAttackDamage = 1;
    private float iceHealInterruptTime = 0.4f;
    private float iceHealForce = 1f;

    //public const float SELF_EXPLOSION__TIME = 0.65f;
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        iceHealAttackArea = GetComponent<Collider2D>();

    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    //攻击事件代码
    public void IceHealAttackEvent()
    {

        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(iceHealAttackArea, iceHealAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //击退效果 
                a.BeatBack(transform, iceHealInterruptTime, iceHealForce);
                //a.BeatBack(transform,iceShieldUseTime, iceShieldVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }

    }

}






