using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class WitcherBlink : Action
{
    public MovementWitcher movementComponent;
    

    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementWitcher>();
    }
    public override TaskStatus OnUpdate()
    {
        movementComponent.IsTimeToBlink();
        return TaskStatus.Success;
    }

}
