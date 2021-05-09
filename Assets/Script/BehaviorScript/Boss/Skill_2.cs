using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Skill_2 : Action
{

    public AttackEnemies attackComponent;
    public MovementEnemies movementComponent;

    public override void OnStart()
    {
        base.OnStart();
        attackComponent = GetComponent<AttackEnemies>();
        movementComponent = GetComponent<MovementEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
        if (attackComponent.curSkillCD2 <= 0)
        {
            //重置CD
            attackComponent.curSkillCD2 = attackComponent.skillCD2;
            attackComponent.Skill2();
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
