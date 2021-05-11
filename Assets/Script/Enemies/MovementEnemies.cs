using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class MovementEnemies : myUpdate
{
    protected AttackEnemies attackComponent;
    protected Rigidbody2D rb;
    public Animator enemyAnim;//动画

    public float originx, leftx, rightx;
    public Vector3 originTransform;

    //移动参数
    public float speed = 0f;
    public float seekSpeed = 0f;//追击速度
    public float jumpForce = 0f;
    //记录初始速度
    protected float speedChangeTime = 0f;
    protected float originSpeed = 0f;
    protected float originSeekSpeed = 0f;
    protected float originJumpForce = 0f;

    protected Vector2 toRightVelocity;
    protected Vector2 toLeftVelocity;
    protected Vector3 rightScale = new Vector3(1, 1, 1);
    protected Vector3 leftScale = new Vector3(-1, 1, 1);

    //移动组件中包含检测
    protected LayerMask playerLayer;
    protected LayerMask groundLayer;
    public bool isSeePlayer = false;
    public GameObject player;
   
    //是否在移动范围内
    public bool isInMoveArea = true;
    //player是否在范围内
    public bool isPlayerInArea = false;
    //检测是否在地面上  
    public bool isOnGround = true;
    //是否朝右
    public bool faceRight;

    //用于boss 无法被眩晕
    public bool isUnStoppable = false;
    
    //用于行为树
    public bool isBeAttacked=false;//是否处于被攻击流程
    public bool isTired=false;//是否处于疲惫
    public bool islostTargetButFind = false;//是否处于失去攻击目标但仍在寻找状态
    public bool isInSeePlayerControl = false;//是否处于看见player的流程
    public float lastAttackedTime = 0f;

    //状态CD
    public float statusCD = 0f;
    public float curStatusCD = 0f;

    private Vector2 noVelocity = new Vector2(0, 0);

    private UpdateType updateType = UpdateType.Enemy;
    //当前怪物处于何种状态：普通，技能释放，被控制
    public enum EnemyStatus { Normal, AbilityWithMovement, AbilityWithNoMovement, Stun, Encompass }

    //当前怪物是否处于非正常状态，指主角是否处于释放技能状态或者被控制的状态，根据这个bool值判断某一帧是否需要计时器++
    protected EnemyStatus currentStatus = EnemyStatus.Normal;

    public enum MovementMode { Normal, Ability, Attacked }

    override public void MyUpdate()
    {
        //如果当前状态不是Normal，则时间++，
        if (isInAbnormalStatus)
        {
            //状态时间++
            controlStatusCurTime += Time.deltaTime;
            
            //是否处于眩晕禁锢叠加态
            if (isStunAndEncompassStatus)
            {
                stunAndEncompassStatusCurTime += Time.deltaTime;
                if (stunAndEncompassStatusCurTime > stunAndEncompassStatusTotalTime)
                {
                    isStunAndEncompassStatus = false;
                }
            }
            //是否处于技能&禁锢叠加态
            if (isAbilityAndEncompassStatus)
            {
                abilityAndEncompassStatusCurTime += Time.deltaTime;
                if (abilityAndEncompassStatusCurTime > abilityAndEncompassStatusTotalTime)
                {
                    isAbilityAndEncompassStatus = false;
                }
            }

            //状态到期发生的事情
            if (controlStatusCurTime >= controlStatusTotalTime)
            {
                ChangeControlStatus(0f, EnemyStatus.Normal);

                if (isStunAndEncompassStatus)//如果眩晕结束后仍处于禁锢眩晕叠加态
                {
                    ChangeControlStatus(stunAndEncompassStatusTotalTime-stunAndEncompassStatusCurTime, EnemyStatus.Encompass);
                    isStunAndEncompassStatus = false;//眩晕结束，叠加态消失
                }else if (isAbilityAndEncompassStatus)
                {
                    ChangeControlStatus(abilityAndEncompassStatusTotalTime - abilityAndEncompassStatusCurTime, EnemyStatus.AbilityWithNoMovement);
                    isAbilityAndEncompassStatus = false;//禁锢结束，叠加态消失
                }
            }
        }
        //在处理正常状态
        else
        {
            //正常状态下需进行人物检测和物理检测
            PlayerCheck();
            PhysicsCheck();
        }
        //处理速度变化
        //if (speedChangeTime>0)
        //{
        //    speedChangeTime -= Time.deltaTime;
        //}
        //else
        //{
        //    speedChangeTime = 0;
        //    RecoverSpeed();
        //}
       
        Clear();
    }
    public override void Initialize()
    {
        playerLayer = LayerMask.GetMask("Player");
        groundLayer = LayerMask.GetMask("Platform");
        player = GameObject.Find("Player");
        attackComponent = GetComponent<AttackEnemies>();
    }
    public bool RequestChangeControlStatus(float statusTime, EnemyStatus status)
    {
        if(isUnStoppable  && (status == EnemyStatus.Stun || status == EnemyStatus.Encompass))
        {
            return false;
        }

        switch (currentStatus)
        {
            case EnemyStatus.Normal:
                ChangeControlStatus(statusTime, status);
                return true;


            case EnemyStatus.AbilityWithNoMovement:
            case EnemyStatus.AbilityWithMovement:
                if (status == EnemyStatus.Normal ||
                    status == EnemyStatus.Encompass||
                   status == EnemyStatus.Stun)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;

            //暂时设定眩晕状态下不允许被打断，眩晕状态下眩晕允许且时间刷新
            case EnemyStatus.Stun:
                if (status == EnemyStatus.Normal ||
                   status == EnemyStatus.Stun)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }else if(status == EnemyStatus.Encompass)
                {
                    //因为stun会覆盖Encompass，禁锢状态申请不会改变其状态，但启用了眩晕&禁锢计时器，记录眩晕和禁锢叠加状态 ,当stun状态结束后，如果还处于眩晕禁锢叠加状态，组件会为其改为禁锢状态
                    isStunAndEncompassStatus = true;
                    isAbilityAndEncompassStatus = false;//如果进入了眩晕禁锢叠加状态,那么技能&禁锢的状态则会被取消
                    stunAndEncompassStatusTotalTime = statusTime;
                    stunAndEncompassStatusCurTime = 0f;
                    return true;
                }
                return false;

            case EnemyStatus.Encompass:
                if(status==EnemyStatus.Normal||
                    status==EnemyStatus.Encompass||
                    status==EnemyStatus.Stun)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }else if(status == EnemyStatus.AbilityWithNoMovement)
                {
                    //禁锢状态下没位移的技能（攻击）能够使用，但不会改变其禁锢状态,原理同眩晕和禁锢叠加状态，当禁锢状态结束后，如果还处于技能&禁锢叠加状态，组件会为其改为技能状态
                    isAbilityAndEncompassStatus = true;
                    abilityAndEncompassStatusTotalTime = statusTime;
                    abilityAndEncompassStatusCurTime = 0f;
                    return true;
                }
                return false;
        }

        return false;
    }
    //内部逻辑部分

    public virtual bool RequestMoveByTime(Vector2 movement, float time, MovementMode mode)
    {
        switch (mode)
        {
            case MovementMode.Normal:
                return false;

            case MovementMode.Ability:
                if (canAbilityMovement)
                {
                    isAbilityMovement = true;
                    if (!isFacingLeft)
                    {
                        abilityMovement = movement;
                        abilityMovementSpeed = movement / time;
                    }
                    else
                    {
                        abilityMovement = -movement;
                        abilityMovementSpeed = -movement / time;
                    }
                    abilityMovementTotalTime = time;
                    abilityMovementCurTime = 0f;
                    return true;
                }
                return false;

            case MovementMode.Attacked:
                if (canPassiveMovement)
                {
                    //被击移动
                    rb.AddForce(movement, ForceMode2D.Impulse);
                    isPassiveMovement = true;
                    
                    return true;
                }
                return false;
        }
        return false;
    }

    //请求在这一帧中进行position的跳跃，movement为相对位移改变量
    //同理，技能控制会根据当前主角朝向改变方向，而被动传送则不会
    public virtual bool RequestMoveByFrame(MovementMode mode)
    {
        Vector2 movement = new Vector2(1, 0);//
        switch (mode)
        {
            case MovementMode.Normal:
                //普通移动
                if (canControllorMovement)
                {
                    isControllorMovement = true;
                    
                    return true;
                }
                return false;

            case MovementMode.Ability:
                if (canActiveTransport)
                {
                    //实现闪现

                    return true;
                }
                return false;

            case MovementMode.Attacked:
                if (canPassiveTransport)
                {
                    isPassiveTransport = true;
                    passiveTransportPosition = movement;
                    return true;
                }
                return false;
        }

        return false;
    }
    //与状态控制相关的整个类所用到的变量


    protected bool isInAbnormalStatus = false;
    //当前状态的计时器，Normal态和Crouch态不需要计时器
    protected float controlStatusTotalTime = 0f;
    protected float controlStatusCurTime = 0f;
    
    //眩晕禁锢状态的额外计时器
    protected float stunAndEncompassStatusTotalTime = 0f;
    protected float stunAndEncompassStatusCurTime = 0f;
    protected bool isStunAndEncompassStatus = false;

    //攻击及禁锢的额外计时器
    protected float abilityAndEncompassStatusTotalTime = 0f;
    protected float abilityAndEncompassStatusCurTime = 0f;
    protected bool isAbilityAndEncompassStatus = false;

    protected bool isFacingLeft = false;

    //主角是否处于下坠状态，当主角在空中受到interrupt类型的攻击时，该状态位为true；当处于该状态且着地时，该状态为恢复为false
    protected bool isAttackedFalling = false;


    //清除该帧统计信息
    protected void Clear()
    {
        isPassiveTransport = false;
        isActiveTransport = false;
        isControllorMovement = false;
    }
    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
    //记录当前帧的运动记录参数

    //传送统一使用local坐标系
    //被动传送 = 被动以帧为结算的位移
    protected Vector2 passiveTransportPosition;
    protected bool canPassiveTransport = true;
    protected bool isPassiveTransport = false;

    //主动传送
    protected Vector2 activeTransportPosition;
    protected bool canActiveTransport = true;
    protected bool isActiveTransport = false;

    //技能位移
    protected Vector2 abilityMovement;
    protected float abilityMovementTotalTime = 0f;
    protected Vector2 abilityMovementSpeed = new Vector2(0, 0);
    protected float abilityMovementCurTime = 0f;
    protected bool canAbilityMovement = true;
    protected bool isAbilityMovement = false;

    //被动位移
    protected Vector2 passiveMovement;
    protected Vector2 passiveMovementSpeed = new Vector2(0, 0);
    protected float passiveMovementTotalTime = 0f;
    protected float passiveMovementCurTime = 0f;
    protected bool canPassiveMovement = true;
    protected bool isPassiveMovement = false;

    //控制位移，以帧结算
    protected Vector2 controllorMovement = new Vector2(0, 0);
    protected bool isControllorMovement = false;
    protected bool canControllorMovement = true;

    RaycastHit2D leftCheck, rightCheck;
    protected Vector2 leftV ;
    protected Vector2 rightV;
    public virtual void PhysicsCheck()
    {
        //地板检测
        leftCheck = Raycast(leftV, Vector2.down, 0.05f, groundLayer);
        rightCheck = Raycast(rightV, Vector2.down, 0.05f, groundLayer);
        if (leftCheck || rightCheck)
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }
    }
    public virtual void PlayerCheck()
    {

    }
    public virtual void Idle()
    {
        //isInAction = false;
    }
    float absLength = 0;
    public virtual void Seek()
    {
        absLength = Mathf.Abs(transform.position.x - player.transform.position.x);
        if (absLength>1.5f && RequestMoveByFrame(MovementMode.Normal))//申请是否可移动
        {
            if (isOnGround)
            {
                if (transform.position.x > player.transform.position.x)
                {
                    transform.localScale = leftScale;
                    faceRight = false;
                }
                else
                {
                    transform.localScale = rightScale;
                    faceRight = true;
                }
                if (faceRight)
                {
                    toRightVelocity.x = seekSpeed;
                    toRightVelocity.y = rb.velocity.y;
                    rb.velocity = toRightVelocity;
                }
                else
                {
                    toLeftVelocity.x = -seekSpeed;
                    toLeftVelocity.y = rb.velocity.y;
                    rb.velocity = toLeftVelocity;
                }
            }
        }
    }
    public virtual void NormalMove()
    {
        if (RequestMoveByFrame(MovementMode.Normal))//申请是否可移动
        {
            if (isOnGround)
            {
                if (faceRight)
                {
                    toRightVelocity.x = speed;
                    toRightVelocity.y = rb.velocity.y;
                    rb.velocity = toRightVelocity;
                    if (transform.position.x > rightx)
                    {
                        transform.localScale = leftScale;
                        faceRight = false;
                    }
                }
                else
                {
                    toLeftVelocity.x = -speed;
                    toLeftVelocity.y = rb.velocity.y;
                    rb.velocity = toLeftVelocity;
                    if (transform.position.x < leftx)
                    {
                        transform.localScale = rightScale;
                        faceRight = true;
                    }
                }
            }
        }
    }

    public virtual void Dash()
    {

    }
    public virtual void StunEvent()
    {
        isInSeePlayerControl = false;
        isBeAttacked = true;
        enemyAnim.SetBool("stunning", true);
    }

    //修复使用技能后的朝向问题
    public void RecoverScale()
    {
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = rightScale;
            faceRight = true;
        }
        else
        {
            transform.localScale = leftScale;
            faceRight = false;
        }
    }

    //修改当前状态的私有方法；在修改当前状态的同时，把Can系列的状态为设置，保证在某些状态下屏蔽掉低优先级的请求
    //(比如在眩晕的时候请求释放技能)；同时，将低优先级的计时器状态设置为false(is系列状态位)，保证进行高优先级计时器是
    //低优先级计时器不在工作(比如0.5s眩晕状态会终止1s的技能释放状态，否则0.5s眩晕结束后，技能没有被打断，仍然处在技能移动状态下)
    protected void ChangeControlStatus(float statusTime, EnemyStatus status)
    {
        //如果是需要计时的状态，则重置计时器
        if (status != EnemyStatus.Normal)
        {
            isInAbnormalStatus = true;
            controlStatusTotalTime = statusTime;
            controlStatusCurTime = 0f;
        }
        //否则不需要重置计时器
        else
        {
            isInAbnormalStatus = false;
            controlStatusTotalTime = 0f;
            controlStatusCurTime = 0f;
        }
        currentStatus = status;

        switch (status)
        {
            case EnemyStatus.Normal:
                //enemyAnim.SetBool("stunning", false);
                canPassiveTransport = true;
                canActiveTransport = true;
                canAbilityMovement = true;
               
                canControllorMovement = true;

                isAbilityMovement = false;
                isPassiveMovement = false;

                isInAbnormalStatus = false;
                break;

            case EnemyStatus.AbilityWithMovement:
                canPassiveMovement = true;
                canActiveTransport = true;
                canAbilityMovement = true;
              
                canControllorMovement = false;

                isPassiveMovement = false;
                isControllorMovement = false;

                isInAbnormalStatus = true;
                break;
            case EnemyStatus.AbilityWithNoMovement:
                canPassiveMovement = true;
                canActiveTransport = true;
                canAbilityMovement = true;
               
                canControllorMovement = true;

                isPassiveMovement = false;

                isInAbnormalStatus = true;
                break;

            case EnemyStatus.Stun:
                //enemyAnim.SetBool("stunning",true);
                canPassiveMovement = true;
                canActiveTransport = false;
                canAbilityMovement = false;
              
                canControllorMovement = false;

                isAbilityMovement = false;
                isPassiveMovement = false;
                isControllorMovement = false;

                isInAbnormalStatus = true;
                break;

            case EnemyStatus.Encompass://禁锢 与stun区别 能攻击但不能移动,此处的移动权限与stun一致 但在RequestChangeControlStatus方法中，没位移的技能状态申请EnemyStatus.AbilityWithNoMovement能被成功返回为true
                canPassiveMovement = true;
                canActiveTransport = false;
                canAbilityMovement = false;
              
                canControllorMovement = false;

                isAbilityMovement = false;
                isPassiveMovement = false;
                isControllorMovement = false;

                isInAbnormalStatus = true;
                break;
        }
    }

    /// <summary>
    /// 把速度设为0，用于被禁锢
    /// </summary>
    public void SetSpeedNull()
    {
       rb.velocity = noVelocity;
    }
    public void SetSkillTypeNull()
    {
        enemyAnim.SetInteger("skillType", 0);
        attackComponent.isInAction = false;
    }
    /// <summary>
    /// 改变敌人移动速度的方法
    /// </summary>
    /// <param name="time">持续时间</param>
    /// <param name="speedChange">改变移动/追击速度的参数，0~1为减速；1以上为加速</param>
    /// <param name="jumpforceChange">改变跳跃速度的参数，默认为1不改变；0~1为减少跳力；1以上为增加跳力</param>
    public virtual void ChangeSpeed(float time,float speedChange,float jumpforceChange = 1f)
    {
        speedChangeTime = time;
        speed *= speedChange;
        seekSpeed *= speedChange;
        jumpForce *= jumpforceChange;
        Invoke("RecoverSpeed", time);
    }
    public virtual void ChangeSpeed(float speedChange,float jumpforceChange = 1f)
    {   
        speed *= speedChange;
        seekSpeed *= speedChange;
        jumpForce *= jumpforceChange; 
    }
    /// <summary>
    /// 恢复敌人至初始速度
    /// </summary>
    public virtual void RecoverSpeed()
    {
        //Debug.Log("??");
        speed = originSpeed;
        seekSpeed = originSeekSpeed;
        jumpForce = originJumpForce;
    }
    
    public EnemyStatus GetEnemyStatus()
    {
        return currentStatus;
    }

    protected RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;//人物位置

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        Debug.DrawRay(pos + offset, rayDiraction * length, color);//显示射线

        return hit;
    }
}

