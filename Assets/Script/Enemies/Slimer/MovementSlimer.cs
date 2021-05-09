using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSlimer : MovementEnemies
{
    
    private int priorityInType = 4;
    //[Header("移动参数")]
    //public float speed = 0.3f;
    //public float jumpForce = 4f;
  
    //RaycastHit2D leftCheck, rightCheck;
    //Vector2 leftV = new Vector2(-0.3f, -0.55f);
    //Vector2 rightV = new Vector2(0.3f, -0.55f);

    
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
        speed = 1f;
        originSpeed = speed;
        seekSpeed = 2f;
        originSeekSpeed = seekSpeed;
        jumpForce = 4f;
        originJumpForce = jumpForce;

        toRightVelocity = new Vector2(speed, jumpForce);
        toLeftVelocity = new Vector2(-speed, jumpForce);
        leftV = new Vector2(-0.3f, -0.55f);
        rightV = new Vector2(0.3f, -0.55f);
    }
    
    public override void PlayerCheck()
    {
        int face = faceRight ? 1 : -1;
        RaycastHit2D eyeCheck = Raycast(new Vector2(0f, 0f), new Vector2(face, 0), 7f, playerLayer);
        RaycastHit2D backCheck = Raycast(new Vector2(0f, 0f), new Vector2(-face, 0), 3f, playerLayer);
        isSeePlayer = eyeCheck||backCheck;
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
        enemyAnim.SetBool("isOnGround", isOnGround);
        enemyAnim.SetFloat("yVelocity", rb.velocity.y);
    }

    public override void Seek()
    {
        //进行运动
        //PhysicsCheck();
        if (RequestMoveByFrame(MovementMode.Normal))//申请是否可移动
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
                    toRightVelocity.y = jumpForce;
                    rb.velocity = toRightVelocity;
                    //if (transform.position.x > player.transform.position.x)
                    //{
                    //    transform.localScale = leftScale;
                    //    faceRight = false;
                    //}
                }
                else
                {
                    toLeftVelocity.x = -seekSpeed;
                    toLeftVelocity.y = jumpForce;
                    rb.velocity = toLeftVelocity;
                    //if (transform.position.x < player.transform.position.x)
                    //{
                    //    transform.localScale = rightScale;
                    //    faceRight = true;
                    //}
                }

            }
        }
    }

    public override void NormalMove()
    {
        //PhysicsCheck();

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


    public override int GetPriorityInType()
    {
        return priorityInType;
    }
}
