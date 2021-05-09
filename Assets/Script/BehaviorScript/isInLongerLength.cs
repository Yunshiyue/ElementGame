using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsInLongerLength : Conditional
{

    private MovementEnemies movementComponent;
    public float longerLength = 0f;
    private float length = 0f;
    // private AttackEnemies attackComponent;
    public override void OnStart()
    {
        base.OnStart();
        //attackComponent = GetComponent<AttackEnemies>();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
        length = Mathf.Abs(movementComponent.player.transform.position.x - transform.position.x);
        if (length>longerLength)
        {
            return TaskStatus.Success;
        }
        else
        {
            
            return TaskStatus.Failure;
        }
    }
}
