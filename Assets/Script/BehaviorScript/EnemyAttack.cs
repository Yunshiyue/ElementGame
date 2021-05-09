using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyAttack : Action
{
    public AttackEnemies attackComponent;
    

    public override void OnStart()
    {
        base.OnStart();
        attackComponent = GetComponent<AttackEnemies>();
    }
    public override TaskStatus OnUpdate()
    {
        //movementComponent.NormalMove();
        attackComponent.AttackControl();
        return TaskStatus.Success;
    }

}
