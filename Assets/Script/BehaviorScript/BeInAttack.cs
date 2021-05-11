using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BeInAttack : Conditional
{
    private DefenceEnemies defenceComponent;
    private MovementEnemies movementComponent;
    //public SharedFloat lastAttackedTime;
    public override void OnStart()
    {
        base.OnStart();

        //slimerAnim = GetComponent<Animator>();
        defenceComponent = GetComponent<DefenceEnemies>();
        movementComponent = GetComponent<MovementEnemies>();

    }

    public override TaskStatus OnUpdate()
    {
        if (defenceComponent.getHpReduction() > 0 || movementComponent.isBeAttacked)
        {
            //Debug.Log("beAttacked!！");
            //lastAttackedTime = 0f;
            movementComponent.isBeAttacked = true;
            movementComponent.isInSeePlayerControl = false;
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}
