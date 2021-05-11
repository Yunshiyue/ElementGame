using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MovementMinotaur))]
[RequireComponent(typeof(MinotaurAttack))]
[RequireComponent(typeof(DefenceEnemies))]
public class Minotaur : Enemies
{
    private MovementMinotaur movementComponent;
    private MinotaurAttack attackComponent;
    private CapsuleCollider2D coll;
    public List<Mechanism> mechanisms = new List<Mechanism>();

    public override void Initialize()
    {

        priorityInType = 2;
        coll = GetComponent<CapsuleCollider2D>();

        movementComponent = GetComponent<MovementMinotaur>();
        if (movementComponent == null)
        {
            Debug.LogError("在Witcher中，没有找到Movement脚本！");
        }
        attackComponent = GetComponent<MinotaurAttack>();
        if (attackComponent == null)
        {
            Debug.LogError("在Witcher中，没有找到Attack脚本！");
        }

        defenceComponent = GetComponent<DefenceEnemies>();
        if (defenceComponent == null)
        {
            Debug.LogError("在Witcher中，没有找到Defence脚本！");
        }
        //设置最大生命值
        defenceComponent.Initialize(100);
    }

    public override void MyUpdate()
    {
        DefenceCheck();
    }


    private void DefenceCheck()
    {
        defenceComponent.AttackCheck();
        if (defenceComponent.getIsDead())
        {
            movementComponent.enemyAnim.SetBool("isDie", true);
            //coll.enabled = false;
        }
        defenceComponent.Clear();
    }
    
    public void DieEvent()
    {
        foreach (Mechanism mechanism in mechanisms)
        {
            mechanism.Trigger(Mechanism.TiggerType.Other);
        }
        gameObject.SetActive(false);
    }
    

    public override int GetPriorityInType()
    {
        return priorityInType;
    }
}
