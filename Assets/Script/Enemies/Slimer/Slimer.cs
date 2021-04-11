﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SlimerMovement))]
[RequireComponent(typeof(SlimerAttack))]
[RequireComponent(typeof(DefenceEnemies))]
public class Slimer : Enemies
{
    private SlimerMovement movementComponent;
    private SlimerAttack attackComponent;

    private bool canCheck = true;//是否可以检测
    private float eyeTime;//目击时间
    public override void Initialize()
    {
        base.Initialize();
        priorityInType = 2;
        movementComponent = GetComponent<SlimerMovement>();
        if (movementComponent == null)
        {
            Debug.LogError("在Slimer中，没有找到Movement脚本！");
        }
        attackComponent = GetComponent<SlimerAttack>();
        if (attackComponent == null)
        {
            Debug.LogError("在Slimer中，没有找到Attack脚本！");
        }

        defenceComponent = GetComponent<DefenceEnemies>();
        if (defenceComponent == null)
        {
            Debug.LogError("在Slimer中，没有找到Defence脚本！");
        }
        //设置最大生命值
        defenceComponent.Initialize(3);
    }

    public override void MyUpdate()
    {
        DefenceCheck();
        PlayerCheck();
        MoveControl();
       // AttackControl();
    }
    private void PlayerCheck()
    {
        if (canCheck)
        {
            int face = movementComponent.faceRight ? 1 : -1;
            RaycastHit2D eyeCheck = Raycast(new Vector2(0f, 0f), new Vector2(face, 0), 7f, playerLayer);
            isSeePlayer = eyeCheck;

            if (isSeePlayer)
            {
                canCheck = false;
                eyeTime = Time.time + 1f;
                movementComponent.slimerAnim.SetBool("isAttacking", true);
            }
        }
        else
        {
            if (Time.time > eyeTime)
            {
                canCheck = true;
                movementComponent.slimerAnim.SetBool("isAttacking", false);
            }
        }
    }
    //移动流程
    private void MoveControl()
    {
        //将是否看见敌人的参数传给移动组件
        movementComponent.eyeSee = isSeePlayer;    
        movementComponent.RequestMoveByFrame(SlimerMovement.MovementMode.Normal);
        
    }
    private void DefenceCheck()
    {
        defenceComponent.AttackCheck();
        if (defenceComponent.getIsDead())
        {
            gameObject.SetActive(false);
        }
        defenceComponent.Clear();
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    //攻击流程
    //private void AttackControl()
    //{
    //   attackComponent

    //}
}