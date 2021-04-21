using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitcherMovement : MovementEnemies
{
    public Animator witcherAnim;//动画

    private CapsuleCollider2D coll;

    public bool faceRight = true;
    GameObject player;

    [SerializeField]//private在unity可见
    [Header("移动参数")]
    private float speed = 1f;
    //移动范围
    //public Transform originTransform;
    private float  originx,leftx, rightx;
    
    //player中优先级等级为5
    private int priorityInType = 5;

    public override void Initialize()
    {
        base.Initialize();

        originx = transform.position.x;
        leftx = originx - 0.5f;
        rightx = originx + 0.5f;

        player = GameObject.Find("Player");

        witcherAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        if (transform.localScale.x > 0)
        {
            faceRight = true;
        }
        else
        {
            faceRight = false;
        }
    }



    //请求在这一帧中进行position的跳跃，movement为相对位移改变量
    //同理，技能控制会根据当前主角朝向改变方向，而被动传送则不会
    public override bool RequestMoveByFrame( MovementMode mode)
    {
        PlayerCheck();
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

                ///Debug.Log(transform.position.x + " " + rightx);
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
    public float GetOriginX()
    {
        return originx;
    }
    private void Blink()
    {
        Transform temp = GameObject.Find("Player").transform;
        Vector3 target = new Vector3(temp.position.x + 3f * (-temp.localScale.x), temp.position.y , 0);
        transform.position = target;
        witcherAnim.SetBool("blinking", false);
    }

    private void PlayerCheck()
    {
        //if (!isSeePlayer)//如果没看到player时检测
        //{
            //Debug.Log("no seePlayer");
            int face = faceRight ? 1 : -1;
            RaycastHit2D eyeCheck = Raycast(new Vector2(0f, 0f), new Vector2(face, 0), 7f, playerLayer);
            isSeePlayer = eyeCheck;
        //}
        //else if (Mathf.Abs(transform.position.x - player.transform.position.x) > 10f)//失去对player的追踪
        //{
        //    isSeePlayer = false;
        //}
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
