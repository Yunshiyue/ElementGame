using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(FlyingEyeMovement))]
[RequireComponent(typeof(FlyingEyeAttack))]
[RequireComponent(typeof(DefenceEnemies))]
public class FlyingEye : Enemies
{
    //private FlyingEyeMovement movementComponent;
    private FlyingEyeAttack attackComponent;

    public override void Initialize()
    {
        
        priorityInType = 2;

        //movementComponent = GetComponent<FlyingEyeMovement>();
        //if (movementComponent == null)
        //{
        //    Debug.LogError("在FlyingEye中，没有找到Movement脚本！");
        //}
        attackComponent = GetComponent<FlyingEyeAttack>();
        if (attackComponent == null)
        {
            Debug.LogError("在FlyingEye中，没有找到Attack脚本！");
        }

        defenceComponent = GetComponent<DefenceEnemies>();
        if (defenceComponent == null)
        {
            Debug.LogError("在FlyingEye中，没有找到Defence脚本！");
        }
        //设置最大生命值 
        defenceComponent.Initialize(3);

    }

    public override void MyUpdate()
    {
        DefenceCheck(); 
       // MoveControl();
    }

    //移动流程
    //private void MoveControl()
    //{
    //    movementComponent.RequestMoveByFrame(FlyingEyeMovement.MovementMode.Normal);
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
