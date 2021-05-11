using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfExplosionAttack : SpellAttackEvent
{
   
    private Collider2D selfExplosinAttackArea;
    private int selfExplosionDamage = 10;
    private float selfExplosinInterruptTime = 1f;
    private float selfExplosinForce = 5f;

    public const float SELF_EXPLOSION__TIME = 0.65f;
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        selfExplosinAttackArea = GetComponent<Collider2D>();
       
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
 
    //攻击事件代码
    public void IceShieldAttackEvent()
    {
        
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(selfExplosinAttackArea, selfExplosionDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //击退效果 
                a.BeatBack(transform, selfExplosinInterruptTime, selfExplosinForce);
                //a.BeatBack(transform,iceShieldUseTime, iceShieldVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }

    }

}






