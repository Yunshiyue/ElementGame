using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class WitcherBlinkBack : Conditional
{
    public MovementWitcher movementComponent;

    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementWitcher>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!movementComponent.isInMoveArea && !movementComponent.isBeAttacked && !movementComponent.isInSeePlayerControl)//不在移动范围且不在被打状态、不在看见player流程内时闪现回原处
        {
            movementComponent.isTimeToBlinkBack = true;
            movementComponent.IsTimeToBlink();
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
        

}
