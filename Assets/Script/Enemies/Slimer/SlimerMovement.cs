using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimerMovement : MovementEnemies
{
    public Animator slimerAnim;//动画

    
    private CapsuleCollider2D coll;
  
    [SerializeField]//private在unity可见
    [Header("移动参数")]
    private float speed = 0.3f;
    private float jumpForce = 4f;

    private float originx, leftx, rightx;

    /// <summary>
    /// 是否看见敌人
    /// </summary>
    private bool canCheck = true;//是否可以检测
    private float eyeTime;//目击时间

    //检测是否在地面上  
    public bool isOnGround = true;
    RaycastHit2D leftCheck, rightCheck;
    Vector2 leftV = new Vector2(-0.3f, -0.55f);
    Vector2 rightV = new Vector2(0.3f, -0.55f);

    //是否朝右
    public bool faceRight = true;
   
    //优先级等级为5
    private int priorityInType = 4;

    public override void Initialize()
    {
        base.Initialize();

        originx = transform.position.x;
        leftx = originx - 4f;
        rightx = originx + 4f;

        slimerAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();

    }

    override public void MyUpdate()
    {
        //如果当前状态不是Normal，则时间++，
        if (isInAbnormalStatus)
        {
            //状态时间++
            controlStatusCurTime += Time.deltaTime;
            //状态到期发生的事情
            if (controlStatusCurTime >= controlStatusTotalTime)
            {
                ChangeControlStatus(0f, EnemyStatus.Normal);
                //playerAnim.SetAbilityNum(999);
            }
        }
        //在处理正常状态
        else
        {
        }
        Clear();
    }

    
    //请求在这一帧中进行position的跳跃，movement为相对位移改变量
    //同理，技能控制会根据当前主角朝向改变方向，而被动传送则不会
    public override bool RequestMoveByFrame(MovementMode mode)
    {
        //判断是否在地面上
        PhysicsCheck();
        Vector2 movement = new Vector2(1, 0);//

        switch (mode)
        {
            case MovementMode.Normal:
                //普通移动
                if (canControllorMovement)
                {
                    //只有可以正常移动时检测player
                    PlayerCheck();
                    isControllorMovement = true;
                    //动画机设置y轴速度
                    slimerAnim.SetFloat("yVelocity", rb.velocity.y);
                    if (isOnGround)
                    {
                        if (faceRight)
                        {
                            if (isSeePlayer)
                            {
                                rb.velocity = new Vector2(speed + 0.5f, jumpForce);
                            }
                            else
                            {
                                rb.velocity = new Vector2(speed, rb.velocity.y);
                                if (transform.position.x > rightx)
                                {

                                    transform.localScale = new Vector3(-1, 1, 1);
                                    faceRight = false;
                                }
                            }
                        }
                        else
                        {
                            if (isSeePlayer)
                            {
                                rb.velocity = new Vector2(-speed - 0.5f, jumpForce);
                            }
                            else
                            {
                                rb.velocity = new Vector2(-speed, rb.velocity.y);
                                if (transform.position.x < leftx)
                                {
                                    transform.localScale = new Vector3(1, 1, 1);
                                    faceRight = true;
                                }
                            }
                        }
                        return true;
                    }
                }
                return false;

            case MovementMode.Ability:
                if (canActiveTransport)
                {
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

    private void PlayerCheck()
    {
        if (canCheck)
        {
            int face = faceRight ? 1 : -1;
            RaycastHit2D eyeCheck = Raycast(new Vector2(0f, 0f), new Vector2(face, 0), 7f, playerLayer);
            isSeePlayer = eyeCheck;

            if (isSeePlayer)
            {
                canCheck = false;
                eyeTime = Time.time + 1.5f;
                slimerAnim.SetBool("isAttacking", true);
            }
        }
        else
        {
            if (Time.time > eyeTime)
            {
                canCheck = true;
                slimerAnim.SetBool("isAttacking", false);
            }
        }
    }
    private void PhysicsCheck()
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

    public override int GetPriorityInType()
    {
        return priorityInType;
    }
}
