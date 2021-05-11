using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementFlyingEye : MovementEnemies
{
    
    private int priorityInType = 4;
    //private GameObject moveArea;
    private BoxCollider2D moveArea;
    private Vector2 minPos;
    private Vector2 maxPos;
    private Vector2 movePos;
    private float waitTime;
    private float originWaitTime = 2f;
    private Collider2D dashColl;
    public bool isDashing = false;

    public override void Initialize()
    {
        base.Initialize();

        originx = transform.position.x;
        leftx = originx - 4f;
        rightx = originx + 4f;
        enemyAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (transform.localScale.x > 0)
        {
            faceRight = true;
        }
        else
        {
            faceRight = false;
        }

        speed = 2f;
        originSpeed = speed;
        seekSpeed = 3f;
        originSeekSpeed = seekSpeed;
        //jumpForce = 0f;
        //originJumpForce = jumpForce;

        //toRightVelocity = new Vector2(speed, jumpForce);
        //toLeftVelocity = new Vector2(-speed, jumpForce);

        moveArea = transform.parent.GetComponentInChildren<BoxCollider2D>();
        minPos.x = moveArea.transform.position.x - (moveArea.size.x / 2);
        minPos.y = moveArea.transform.position.y - (moveArea.size.y / 2);
        maxPos.x = moveArea.transform.position.x + (moveArea.size.x / 2);
        maxPos.y = moveArea.transform.position.y + (moveArea.size.y / 2);

        //获取随机移动的目标位置并改变朝向
        movePos= GetRandomPos();
        ScaleToRight();
        waitTime = originWaitTime;

        dashColl = GetComponentsInChildren<Collider2D>()[2];
        dashColl.enabled = false;
        //leftV = new Vector2(-0.3f, -0.55f);
        //rightV = new Vector2(0.3f, -0.55f);
    }

    //public override void PlayerCheck()
    //{
    //    int face = faceRight ? 1 : -1;
    //    RaycastHit2D eyeCheck = Raycast(new Vector2(0f, 0f), new Vector2(face, 0), 7f, playerLayer);
    //    RaycastHit2D backCheck = Raycast(new Vector2(0f, 0f), new Vector2(-face, 0), 3f, playerLayer);
    //    isSeePlayer = eyeCheck||backCheck;
    //}

    //public override void PhysicsCheck()
    //{
    //    base.PhysicsCheck();
    //    enemyAnim.SetFloat("yVelocity", rb.velocity.y);
    //}
    private float length = 0f;
    public override void Seek()
    {
        if (isDashing) DashEventFinish();
        //进行运动
        //PhysicsCheck();
        if (RequestMoveByFrame(MovementMode.Normal))//申请是否可移动
        {
            RecoverScale();
            length = Mathf.Abs(player.transform.position.x - transform.position.x);
            if (length > 0.3f)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, seekSpeed * Time.deltaTime);
            }
        }
    }

    private void ScaleToRight()
    {
        //朝向矫正
        if (movePos.x > transform.position.x)
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
    public override void NormalMove()
    {
        //矫正冲刺可能遗留的速度
        if (isDashing) DashEventFinish();

        if (RequestMoveByFrame(MovementMode.Normal))//申请是否可移动
        {
            //移动
            //ScaleToRight();
            transform.position = Vector2.MoveTowards(transform.position, movePos,speed * Time.deltaTime);
        }
        if (Vector2.Distance(transform.position, movePos) < 0.2f)
        {
            if (waitTime <= 0)
            {
                movePos= GetRandomPos();
                waitTime = originWaitTime;
                ScaleToRight();
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
            
        }
    }
    public override void StunEvent()
    {
        enemyAnim.SetBool("stunning", true);
    }

    public override void Dash()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, seekSpeed*Time.deltaTime);
    }
    public void DashEvent()
    {
        isDashing = true;
        isUnStoppable = true;
        dashColl.enabled = true;
        ChangeSpeed(4f);
    }
    public void DashEventFinish()
    {
        
        RecoverSpeed();
        isUnStoppable = false;
        isDashing = false;
        dashColl.enabled = false;
        SetSkillTypeNull();
    }
   
    private Vector2 GetRandomPos()
    {
        Vector2 rndPos = new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
        return rndPos;
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }
}
