using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAbility : myUpdate, Ability
{
    private UpdateType type = UpdateType.Player;
    private int priorityInType = 4;

    private bool isOn = true;

    private MovementPlayer movementComponent;

    private FireBallSpell fireBallSpell;
    private MeteoriteSpell meteoriteSpell;
    private ProtectiveFireBallSpell protectiveFireBallSpell;
    private RocketPackSpell rocketPackSpell;
    private FireThunderSpell fireThunderSpell;
    private RemoteControlBombSpell remoteControlBombSpell;
    private LavaSpell lavaSpell;
    private MeteorShowerSpell meteorShowerSpell;
    private SelfExplosionSpell selfExplosionSpell;
    //激活该主元素，同时制定两个辅助元素
    public void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        //把火主元素本身需要的资源激活
        isOn = true;

        //把指定技能所需要的资源激活

        fireBallSpell.Enable();
        meteoriteSpell.Enable();

        //
        if (aElement == ElementAbilityManager.Element.Thunder || bElement == ElementAbilityManager.Element.Thunder)
        {
            fireThunderSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Ice || bElement == ElementAbilityManager.Element.Ice)
        {
            protectiveFireBallSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Wind || aElement == ElementAbilityManager.Element.Wind)
        {
            rocketPackSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Thunder && bElement == ElementAbilityManager.Element.Wind ||
            aElement == ElementAbilityManager.Element.Wind && bElement == ElementAbilityManager.Element.Thunder)
        {
            lavaSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Ice && bElement == ElementAbilityManager.Element.Wind ||
            aElement == ElementAbilityManager.Element.Wind && bElement == ElementAbilityManager.Element.Ice)
        {
            meteorShowerSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Ice && bElement == ElementAbilityManager.Element.Thunder ||
            aElement == ElementAbilityManager.Element.Thunder && bElement == ElementAbilityManager.Element.Ice)
        {
            selfExplosionSpell.Enable();
        }

    }
    //休眠该主元素
    public void DisActivate()
    {
        fireBallSpell.Disable();
        meteoriteSpell.Disable();
        fireThunderSpell.Disable();
        protectiveFireBallSpell.Disable();
        rocketPackSpell.Disable();
        lavaSpell.Disable();
        meteorShowerSpell.Disable();
        selfExplosionSpell.Disable();

        isOn = false;
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

        protectiveFireBallSpell = new ProtectiveFireBallSpell();
        protectiveFireBallSpell.Initialize();

        rocketPackSpell = new RocketPackSpell();
        rocketPackSpell.Initialize();

        fireThunderSpell = new FireThunderSpell();
        fireThunderSpell.Initialize();

        //辅助技能直接enable
        remoteControlBombSpell = new RemoteControlBombSpell();
        remoteControlBombSpell.Initialize();
        remoteControlBombSpell.Enable();

        lavaSpell = new LavaSpell();
        lavaSpell.Initialize();

        meteorShowerSpell = new MeteorShowerSpell();
        meteorShowerSpell.Initialize();

        selfExplosionSpell = new SelfExplosionSpell();
        selfExplosionSpell.Initialize();
    }

    public override void MyUpdate()
    {
        remoteControlBombSpell.BombClock();
        rocketPackSpell.RocketPackClock();
        meteorShowerSpell.MeteorShowerClock();
    }

    public void ShortSpell()
    {
        Debug.Log("请求火球！");
        if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME * 2f , MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            fireBallSpell.Cast();
            Debug.Log("扔出火球！");
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

    public void FullySpell(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        if (aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.NULL)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                meteoriteSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Thunder && bElement == ElementAbilityManager.Element.NULL ||
                 aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Thunder)
        {
             if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
             {
                fireThunderSpell.Cast();
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
                rocketPackSpell.Cast();
             }
        }
        else if (aElement == ElementAbilityManager.Element.Thunder && bElement == ElementAbilityManager.Element.Wind ||
                 aElement == ElementAbilityManager.Element.Wind && bElement == ElementAbilityManager.Element.Thunder)
        {
             if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
             {
                lavaSpell.Cast();
             }
        }
        else if (aElement == ElementAbilityManager.Element.Ice && bElement == ElementAbilityManager.Element.Wind ||
                 aElement == ElementAbilityManager.Element.Wind && bElement == ElementAbilityManager.Element.Ice)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                meteorShowerSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Ice && bElement == ElementAbilityManager.Element.Thunder ||
         aElement == ElementAbilityManager.Element.Thunder && bElement == ElementAbilityManager.Element.Ice)
        {
            if (movementComponent.RequestChangeControlStatus(SelfExplosionAttack.SELF_EXPLOSION__TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                selfExplosionSpell.Cast();
            }
        }
    }

    public int NextAuxiliarySpellCost()
    {
        return remoteControlBombSpell.GetNextSpellCost();
    }

    public void AuxiliarySpell()
    {
        if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            remoteControlBombSpell.Cast();
        }
    }

    public bool Casting(bool isFullySpelt, ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        return movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Casting);
    }
}
