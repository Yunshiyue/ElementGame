using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementWitcher : MovementEnemies
{ 
    private CapsuleCollider2D coll;
    private DefenceEnemies defenceComponent;

    //public bool faceRight = true;
    //GameObject player;

    [SerializeField]//private在unity可见
    //[Header("移动参数")]
    //private float speed = 1f;
    //移动范围
    //public Transform originTransform;
    //private float  originx,leftx, rightx;

    //player中优先级等级为5
    private int priorityInType = 5;
    public bool isTimeToBlinkBack = false;
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
        coll = GetComponent<CapsuleCollider2D>();
        defenceComponent = GetComponent<DefenceEnemies>();
        if (transform.localScale.x > 0)
        {
            faceRight = true;
        }
        else
        {
            faceRight = false;
        }
        //初始化速度
        speed = 1f;
        seekSpeed = 2f;
        originSpeed = speed;
        originSeekSpeed = seekSpeed;
        toRightVelocity = new Vector2(speed, jumpForce);
        toLeftVelocity = new Vector2(-speed, jumpForce);

        //脚下检测点偏移量
        leftV = new Vector2(-0.3f, -0.55f);
        rightV = new Vector2(0.3f, -0.55f);
    }

    public override void MyUpdate()
    {
        base.MyUpdate();
        if ((rb.gravityScale==0) && (defenceComponent.getHpReduction()>0))
        {
            //被打后恢复重力
            //rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1;
        }

        //targetPosOffset1 = targetPos;
        //targetPosOffset2 = targetPos;
        //targetPosOffset1.x = targetPosOffset1.x - 0.5f;
        //targetPosOffset2.y = targetPosOffset2.y + 0.5f;
        //RaycastHit2D blinkCheck1 = Physics2D.Raycast(targetPosOffset1, Vector2.right, 1f, groundLayer);
        //RaycastHit2D blinkCheck2 = Physics2D.Raycast(targetPosOffset2, Vector2.down, 1f, groundLayer);
        //Color color = blinkCheck1 ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green
        //Color color2 = blinkCheck2 ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        //Debug.DrawRay(targetPosOffset1, Vector2.right, color);//显示射线
        //Debug.DrawRay(targetPosOffset2, Vector2.down, color2);//显示射线
    }

    //请求在这一帧中进行position的跳跃，movement为相对位移改变量
    //同理，技能控制会根据当前主角朝向改变方向，而被动传送则不会
    public override bool RequestMoveByFrame(MovementMode mode)
    {
        //PlayerCheck();
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
                    //Debug.Log("我闪)
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
    public override void NormalMove()
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

    public override void Seek()
    {
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
    public void ChangeRange(float r, float l)
    {
        rightx = r;
        leftx = l;
    }
    public float GetOriginX()
    {
        return originx;
    }

    //帧事件调用闪现
    Vector3 targetPos;
    public bool targetIsOk = true;
    private void Blink()
    {
        
        
        //Color color = blinkCheck ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        //Debug.DrawRay(pos + offset, rayDiraction * length, color);//显示射线

        transform.position = targetPos;
        RecoverScale();
        enemyAnim.SetBool("blinking", false);
        
    }
    Vector3 targetPosOffset1, targetPosOffset2;
    public void IsTimeToBlink()
    {
        if (isTimeToBlinkBack)
        {
            isTimeToBlinkBack = false;
            targetPos = originTransform;
        }
        else
        {
            targetPos = new Vector3(player.transform.position.x + 5f * (-player.transform.localScale.x), player.transform.position.y, 0);
        }
        //查看闪现目标位置是否为墙体
        targetPosOffset1 = targetPos;
        targetPosOffset2 = targetPos;
        targetPosOffset1.x = targetPosOffset1.x - 0.4f;
        targetPosOffset2.y = targetPosOffset2.y + 0.4f;
        RaycastHit2D blinkCheck1 = Physics2D.Raycast(targetPosOffset1, Vector2.right, 0.8f, groundLayer);
        RaycastHit2D blinkCheck2 = Physics2D.Raycast(targetPosOffset2, Vector2.down, 0.8f, groundLayer);
       
        if (blinkCheck1 || blinkCheck2)
        {
            targetIsOk = false;
            return;
        }
        else
        {
            if (RequestMoveByFrame(MovementMode.Ability))
            {
                //不受物理控制取消重力
                //rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0;
                Invoke("RecoverRbType", 1.7f);//恢复重力
                enemyAnim.SetBool("blinking", true);
            }
        }
            
    }

    private void RecoverRbType()
    {
        //rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
    }
    
    public override void PlayerCheck()
    {
        int face = faceRight ? 1 : -1;
        RaycastHit2D eyeCheck = Raycast(new Vector2(0f, 0f), new Vector2(face, 0), 7f, playerLayer);
        isSeePlayer = eyeCheck;
       
    }
    public void setIsGravity(bool isGravity)
    {
        if (isGravity)
        {
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = 0;
        }

    }
    public bool getIsSeePlayer()
    {
        return isSeePlayer;
    }
    public override int GetPriorityInType()
    {
        return priorityInType;
    }

}
