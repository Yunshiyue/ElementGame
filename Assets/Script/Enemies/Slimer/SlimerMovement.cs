using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimerMovement : MovementEnemies
{
    public Animator slimerAnim;//动画

    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    public LayerMask Ground;

    public bool faceRight = true;
    [SerializeField]//private在unity可见
    [Header("移动参数")]
    private float speed = 2f;
    private float jumpForce = 5f;

    private float originx, leftx, rightx;

    /// <summary>
    /// 是否看见敌人
    /// </summary>
    public bool eyeSee = false;


    //优先级等级为5
    private int priorityInType = 4;

    public override void Initialize()
    {
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
        Vector2 movement = new Vector2(1, 0);//
        switch (mode)
        {
            case MovementMode.Normal:
                //普通移动
                if (canControllorMovement)
                {
                    isControllorMovement = true;
                    //移动代码
                    slimerAnim.SetFloat("yVelocity", rb.velocity.y);

                    if (coll.IsTouchingLayers(Ground))
                    {
                        if (faceRight)
                        {
                            if (eyeSee)
                            {
                                rb.velocity = new Vector2(speed + 0.5f, jumpForce);
                            }
                            else
                            {
                                rb.velocity = new Vector2(speed, 0f);
                                if (transform.position.x > rightx)
                                {

                                    transform.localScale = new Vector3(-1, 1, 1);
                                    faceRight = false;
                                }
                            }
                        }
                        else
                        {
                            if (eyeSee)
                            {
                                rb.velocity = new Vector2(-speed - 0.5f, jumpForce);
                            }
                            else
                            {
                                rb.velocity = new Vector2(-speed, 0f);
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
     
    public override int GetPriorityInType()
    {
        return priorityInType;
    }
}
