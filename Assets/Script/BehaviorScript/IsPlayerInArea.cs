using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsPlayerInArea : Conditional
{

    private MovementEnemies movementComponent;
    // private AttackEnemies attackComponent;
    public override void OnStart()
    {
        base.OnStart();
        //attackComponent = GetComponent<AttackEnemies>();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {

        if (movementComponent.isPlayerInArea)
        {
            return TaskStatus.Success;
        }
        else
        {
            
            return TaskStatus.Failure;
        }
    }
}
