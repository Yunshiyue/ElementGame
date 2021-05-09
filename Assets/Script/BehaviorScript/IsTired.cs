using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsTired : Conditional
{

    private MovementEnemies movementComponent;
    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
        if (movementComponent.isTired)
        {
            movementComponent.isUnStoppable = false;
            movementComponent.enemyAnim.SetBool("tired", true);
            return TaskStatus.Running;
        }
        else
        {
            movementComponent.isUnStoppable = true;
            movementComponent.enemyAnim.SetBool("tired", false);
            return TaskStatus.Failure;
        }
    }
}
