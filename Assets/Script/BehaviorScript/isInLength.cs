using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsInLength : Conditional
{

    private MovementEnemies movementComponent;
    public float shorterLength = 0f;
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
        //length =Mathf.Abs(movementComponent.player.transform.position.x - transform.position.x);
        length = (movementComponent.player.transform.position - transform.position).sqrMagnitude;
        if (length>shorterLength && length< longerLength)
        {
            return TaskStatus.Success;
        }
        else
        {
            
            return TaskStatus.Failure;
        }
    }
}
