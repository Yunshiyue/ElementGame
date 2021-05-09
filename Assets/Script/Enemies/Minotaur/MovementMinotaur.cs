using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMinotaur : MovementEnemies
{
   
    private DefenceEnemies defenceComponent;
    // private Vector2 dashVelocity =new Vector2(10f,)
    public bool isDashing = false;
    private int priorityInType = 6;
    private Collider2D dashColl;
    public override void Initialize()
    {
        base.Initialize();

        originTransform = transform.position;
        originx = transform.position.x;
        leftx = originx - 4f;
        rightx = originx + 4f;

        //player = GameObject.Find("Player");

        enemyAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        dashColl = GameObject.Find("DashColl").GetComponent<Collider2D>();
        dashColl.enabled = false;
        attackComponent = GetComponent<AttackEnemies>();
        defenceComponent = GetComponent<DefenceEnemies>();

        //确定朝向
        if (transform.localScale.x > 0)
        {
            faceRight = true;
        }
        else
        {
            faceRight = false;
        }
        //初始化速度
        speed = 2f;
        seekSpeed = 3f;
        originSpeed = speed;
        originSeekSpeed = seekSpeed;
        toRightVelocity = new Vector2(speed, jumpForce);
        toLeftVelocity = new Vector2(-speed, jumpForce);

        //脚下检测点偏移量
        leftV = new Vector2(0f, -2.1f);
        rightV = new Vector2(2f, -2.1f);

        //boss无法被轻易眩晕
        isUnStoppable = true;
        //疲劳状态CD
        statusCD = 5f;
    }

    public override void MyUpdate()
    {
        base.MyUpdate();
        if((currentStatus == EnemyStatus.Stun)||curStatusCD<=0)
        {
            isTired = false;
        }
    }
    public override void Idle()
    {
        SetSpeedNull();
        enemyAnim.SetBool("idle", true);
    }
    //范围内移动
    public override void NormalMove()
    {
        base.NormalMove();
        enemyAnim.SetBool("idle", false);
    }

    public override void Dash()
    {
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
    public void DashEvent()
    {
        isDashing = true;
        dashColl.enabled = true;
        ChangeSpeed(1f, 4);
    }
   

    public override void RecoverSpeed()
    {
        base.RecoverSpeed();
        isDashing = false;
        dashColl.enabled = false;
        ToBeTired();
        //isUnStoppable = false;
        //enemyAnim.SetBool("tired", true);
    }

    public void ToBeTired()
    {
        //Debug.LogError("????");
        isTired = true;
        attackComponent.isInAction = false;
        enemyAnim.SetInteger("skillType", 0);
        curStatusCD = statusCD;
    }
    public float GetOriginX()
    {
        return originx;
    }
    public override void StunEvent()
    {
       //isTired = false;
       enemyAnim.SetBool("stunning", true);
    }
    public override void PlayerCheck()
    {
        int face = faceRight ? 1 : -1;
        RaycastHit2D eyeCheck = Raycast(new Vector2(0f, -1.5f), new Vector2(face, 0), 3f, playerLayer);
        isSeePlayer = eyeCheck;

    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }
    

}
