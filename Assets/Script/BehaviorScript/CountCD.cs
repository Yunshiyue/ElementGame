using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CountCD : Action
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
        if (movementComponent.curStatusCD > 0)
        {
            movementComponent.curStatusCD -= Time.deltaTime; 
        }

        if (attackComponent.curSkillCD1 > 0)
        {
            attackComponent.curSkillCD1 -= Time.deltaTime; 
        }

        if (attackComponent.curSkillCD2 > 0)
        {
            attackComponent.curSkillCD2 -= Time.deltaTime; 
        }

        if (attackComponent.curSkillCD3 > 0)
        {
            attackComponent.curSkillCD3 -= Time.deltaTime; 
        }

        if (attackComponent.curSkillCD4 > 0)
        {
            attackComponent.curSkillCD4 -= Time.deltaTime; 
        }
        return TaskStatus.Running;
    }
}
