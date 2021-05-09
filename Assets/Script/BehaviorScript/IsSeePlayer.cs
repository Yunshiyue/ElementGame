using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsSeePlayer : Conditional
{

    public MovementEnemies movementComponent;
   // private AttackEnemies attackComponent;
    public override void OnStart()
    {
        base.OnStart();
        //attackComponent = GetComponent<AttackEnemies>();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
       
        if ((movementComponent.isSeePlayer && !movementComponent.isBeAttacked)|| movementComponent.islostTargetButFind)
        {
            //见到player的流程设为true
            movementComponent.isInSeePlayerControl = true;
            return TaskStatus.Success;
        }
        else
        {
            movementComponent.isInSeePlayerControl = false;
            return TaskStatus.Failure;
        }
    }
}
