using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * @Description: 怪物史莱姆 ，普通移动，扑击
 * @Author:  夜里猛

 
 *备注：速度 跳跃力方面的数值有待调整，暂时没完善父类，待怪物多时，寻找共同点再丰富父类。
 *
 * @Edit: 
 */
public class Slimer : Enemy
{
    private Rigidbody2D rb;
    //private Animator Anim;
    private Collider2D coll;
    private Animator anim;
    public LayerMask Ground;

    private CanFight fight;
    private float interruptTime = 0.2f;
    private Vector2 interruptVector = new Vector2(0.5f, 0);
    private string fightTarget = "Player";

    private DefenceSlimer defence;

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

        anim = GetComponent<Animator>();

        originx = transform.position.x;
        leftx = originx - 4f;
        rightx = originx + 4f;

        fight = GetComponent<CanFight>();
        if(fight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到canFight组件");
        }
        string[] target = new string[1];
        target[0] = fightTarget;
        //初始化攻击目标
        fight.Initiailize(target);

        defence = GetComponent<DefenceSlimer>();
        if(defence == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到defence组件");
        }
        defence.Initialize(2);
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

        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isAttacking", eyeSee);

        defence.AttackCheck();
        if(defence.getIsDead())
        {
            gameObject.SetActive(false);
        }
        defence.Clear();
        
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Transform player = collision.transform;
        if (player.gameObject.name == "Player")
        {
            //Debug.Log("碰到了主角！");
            fight.Attack(collision.transform.GetComponent<CanBeFighted>(), 2, AttackInterruptType.WEAK);
            MovementPlayer movement = player.GetComponent<MovementPlayer>();
            if(movement.RequestChangeControlStatus(interruptTime, MovementPlayer.PlayerControlStatus.Interrupt))
            {
                if(player.position.x > transform.position.x)
                {
                    movement.RequestMoveByTime(interruptVector, interruptTime, MovementPlayer.MovementMode.Attacked);
                }
                else
                {
                    movement.RequestMoveByTime(-interruptVector, interruptTime, MovementPlayer.MovementMode.Attacked);
                }
            }
        }
    }
}
