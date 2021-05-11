using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ThunderAbility : myUpdate, Ability
{
    private UpdateType type = UpdateType.Player;
    private int priorityInType = 5;

    private MovementPlayer movementComponent;

    private bool isOn = false;
    private bool isInThunderWind = false;

    private GameObject thunderCircleObject;
    private SpriteRenderer thunderCircleRenderer;
    private ThunderCircle thunderCircleScript;

    //通过OnTriggerEnter和OnTriggerExit记录在雷圈中的敌人或者物体
    private List<GameObject> targetInThunderCircle = new List<GameObject>(MAX_ENMEY_NUMBER_IN_THUNDER_CIRCLE);

    private const int MAX_ENMEY_NUMBER_IN_THUNDER_CIRCLE = 32;

    //雷灵
    private ThunderElfSpell thunderElfSpell;
    //闪电链
    private ThunderLinkSpell thunderLinkSpell;
    //雷球
    private ThunderBallSpell thunderBallSpell;
    //雷长按
    private ThunderLongSpell thunderLongSpell;
    //雷冰
    private ThunderIceSpell thunderIceSpell;


    //激活该主元素，同时制定两个辅助元素
    public void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        isOn = true;
        //把雷主元素本身需要的资源激活
        thunderCircleRenderer.enabled = true;

        //把制定辅助技能所需要的资源激活
        //雷球
        thunderBallSpell.Enable();

        thunderLongSpell.Enable();

        //激活闪电链
        if(aElement == ElementAbilityManager.Element.Fire || bElement == ElementAbilityManager.Element.Fire)
        {
            thunderLinkSpell.Enable();
        }
        else if (aElement == ElementAbilityManager.Element.Ice || bElement == ElementAbilityManager.Element.Ice)
        {
            thunderIceSpell.Enable();
        }
    }
    //休眠该主元素
    public void DisActivate()
    {
        isOn = false;
        thunderCircleRenderer.enabled = false;
        targetInThunderCircle.Clear();

        //雷球
        thunderBallSpell.Disable();
        thunderLinkSpell.Disable();

        thunderLongSpell.Disable();
        thunderIceSpell.Disable();

    }

    public override void Initialize()
    {
        //获得场景物体引用
        thunderCircleObject = GameObject.Find("ThunderCircle");
        thunderCircleScript = thunderCircleObject.GetComponent<ThunderCircle>();
        thunderCircleRenderer = thunderCircleObject.GetComponent<SpriteRenderer>();
        if (thunderCircleScript == null)
        {
            Debug.LogError("在ThunderAbility中，没有找到雷圈脚本！");
        }

        movementComponent = GetComponent<MovementPlayer>();

        //初始化技能类
        thunderLinkSpell = new ThunderLinkSpell();
        thunderLinkSpell.Initialize();

        thunderBallSpell = new ThunderBallSpell();
        thunderBallSpell.Initialize();

        thunderLongSpell = new ThunderLongSpell();
        thunderLongSpell.Initialize();

        thunderIceSpell = new ThunderIceSpell();
        thunderIceSpell.Initialize();

        thunderElfSpell = new ThunderElfSpell();
        thunderElfSpell.Initialize();
        thunderElfSpell.Enable();
    }

    public override void MyUpdate()
    {
        if(isOn)
        {
            thunderLinkSpell.ThunderLinkClock();
            ThunderWindClock();
        }
        thunderElfSpell.ThunderElfClock();
    }
    public void ShortSpell()
    {
        if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME * 2.5f, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            thunderBallSpell.Cast();
        }
    }

    public void FullySpell(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        if (aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.NULL)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                thunderLongSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Fire && bElement == ElementAbilityManager.Element.NULL ||
                aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Fire)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                thunderLinkSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Ice && bElement == ElementAbilityManager.Element.NULL ||
                 aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Ice)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                thunderIceSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Wind && bElement == ElementAbilityManager.Element.NULL ||
                    aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Wind)
        {
             if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
             {
                 isInThunderWind = true;
                 thunderCircleScript.ExpendCircle();
             }
        }
    }
    public GameObject GetClosestTargetInList(List<GameObject> gameObjects)
    {
        float minDistance = float.MaxValue;
        float tempDistance;
        GameObject cloestTarget = null;
        foreach (GameObject target in gameObjects)
        {
            tempDistance = Vector2.Distance(target.transform.position, transform.position);
            if (tempDistance < minDistance)
            {
                minDistance = tempDistance;
                cloestTarget = target;
            }
        }
        return cloestTarget;
    }

    //雷圈相关的函数
    public List<GameObject> GetTargetInThunderCircle()
    {
        return targetInThunderCircle;
    }
    public void AddInThunderCircleTarget(GameObject target)
    {
        targetInThunderCircle.Add(target);
        printObjectInThunderCircle();
    }
    public void RemoveInThunderCircleTarget(GameObject target)
    {
        targetInThunderCircle.Remove(target);
        printObjectInThunderCircle();
    }
    StringBuilder lala = new StringBuilder(128);
    private void printObjectInThunderCircle()
    {
        lala.Append("在雷圈里有：");
        foreach (GameObject a in targetInThunderCircle)
        {
            lala.Append(a.name);
            lala.Append("-");
            lala.Append(a.GetInstanceID());
            lala.Append(" ");
        }
        Debug.Log(lala.ToString());
        lala.Clear();
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return type;
    }

    public int NextAuxiliarySpellCost()
    {
        return 1;
    }

    public void AuxiliarySpell()
    {
        if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            thunderElfSpell.Cast();
        }
    }

    public bool Casting(bool isFullySpelt, ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        return movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Casting);
    }
    private float curThunderWindTime = 0;
    private float totalThunderWindTime = 10f;
    private float curThunderAttackTime = 0;
    private float onceThunderAttackTime = 1f;
    private void ThunderWindClock()
    {
        if(isInThunderWind)
        {
            curThunderAttackTime += Time.deltaTime;
            if(curThunderAttackTime >= onceThunderAttackTime)
            {
                curThunderAttackTime = 0;
                thunderCircleScript.AttackThunderCircle();
            }

            curThunderWindTime += Time.deltaTime;
            if (curThunderWindTime >= totalThunderWindTime)
            {
                curThunderWindTime = 0;
                thunderCircleScript.LessenCircle();
                isInThunderWind = false;
            }
        }
    }
}
