using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Skill_4 : Action
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
        if (attackComponent.curSkillCD4 <= 0)
        {
            //重置CD
            attackComponent.curSkillCD4 = attackComponent.skillCD4;
            attackComponent.Skill4();
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
