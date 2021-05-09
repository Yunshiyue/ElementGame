using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsStun : Conditional
{

    private MovementEnemies movementComponent;
    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
        if (movementComponent.GetEnemyStatus()== MovementEnemies.EnemyStatus.Stun)
        {
            movementComponent.StunEvent();
            return TaskStatus.Running;
        }
        else
        {
            movementComponent.enemyAnim.SetBool("stunning", false);
            return TaskStatus.Failure;
        }
    }
}
