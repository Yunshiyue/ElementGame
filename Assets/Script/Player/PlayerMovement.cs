using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @Description: In User Settings Edit  实现人物的移动，跳跃（长按跳跃），下蹲（蹲跳），碰撞检测
 * @Author: your name  lanyihao

 *
 * 备注：人物锚点怎么调整位置？（貌似只能在sprite editor里面调整） 我一开始的人物锚点在中心，导致在用射线做碰撞检测
 * 的时候不能像M_studio的教程那样直接弄好（他的素材锚点直接在底部），而是做了一些手动的数字调整，目前来说测试没什么问题。。。
 * 看以后有没有需要再调整了
 *
*/
public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;//蹲下时的移速

    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpForce = 2.5f;

    float jumpTime;

    [Header("状态")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlock;
    public bool isHanging;

    //按键状态
    public bool jumpPressed;
    public bool jumpHeld;
    public bool crouchHeld;

    [Header("环境检测")]
    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundStance = 0.1f;

    public LayerMask groundLayer;

    public float xVelocity;

    //碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;//坐标
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;


    //临时测试
    int Hp = 3;
    public GameObject enemy;
    Transform sword;
    void Start()
    {
        //获取刚体和碰撞体
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();

        colliderStandSize = coll.size;//人物站立时大小
        colliderStandOffset = coll.offset;//人物站立时坐标
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2);//人物蹲下时大小
        colliderCrouchOffset = new Vector2(coll.offset.x, -0.2418612f);//人物蹲下时坐标

        ///测试HP显示
        for (int i = 0; i < Hp; i++)
        {
            GameObject HpPanel = GameObject.Find("HP Panel");
            Transform hp = HpPanel.transform.GetChild(i);
            hp.GetComponent<HPItem>().Getting();
        }
        //技能动画测试
        sword = transform.GetChild(0);
       
    }

    // Update is called once per frame
    void Update()
    {
        //获取按键状态
        jumpPressed = Input.GetButtonDown("Jump");//按下时返回true 按住和释放都不为true，只有一个帧为true
        jumpHeld = Input.GetButton("Jump");//只要按下（按住）就返回true
        crouchHeld = Input.GetButton("Crouch");

        //技能动画测试
        if (Input.GetKeyDown("u"))
        {
            sword.GetComponent<SwordHandle>().SkillAttack(enemy.transform.position);

        }
    }

    private void FixedUpdate()
    {
        physicsCheck();
        Movement();
        MidAirMovement();
    }

    /// <summary>
    /// 人物移动
    /// </summary>
    void Movement()
    {
        //先判断是否为蹲
        if (crouchHeld && !isCrouch && isOnGround)//按住蹲键，当前状态不是蹲，并在地面上
        {
            Crouch();
        }
        else if (!crouchHeld && isCrouch && !isHeadBlock)//!isHeadBlock：头部上没有碰撞物
        {
            StandUp();
        }
        else if (!isOnGround && isCrouch)
        {
            StandUp();
        }

        xVelocity = Input.GetAxisRaw("Horizontal");//获得水平方向

        if (isCrouch)//处理蹲下时的速度
        {
            xVelocity /= crouchSpeedDivisor;
        }

        //人物移动速度
        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        FilpDirction();
    }

    /// <summary>
    /// 跳跃，空中的移动
    /// </summary>
    void MidAirMovement()
    {
        if (jumpHeld && isOnGround && !isJump)
        {
            if (isCrouch && isOnGround)//蹲跳，比直接跳多加一个力crouchJumpForce
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpForce), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;//Time.time系统的时间 在不断增长；  jumpTime为长按跳跃的有效时间

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        else if (isJump)
        {
            if (jumpHeld)//长按跳跃时，按住时持续给力
            {
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            }
            if (jumpTime < Time.time)//如果系统时间超过了jumpTime 那么将无法再有长按跳跃效果
            {
                isJump = false;
            }
        }
    }

    /// <summary>
    /// 人物朝向
    /// </summary>
    void FilpDirction()
    {
        if (xVelocity < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        if (xVelocity > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    /// <summary>
    /// 下蹲动作
    /// </summary>
    void Crouch()
    {
        isCrouch = true;
        //改变碰撞体大小
        coll.enabled = false;
    }

    /// <summary>
    /// 站起动作
    /// </summary>
    void StandUp()
    {
        isCrouch = false;
        //改变碰撞体大小
        //coll.size = colliderStandSize;
        //coll.offset = colliderStandOffset;
        coll.enabled = true;
    }

    /// <summary>
    /// 物理检测，检测人物是否在地面;
    /// leftCheck：左脚  ；rightCheck：右脚;
    /// headCheck：头部检测
    /// </summary>
    void physicsCheck()
    {
        RaycastHit2D leftCheck = Raycast(new Vector2(-coll.size.x / 2, -colliderStandSize.y / 2 + 0.029f), Vector2.down, groundStance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(coll.size.x / 2, -colliderStandSize.y / 2 + 0.029f), Vector2.down, groundStance, groundLayer);

        if (leftCheck || rightCheck)
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }

        RaycastHit2D headCheck = Raycast(new Vector2(0, coll.size.y - colliderStandSize.y / 2 + 0.029f), Vector2.up, headClearance, groundLayer);

        isHeadBlock = headCheck;
    }

    /// <summary>
    /// 射线检测方法，对原本的Physics2D.Raycast(Vector2 origin, Vector2 direction, float length, int layerMask)方法进行封装
    /// 射线的起始点origin由当前人物位置pos + 偏移量 offset确定
    /// </summary>
    /// <param name="offset">偏移量</param>
    /// <param name="rayDiraction">射线方向</param>
    /// <param name="length">射线长度</param>
    /// <param name="layer">投射射线，选择投射的层蒙版</param>
    /// <returns>返回bool，判断射线是否触碰到layer</returns>
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;//人物位置

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        Debug.DrawRay(pos + offset, rayDiraction * length, color);//显示射线

        return hit;
    }
}
