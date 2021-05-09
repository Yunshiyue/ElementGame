using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(SlimerMovement))]
[RequireComponent(typeof(SlimerAttack))]
[RequireComponent(typeof(DefenceEnemies))]
public class Slimer : Enemies
{
    //private SlimerMovement movementComponent;
    private SlimerAttack attackComponent;

    public override void Initialize()
    {
        
        priorityInType = 2;

        //movementComponent = GetComponent<SlimerMovement>();
        //if (movementComponent == null)
        //{
        //    Debug.LogError("在Slimer中，没有找到Movement脚本！");
        //}
        attackComponent = GetComponent<SlimerAttack>();
        if (attackComponent == null)
        {
            Debug.LogError("在Slimer中，没有找到Attack脚本！");
        }

        defenceComponent = GetComponent<DefenceEnemies>();
        if (defenceComponent == null)
        {
            Debug.LogError("在Slimer中，没有找到Defence脚本！");
        }
        //设置最大生命值 
        defenceComponent.Initialize(20);

    }

    public override void MyUpdate()
    {
        DefenceCheck(); 
       // MoveControl();
    }

    //移动流程
    //private void MoveControl()
    //{
    //    movementComponent.RequestMoveByFrame(SlimerMovement.MovementMode.Normal);
    //}

    private void DefenceCheck()
    {
        defenceComponent.AttackCheck();
        if (defenceComponent.getIsDead())
        {
            gameObject.SetActive(false);
        }
        defenceComponent.Clear();
    }
    
    public override int GetPriorityInType()
    {
        return priorityInType;
    }

}
