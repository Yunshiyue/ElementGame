using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAbility : myUpdate, Ability
{
    private UpdateType type = UpdateType.Player;
    private int priorityInType = 7;

    private MovementPlayer movementComponent;

    private SightHead sightHead;
    private bool isCurCasting = false;
    private bool isLastCasting = false;

    private HurricaneSpell hurricaneSpell;
    private WindArrowSpell windArrowSpell;
    private BlinkBackSpell blinkBackSpell;
    private WIndShortSpell windShortSpell;
    private WindThunderSpell windThunderSpell;

    //激活该主元素，同时制定两个辅助元素
    public void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        this.enabled = true;
        //把风主元素本身需要的资源激活
        windShortSpell.Enable();
        //把指定辅助技能所需要的资源激活

        //飓风
        hurricaneSpell.Enable();


        if (aElement == ElementAbilityManager.Element.Fire || bElement == ElementAbilityManager.Element.Fire)
        {
            blinkBackSpell.Enable();
        }
        //风箭
        if (aElement == ElementAbilityManager.Element.Ice || bElement == ElementAbilityManager.Element.Ice)
        {
            windArrowSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Thunder || bElement == ElementAbilityManager.Element.Thunder)
        {
            windThunderSpell.Enable();
        }
    }
    //休眠该主元素
    public void DisActivate()
    {
        sightHead.gameObject.SetActive(false);
        hurricaneSpell.Disable();
        windArrowSpell.Disable();
        blinkBackSpell.Disable();
        windShortSpell.Disable();
        windThunderSpell.Disable();

        this.enabled = false;
    }

    public override void Initialize()
    {
        //获得场景物体引用
        movementComponent = GetComponent<MovementPlayer>();

        sightHead = GameObject.Find("SightHead").GetComponent<SightHead>();
        sightHead.gameObject.SetActive(false);

        //初始化技能类
        hurricaneSpell = new HurricaneSpell();
        hurricaneSpell.Initialize();

        windArrowSpell = new WindArrowSpell();
        windArrowSpell.Initialize();

        blinkBackSpell = new BlinkBackSpell();
        blinkBackSpell.Initialize();

        windShortSpell = new WIndShortSpell();
        windShortSpell.Initialize();

        windThunderSpell = new WindThunderSpell();
        windThunderSpell.Initialize();
    }

    public override void MyUpdate()
    {
        if(isCurCasting && !isLastCasting)
        {
            WindSightHeadOn();
        }
        else if(!isCurCasting && isLastCasting)
        {
            WindSightHeadOff();
        }
        isLastCasting = isCurCasting;
        isCurCasting = false;

        blinkBackSpell.BlinkBackClock();
    }
    public void ShortSpell()
    {
        Debug.Log("风短做好！");
        if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            windShortSpell.Cast();
        }
        
    }

    public void FullySpell(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        if (aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.NULL)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                hurricaneSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Fire && bElement == ElementAbilityManager.Element.NULL ||
                aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Fire)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                blinkBackSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Ice && bElement == ElementAbilityManager.Element.NULL ||
                 aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Ice)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                windArrowSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Thunder && bElement == ElementAbilityManager.Element.NULL ||
                 aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Thunder)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
            {
                windThunderSpell.SetTargetPosition(GetSightHeadPosition());
                windThunderSpell.Cast();
            }
        }
    }

    private void WindSightHeadOn()
    {
        //出现瞄准镜
        if (!sightHead.gameObject.activeSelf)
        {
            sightHead.gameObject.SetActive(true);
            sightHead.SetPosition(transform.position);
        }
    }
    private void WindSightHeadOff()
    {
        sightHead.gameObject.SetActive(false);
    }
    public Vector2 GetSightHeadPosition()
    {
        return new Vector2(sightHead.transform.position.x, sightHead.transform.position.y);
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
        isCurCasting = true;
        return movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Casting);
    }
}
