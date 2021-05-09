using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CountTime : Action
{

    public MovementEnemies movementComponent;

    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementWitcher>();
    }

    public override TaskStatus OnUpdate()
    {
        if (movementComponent.lastAttackedTime > 0 && movementComponent.lastAttackedTime < 3)
        {
            movementComponent.lastAttackedTime += Time.deltaTime;
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }
}
