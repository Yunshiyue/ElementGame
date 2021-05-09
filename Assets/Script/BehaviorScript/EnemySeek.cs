using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
//该Seek会因为一段时间检测不到player或超出移动范围后结束
public class EnemySeek : Action
{
    //private Rigidbody2D rb;
    public MovementEnemies movementComponent;
    private DefenceEnemies defenceComponent;
    private AttackEnemies attackComponent;
    public float continueSeekTime=2f;
    public SharedFloat lastAttackedTime = 0f;
    //public SharedBool islostTargetButFind = false;

    public float lostTargetTime = 0f;
    public override void OnStart()
    {
        base.OnStart();
        defenceComponent = GetComponent<DefenceEnemies>();
        attackComponent = GetComponent<AttackEnemies>();
    }

    public override TaskStatus OnUpdate()
    {
       
        //如果是有主动攻击的怪并且主角已在指定的攻击范围内,并且不在遗失目标但在寻找状态 则success进入下一状态(攻击）
        if (attackComponent.canActiveAttack && attackComponent.isInAttackLength && !movementComponent.islostTargetButFind )
        {
            return TaskStatus.Success;
        }

        //追踪逻辑
        clear();
        if (defenceComponent.getHpReduction() > 0)
        {
            movementComponent.isBeAttacked = true;
        }
        if (!movementComponent.isInMoveArea || movementComponent.isBeAttacked) //如果 超出移动范围||被攻击
        {
            //Debug.Log("enemyseek返回falseisin:" + movementComponent.isInMoveArea);
            lostTargetTime = 0f;
            movementComponent.islostTargetButFind = false;
            movementComponent.isInSeePlayerControl = false;
            return TaskStatus.Failure;
        }

        if (!movementComponent.isSeePlayer)//如果看不到主角continueSeekTime后，则进入下一行为
        {
            movementComponent.islostTargetButFind = true;
            lostTargetTime += Time.deltaTime;
            //Debug.Log("lostTargetTime:" + lostTargetTime);
            if (lostTargetTime > continueSeekTime)
            {
                lostTargetTime = 0f;
                movementComponent.islostTargetButFind = false;
                movementComponent.isInSeePlayerControl = false;
                return TaskStatus.Failure;
            }
        }
        else//看到主角则把目标消失时间清零
        {
            movementComponent.islostTargetButFind = false;
            lostTargetTime = 0f;
        }

        //执行seek
        movementComponent.Seek();

        return TaskStatus.Running;
    }
    private void clear()
    {
        if (lastAttackedTime.Value > 0) lastAttackedTime = 0f;
    }
}
