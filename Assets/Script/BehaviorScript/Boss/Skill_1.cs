using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Skill_1 : Action
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
        if (attackComponent.curSkillCD1 <= 0)
        {
            //重置CD
            attackComponent.curSkillCD1 = attackComponent.skillCD1;
            attackComponent.Skill1();
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
