using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitcherMovement : MovementEnemies
{
    public Animator witcherAnim;//动画

    private Rigidbody2D rb;
    private CapsuleCollider2D coll;

    public bool faceRight = true;

    [SerializeField]//private在unity可见
    [Header("移动参数")]
    private float speed = 1f;
    //移动范围
    private float originx, leftx, rightx;

    //player中优先级等级为5
    private int priorityInType = 5;

    public override void Initialize()
    {
        originx = transform.position.x;
        leftx = originx - 1f;
        rightx = originx + 1f;

        //player= GameObject.Find("Player");

        witcherAnim = GetComponent<Animator>();
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
    public override bool RequestMoveByFrame( MovementMode mode)
    {
        Vector2 movement = new Vector2(1, 0);//
        switch (mode)
        {
            case MovementMode.Normal:
                //普通移动
                if (canControllorMovement)
                {
                    isControllorMovement = true;
                    RangeMove();
                return true;
                }
                return false;

             case MovementMode.Ability:
                if (canActiveTransport)
                {
                    //实现闪现
                    Debug.Log("我闪");
                    witcherAnim.SetBool("blinking", true);
                    
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

    //范围内移动
    private void RangeMove()
    {
        if (faceRight)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            if (transform.position.x > rightx)
            {

                transform.localScale = new Vector3(-1, 1, 1);
                faceRight = false;
            }
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
    public void ChangeRange(float r,float l)
    {
        rightx = r;
        leftx = l;
    }
    private void Blink()
    {
        Transform temp = GameObject.Find("Player").transform;
        Vector3 target = new Vector3(temp.position.x + 3f * (-temp.localScale.x), temp.position.y , 0);
        transform.position = target;
        witcherAnim.SetBool("blinking", false);
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }
}
