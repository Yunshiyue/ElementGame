using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * @Description: 怪物史莱姆 ，普通移动，扑击
 * @Author:  夜里猛
 * @Date: 2021/1/27  作者编写完成时间
 
*备注：速度 跳跃力方面的数值有待调整，暂时没完善父类，待怪物多时，寻找共同点再丰富父类。

*/
public class Slimer : Enemy
{
    private Rigidbody2D rb;
    //private Animator Anim;
    private Collider2D coll;
    public LayerMask Ground;


    private float originx, leftx, rightx;

    private bool faceRight = true;//面朝左边

    /// <summary>
    /// 是否看见敌人
    /// </summary>
    private bool eyeSee = false;
    private bool canCheck = true;//是否可以检测
    private float eyeTime;//目击时间


    [Header("移动参数")]
    public float speed = 2f;
    public float jumpForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        base.Initialize();//Enermy初始化

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        originx = transform.position.x;
        leftx = originx - 4f;
        rightx = originx + 4f;
        

    }

    // Update is called once per frame
    void Update()
    {
        //检测player
        if (canCheck) {
            PlayerCheck();
        }
        else
        {
            if (Time.time > eyeTime)
            {
                canCheck = true;
            }
        }
        
        //移动
        if (!isDead)
        {
            Movement();
        }

        //Debug.DrawLine(new Vector3(leftx, transform.position.y, 0f), new Vector3(rightx, transform.position.y, 0f), Color.green);
    }

    public override void Movement()
    {
        if (coll.IsTouchingLayers(Ground)){
            //普通移动/看到人时扑击
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
        }
    }

    /// <summary>
    /// player检测
    /// </summary>
    void PlayerCheck()
    {
        int face = faceRight ? 1 : -1;
        RaycastHit2D eyeCheck = Raycast(new Vector2(0f, 0.57f), new Vector2(face,0), 7f, playerLayer);
        eyeSee = eyeCheck;

        //如果看到了player 则暂时不再检测 1s
        if (eyeSee) {
            canCheck = false;
            eyeTime = Time.time + 1f;

        }
        
    }
}
