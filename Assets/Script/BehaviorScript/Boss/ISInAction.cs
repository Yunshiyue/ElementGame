using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class isInAction : Conditional
{

    private AttackEnemies attackComponent;
    
    // private AttackEnemies attackComponent;
    public override void OnStart()
    {
        base.OnStart();
        attackComponent = GetComponent<AttackEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
        
        if (attackComponent.isInAction)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}
