using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
//该seek只会在一段时间不被攻击后结束
public class AttackedSeek : Action
{
    public MovementEnemies movementComponent;
    private DefenceEnemies defenceComponent;
    private AttackEnemies attackComponent;
    public float attakedOffTime = 5f;
    //public SharedBool isBeAttacked; 
    //上次被打的时间
    
    public override void OnStart()
    {
        base.OnStart();
        defenceComponent = GetComponent<DefenceEnemies>();
        attackComponent = GetComponent<AttackEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
        //如果再次被打
        if (defenceComponent.getHpReduction() > 0 || attackComponent.isHitPlayer)
        {
            //上次被打时间重置
            movementComponent.lastAttackedTime = 0f;
            attackComponent.isHitPlayer = false;
        }
        //movementComponent.isBeAttacked = true;
        movementComponent.lastAttackedTime += Time.deltaTime;

        //如果上次被打后时间大于脱战时间
        if (movementComponent.lastAttackedTime > attakedOffTime)
        {
            movementComponent.isBeAttacked = false;
            //状态结束
            //Debug.Log("close");
            return TaskStatus.Failure;
        }

        //如果是有主动攻击的怪并且主角已在指定的攻击范围内 则success进入下一状态
        if (attackComponent.canActiveAttack && attackComponent.isInAttackLength )
        {
            return TaskStatus.Success;
        }

       
        //执行seek
        //Debug.Log("seek");
        movementComponent.Seek();

        return TaskStatus.Running;
    }
}
