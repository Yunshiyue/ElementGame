/**
 * @Description: 主角的控制脚本，继承自myUpdate类。拥有移动组件、攻击组件、防御组件等主要组件。
 *               负责输入控制和通知各组件控制信息，诸如移动、跳跃、释放技能等。
 * @Author: ridger

 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MovementPlayer))]
[RequireComponent(typeof(AttackPlayer))]
[RequireComponent(typeof(DefencePlayer))]
public class Player : myUpdate
{
    private MovementPlayer movementComponent;
    private AttackPlayer attackComponent;
    private DefencePlayer defenceComponent;

    private PlayerAnim playerAnim;

    //updateManager.player调用顺序
    //Player: 0
    //OnFloorDetector: 2
    private int priorityInType = 0;
    private UpdateType updateType = UpdateType.Player;
    //临时变量
    private Vector2 tempMovement = new Vector2(0, 0);

    private GameObject HpPanel;//血条UI
    private void Start()
    {
        HpPanel = GameObject.Find("HP Panel");
    }
    public override void Initialize()
    {
        playerAnim = GetComponent<PlayerAnim>();

        movementComponent = GetComponent<MovementPlayer>();
        if(movementComponent == null)
        {
            Debug.LogError("在Player中，没有找到MovementPlayer脚本！");
        }

        attackComponent = GetComponent<AttackPlayer>();
        if (attackComponent == null)
        {
            Debug.LogError("在Player中，没有找到AttackPlayer脚本！");
        }

        defenceComponent = GetComponent<DefencePlayer>();
        if(defenceComponent == null)
        {
            Debug.LogError("在Player中，没有找到DefencePlayer脚本！");
        }
        defenceComponent.Initialize(5);

        
    }
    private string temp;
    //根据设计，先进行受伤判断，再进行移动控制和技能控制
    public override void MyUpdate()
    {
        MenuCheck();
        DefenceCheck();
        MoveControl();


        //将技能控制的结果输出到控制台
        temp = AttackControl();
        if(temp != null)
        {
            Debug.Log(temp);
        }

    }
    private void MenuCheck()
    {
        if(Input.GetButtonDown("ReloadScene"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //防御组件检查，目前只统计信息，但并不对信息做什么处理
    private void DefenceCheck()
    {
        defenceComponent.AttackCheck();
        defenceComponent.Clear();
    }
    //移动控制，包括X轴移动、跳跃、下蹲；通过向移动组件“请求”实现。
    private void MoveControl()
    {
        tempMovement.x = Input.GetAxis("Horizontal");
        movementComponent.RequestMoveByFrame(tempMovement, MovementPlayer.MovementMode.PlayerControl);

        if(Input.GetButtonDown("Jump"))
        {
            movementComponent.RequestJump();
        }
        if(Input.GetButton("Crouch"))
        {
            movementComponent.RequestCrouch();
        }
    }

    //技能控制，在一帧中只能释放一个技能
    private string AttackControl()
    {
        if (Input.GetButtonDown(AttackPlayer.BLINK_NAME))
        {
            attackComponent.blink();
            return AttackPlayer.BLINK_NAME;
        }

        if (Input.GetButtonDown(AttackPlayer.DASH_NAME))
        {
            attackComponent.dash();
            return AttackPlayer.DASH_NAME;
        }

        if (Input.GetButtonDown(AttackPlayer.NORMAL_ATTACK_NAME))
        {
            
            attackComponent.NormalAttack();
            return AttackPlayer.NORMAL_ATTACK_NAME;
        }

        if (Input.GetButtonDown("Dart"))
        {
            GameObject dart = PoolManager.Instance.GetGameObject(PoolManager.poolType.Dart, gameObject.transform.position);
            dart.GetComponent<Dart>().Init(gameObject);
            return "Dart";
        }
        return null;
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
}
