﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAbility : myUpdate, Ability
{
    private UpdateType type = UpdateType.Player;
    private int priorityInType = 4;

    private MovementPlayer movementComponent;

    private FireBallSpell fireBallSpell;
    private MeteoriteSpell meteoriteSpell;
    private LavaSpell lavaSpell;
    private ProtectiveFireBallSpell protectiveFireBallSpell;

    //激活该主元素，同时制定两个辅助元素
    public void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        //把火主元素本身需要的资源激活
        this.enabled = true;

        //把指定辅助技能所需要的资源激活
        fireBallSpell.Enable();
        meteoriteSpell.Enable();

        //
        if(aElement == ElementAbilityManager.Element.Wind || aElement == ElementAbilityManager.Element.Wind)
        {
            lavaSpell.Enable();
        }
        if(aElement == ElementAbilityManager.Element.Ice || bElement == ElementAbilityManager.Element.Ice)
        {
            protectiveFireBallSpell.Enable();
        }
    }
    //休眠该主元素
    public void DisActivate()
    {
        fireBallSpell.Disable();
        meteoriteSpell.Disable();
        lavaSpell.Disable();

        this.enabled = false;
    }

    public override void Initialize()
    {
        //获得场景物体引用
        movementComponent = GetComponent<MovementPlayer>();

        //初始化技能类
        fireBallSpell = new FireBallSpell();
        fireBallSpell.Initialize();

        meteoriteSpell = new MeteoriteSpell();
        meteoriteSpell.Initialize();

        lavaSpell = new LavaSpell();
        lavaSpell.Initialize();

        protectiveFireBallSpell = new ProtectiveFireBallSpell();
        protectiveFireBallSpell.Initialize();
    }

    public override void MyUpdate()
    {
    }

    public void ShortSpell()
    {
        if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME , MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            fireBallSpell.Cast();
        }
    }
    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return type;
    }

    public bool Casting()
    {
        return movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Casting);
    }
    public void FullySpell(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        if (aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.NULL)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                meteoriteSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Ice && bElement == ElementAbilityManager.Element.NULL ||
                 aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Ice)
        {
             if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
             {
                 protectiveFireBallSpell.Cast();
             }
        }
        else if (aElement == ElementAbilityManager.Element.Wind && bElement == ElementAbilityManager.Element.NULL ||
                    aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Wind)
        {
             if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
             {
                 lavaSpell.Cast();
             }
        }
    }
}