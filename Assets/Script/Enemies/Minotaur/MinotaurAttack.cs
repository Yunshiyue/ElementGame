using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(MovementMinotaur))]
public class MinotaurAttack : AttackEnemies
{
    //相机
    private Transform cameraTransform;

    //CanFight canFight;
    MovementMinotaur movementComponent;
    private string targetLayerName = "Player";
    protected PoolManager poolManager;

    //下切攻击 技能1
    private Collider2D cutColl;
    private int cutDamage = 2;
    private float cutAttackTime = 0.5f;
    private float cutAttackInterruptTime = 0.5f;
    private Vector2 cutAttackInterruptVetcor = new Vector2(3,0);

    //旋风斩 技能2
    private Collider2D whirlWindColl;
    private int WhirlWindDamage = 2;
    private float WhirlWindCollAttackTime = 0.5f;
    private float whirlWindInterruptTime = 0.5f;
    private Vector2 whirlWindInterruptVetcor = new Vector2(4,0);

    //火球初始位置 技能3
    private Vector2 flyingFromDirection;

    //冲刺 技能4
    //private int dashDamage = 2;
    private float dashAttackTime = 3f;
    //private float dashInterruptTime = 1f;
    //private Vector2 dashInterruptVetcor = new Vector2(4, 0);
    void Awake()
    {
        canFight = GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
        }

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);

        movementComponent = GetComponent<MovementMinotaur>();
        if (movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }

        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        cutColl = GameObject.Find("CutColl").GetComponent<Collider2D>();
        whirlWindColl = GameObject.Find("WhirlWindColl").GetComponent<Collider2D>();
        cameraTransform = GameObject.Find("Main Camera").transform;

        //火球位置
        flyingFromDirection = new Vector2(movementComponent.originx-3f, 10f);

        //用于行为树的一些参数
        canActiveAttack = true;
        attackLength = 2f;
       
        //技能cd
        skillCD1 = 3f;
        skillCD2 = 8f;
        skillCD3 = 10f;
        skillCD4 = 15f;
    }
    //public override void AttackControl()
    //{
    //    PushFire();
    //}

    //下劈攻击
    public override void Skill1()
    {
        if (movementComponent.RequestChangeControlStatus(cutAttackTime, MovementEnemies.EnemyStatus.AbilityWithNoMovement))
        {
            isInAction = true;
            movementComponent.RecoverScale();
            movementComponent.enemyAnim.SetInteger("skillType", 1);
        }
    }
    public void CutAttackEvent()
    {
        cameraTransform.DOShakeRotation(1, new Vector3(1f, 1.5f, 0));

        targets = canFight.AttackArea(cutColl, cutDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                //击退效果
                a.BeatBack(transform, cutAttackInterruptTime, cutAttackInterruptVetcor);
            }
        }
    }

    //旋风斩
    public override void Skill2()
    {
        if (movementComponent.RequestChangeControlStatus(cutAttackTime, MovementEnemies.EnemyStatus.AbilityWithNoMovement))
        {
            isInAction = true;
            movementComponent.enemyAnim.SetInteger("skillType", 2);
        }
    }

    public void WhirlWindAttackEvent()
    {
        targets = canFight.AttackArea(whirlWindColl, WhirlWindDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                //击退效果
                a.BeatBack(transform, whirlWindInterruptTime, whirlWindInterruptVetcor);
            }
        }
    }

    public override void Skill3()
    {
        if (movementComponent.RequestChangeControlStatus(0.05f, MovementEnemies.EnemyStatus.AbilityWithNoMovement))
        {
            isInAction = true;
            movementComponent.RecoverScale();
            movementComponent.enemyAnim.SetInteger("skillType", 3);
            //帧事件 FireBall
            //Debug.Log("fire");
        }
    }
    //疲劳状态
    //public void Tired()
    //{
    //    movementComponent.enemyAnim.SetInteger("skillType", 3);
    //}
    
    public void FireBall()
    {
        cameraTransform.DOShakeRotation(0.5f, new Vector3(1f, 2f, 0));

        for (int i = 0; i < 8; i++)
        {   
            GameObject fireBallAbility = poolManager.GetGameObject("FireBallAbility");
            FireBallAbility a = fireBallAbility.GetComponent<FireBallAbility>();
            a.SetThrower(gameObject);
            a.SetStartPosition(flyingFromDirection);
            flyingFromDirection.x += 2f;
            a.SetDirection(Vector3.down);
            a.SetTargetLayerName("Player");
            a.SetDamage(2);
        }
        flyingFromDirection.x = movementComponent.originx - 7f;
    }

    //冲刺
    public  override void Skill4()
    {
        if (movementComponent.RequestChangeControlStatus(dashAttackTime, MovementEnemies.EnemyStatus.AbilityWithNoMovement))
        {
            cameraTransform.DOShakeRotation(2f, new Vector3(1f, 1f, 0));
            isInAction = true;
            movementComponent.RecoverScale();
            movementComponent.enemyAnim.SetInteger("skillType", 4);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (movementComponent.isDashing)
    //    {
    //        Transform player = collision.transform;
    //        if (player.gameObject.name == "Player")
    //        {
    //            //Debug.Log("碰到了主角！");
    //            canFight.Attack(player.GetComponent<CanBeFighted>(), 1, AttackInterruptType.WEAK);
    //            isHitPlayer = true;
    //            player.GetComponent<CanBeFighted>().BeatBack(transform, dashInterruptTime, dashInterruptVetcor);

    //        }
    //    }
        
    //}
}
