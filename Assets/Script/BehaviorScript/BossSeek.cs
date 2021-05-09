using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BossSeek : Action
{

    public MovementEnemies movementComponent;

    public override void OnStart()
    {
        base.OnStart();
        movementComponent = GetComponent<MovementEnemies>();
    
    }

    public override TaskStatus OnUpdate()
    {

        //执行seek
        movementComponent.Seek();

        return TaskStatus.Running;
    }
  
}
