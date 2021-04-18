using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAbility : myUpdate, Ability
{
    private UpdateType type = UpdateType.Player;
    private int priorityInType = 6;

    private MovementPlayer movementComponent;

    //水盾
    private WaterShieldSpell waterShieldSpell;

    //激活该主元素，同时制定两个辅助元素
    public void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        //把冰主元素本身需要的资源激活

        this.enabled = true;
        //把指定辅助技能所需要的资源激活

        //水盾
        if (aElement == ElementAbilityManager.Element.Fire || bElement == ElementAbilityManager.Element.Fire)
        {
            waterShieldSpell.Enable();
        }
    }
    //休眠该主元素
    public void DisActivate()
    {
        waterShieldSpell.Disable();


        this.enabled = false;
    }

    public override void Initialize()
    {
        //获得场景物体引用
        movementComponent = GetComponent<MovementPlayer>();

        //初始化技能类
        waterShieldSpell = new WaterShieldSpell();
        waterShieldSpell.Initialize();
    }

    public override void MyUpdate()
    {
        waterShieldSpell.WaterShieldClock();
    }
    public void ShortSpell()
    {
        Debug.Log("冰短还未做好！");
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
        if (aElement == ElementAbilityManager.Element.Fire && bElement == ElementAbilityManager.Element.NULL ||
           aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Fire)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                waterShieldSpell.Cast();
            }
        }
    }
}
