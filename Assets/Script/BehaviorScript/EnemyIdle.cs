using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyIdle : Action
{
    public MovementEnemies movementComponent;

    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
        movementComponent.Idle();
        return TaskStatus.Running;
    }

}
