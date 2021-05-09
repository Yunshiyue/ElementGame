using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Skill_3 : Action
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
        if (attackComponent.curSkillCD3 <= 0)
        {
            //重置CD
            attackComponent.curSkillCD3 = attackComponent.skillCD3;
            attackComponent.Skill3();
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
