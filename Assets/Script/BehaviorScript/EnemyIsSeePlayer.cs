using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyIsSeePlayer : Conditional
{

    public MovementEnemies movementComponent;
    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
       
        if ((movementComponent.isSeePlayer))
        {
            return TaskStatus.Success;
        }
        else
        { 
            return TaskStatus.Failure;
        }
    }
}
