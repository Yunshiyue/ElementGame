using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IceAbility : myUpdate, Ability
{
    private UpdateType type = UpdateType.Player;
    private int priorityInType = 6;

    private bool isOn = true;
    private MovementPlayer movementComponent;

    //平台
    private PlatformJudge.Platfrom platform;

    //移动端需要的控件
    private Joystick joystick;
    /// <summary>
    /// 平台管理器
    /// </summary>
    private PlatformJudge platformJudge;


    //武器切换UI
    private GameObject weaponChangePanel;
    private WeaponChangeUI weaponChangeUI;

    //冰疗
    private IceHealSpell iceHealSpell;
    //水盾
    private WaterShieldSpell waterShieldSpell;

    //冰雷
    private IceThunderSpell iceThunderSpell;

    public enum IceWeapon { Sword, Arrow, Shield, Hammer}
    private IceWeapon weapon = IceWeapon.Sword;
    //冰墙
    private IceShieldSpell iceShieldSpell;
    //冰剑
    private IceSwordSpell iceSwordSpell;
    //冰箭
    private IceArrowSpell iceArrowSpell;
    //冰锤
    private IceHammerSpell iceHammerSpell;
    //冰风四技能
    //冰剑
    private IceBlinkSpell iceBlinkSpell;
    //冰箭
    private IceShotSpell iceShotSpell;
    //冰盾
    private IceShieldMashSpell iceSheildMashSpell;

    //冻结层级的filter
    private ContactFilter2D filter;
    ////冰层的计时器
    public static readonly float MAX_ICE_EXIST_TIME = 8f;
    private PoolManager poolManager;
    public static readonly string IceFreezingZoneName = "IceFreezingZone";
    //public readonly int MAX_ICE_EXIST_NUMBER = 32;
    //private LinkedList<int> freeClockArrayElement = new LinkedList<int>();
    //private 


    //激活该主元素，同时制定两个辅助元素
    public void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        //把冰主元素本身需要的资源激活

        this.isOn = true;
        iceShieldSpell.Enable();
        iceSwordSpell.Enable();
        iceArrowSpell.Enable();
        iceHealSpell.Enable();
        //把指定辅助技能所需要的资源激活

        //水盾
        if (aElement == ElementAbilityManager.Element.Fire || bElement == ElementAbilityManager.Element.Fire)
        {
            waterShieldSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Thunder || bElement == ElementAbilityManager.Element.Thunder)
        {
            iceThunderSpell.Enable();
        }
        if (aElement == ElementAbilityManager.Element.Wind || bElement == ElementAbilityManager.Element.Wind)
        {
            iceBlinkSpell.Enable();
            iceShotSpell.Enable();
        }

    }
    //休眠该主元素
    public void DisActivate()
    {
        waterShieldSpell.Disable();
        iceThunderSpell.Disable();
        iceShieldSpell.Disable();
        iceBlinkSpell.Disable();
        iceArrowSpell.Disable();
        iceShotSpell.Disable();
        this.isOn = false;

    }

    public override void Initialize()
    {
        //获得场景物体引用
        movementComponent = GetComponent<MovementPlayer>();
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();

        //武器切换UI
        weaponChangePanel = GameObject.Find("WeaponChangePanel");
        weaponChangeUI = weaponChangePanel.GetComponent<WeaponChangeUI>();
        weaponChangePanel.SetActive(false);

        //初始化技能类
        waterShieldSpell = new WaterShieldSpell();
        waterShieldSpell.Initialize();

        iceThunderSpell = new IceThunderSpell();
        iceThunderSpell.Initialize();

        iceShieldSpell = new IceShieldSpell();
        iceShieldSpell.Initialize();
        
        iceSwordSpell = new IceSwordSpell();
        iceSwordSpell.Initialize();

        iceHealSpell = new IceHealSpell();
        iceHealSpell.Initialize();
        iceHealSpell.Enable();

        iceBlinkSpell = new IceBlinkSpell();
        iceBlinkSpell.Initialize();

        iceArrowSpell = new IceArrowSpell();
        iceArrowSpell.Initialize();
        
        iceHammerSpell = new IceHammerSpell();
        iceHammerSpell.Initialize();

        iceShotSpell = new IceShotSpell();
        iceShotSpell.Initialize();

        iceSheildMashSpell = new IceShieldMashSpell();
        iceSheildMashSpell.Initialize();

        //以下为碰墙检测参数的初始化
        filter.useNormalAngle = false;
        filter.useDepth = false;
        filter.useOutsideDepth = false;
        filter.useOutsideNormalAngle = false;
        filter.useTriggers = false;

        filter.useLayerMask = true;

        LayerMask layerMask = 0;
        layerMask ^= 1 << LayerMask.NameToLayer("Water");

        filter.layerMask = layerMask;

        //判断平台
        platformJudge = GameObject.Find("ControllerMode").GetComponent<PlatformJudge>();
        if (platformJudge == null)
        {
            Debug.LogError("找不到ControllerMode");
        }
        platform = platformJudge.GetPlatform();
        if (platform == PlatformJudge.Platfrom.ANDROID || platform == PlatformJudge.Platfrom.IOS || platform == PlatformJudge.Platfrom.WEB_MOBILE)
        {
            joystick = GameObject.Find("Variable Joystick").GetComponent<Joystick>();
            if (joystick == null)
            {
                Debug.LogError("移动端获取摇杆失败！");
            }
        }
    }

    public override void MyUpdate()
    {
        if(isOn)
        {
            waterShieldSpell.WaterShieldClock();
            iceShotSpell.IceShotClock();
        }
    }
    public void ShortSpell()
    {
       
            switch (weapon)
            {
            case IceWeapon.Sword:
                if (movementComponent.RequestChangeControlStatus(IceSwordSpell.ICE_SWORD_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
                {
                    iceSwordSpell.Cast();
                }
                break;

            case IceWeapon.Arrow:
                if (movementComponent.RequestChangeControlStatus(IceArrowSpell.ICE_ARROW_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
                {
                    iceArrowSpell.Cast();
                }
                break;
            case IceWeapon.Shield:
                if (movementComponent.RequestChangeControlStatus(IceShieldSpell.ICE_SHIELD_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
                {
                    iceShieldSpell.Cast();
                }
                break;

            case IceWeapon.Hammer:
                if (movementComponent.RequestChangeControlStatus(IceHammerSpell.ICE_HAMMER_TIME , MovementPlayer.PlayerControlStatus.AbilityWithMovement))
                {
                    iceHammerSpell.Cast();
                }
                break;
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
        if (aElement == ElementAbilityManager.Element.Fire && bElement == ElementAbilityManager.Element.NULL ||
           aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Fire)
        {
            if (movementComponent.RequestChangeControlStatus(ElementAbilityManager.DEFALT_CASTING_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                waterShieldSpell.Cast();
            }
        }
        else if (aElement == ElementAbilityManager.Element.Thunder && bElement == ElementAbilityManager.Element.NULL ||
           aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Thunder)
        {
            if (movementComponent.RequestChangeControlStatus(IceThunderSpell.ICE_THUNDER_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                iceThunderSpell.Cast();
            }
        }

        else if (aElement == ElementAbilityManager.Element.Wind && bElement == ElementAbilityManager.Element.NULL ||
           aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.Wind)
        {
            if (movementComponent.RequestChangeControlStatus(IceShieldMashSpell.ICE_SHIELD_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
            {
                switch(weapon)
                {
                    case IceWeapon.Sword:
                        iceBlinkSpell.Cast();
                        break;
                    case IceWeapon.Arrow:
                        iceShotSpell.Cast();
                        break;
                    case IceWeapon.Shield:
                        iceSheildMashSpell.Cast();
                        break;
                }
            }
        }
    }

    public int NextAuxiliarySpellCost()
    {
        return 1;
    }

    public void AuxiliarySpell()
    {
        if (movementComponent.RequestChangeControlStatus(IceThunderSpell.ICE_THUNDER_TIME, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            iceHealSpell.Cast();
        }
    }

    public bool Casting(bool isFullySpelt, ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement)
    {
        if(movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Casting))
        {
            if(isFullySpelt && aElement == ElementAbilityManager.Element.NULL && bElement == ElementAbilityManager.Element.NULL)
            {
                //显示选择界面
                weaponChangePanel.SetActive(true);

                if (platform == PlatformJudge.Platfrom.PC || platform == PlatformJudge.Platfrom.WEB_PC)
                {
                    if (Input.GetKeyUp(KeyCode.A))
                    {
                        //切换到
                        weaponChangeUI.ChooseWeapon(IceWeapon.Shield);
                        weapon = IceWeapon.Shield;
                    }
                    else if (Input.GetKeyUp(KeyCode.D))
                    {
                        weaponChangeUI.ChooseWeapon(IceWeapon.Sword);
                        weapon = IceWeapon.Sword;
                    }
                    else if (Input.GetKeyUp(KeyCode.W))
                    {
                        weaponChangeUI.ChooseWeapon(IceWeapon.Arrow);
                        weapon = IceWeapon.Arrow;
                    }
                    else if (Input.GetKeyUp(KeyCode.S))
                    {
                        weaponChangeUI.ChooseWeapon(IceWeapon.Hammer);
                        weapon = IceWeapon.Hammer;
                    }
                }
                else
                {
                    float x = joystick.Horizontal;
                    float y = joystick.Vertical;
                    //左
                    if (x < -0.5f)
                    {
                        //切换到
                        weaponChangeUI.ChooseWeapon(IceWeapon.Shield);
                        weapon = IceWeapon.Shield;
                    }
                    //右
                    else if (x > 0.5f)
                    {
                        weaponChangeUI.ChooseWeapon(IceWeapon.Sword);
                        weapon = IceWeapon.Sword;
                    }
                    //上
                    else if (y > 0.5f)
                    {
                        weaponChangeUI.ChooseWeapon(IceWeapon.Arrow);
                        weapon = IceWeapon.Arrow;
                    }
                    //下
                    else if (y < -0.5f)
                    {
                        weaponChangeUI.ChooseWeapon(IceWeapon.Hammer);
                        weapon = IceWeapon.Hammer;
                    }
                }

            }
            else
            {
                //选择界面消失
                weaponChangePanel.SetActive(false);
            }
            return true;
        }
        return false;
    }

    public void FreezingZone(BoxCollider2D abilityCollider)
    {
        Debug.Log("进入了冰冻函数");
        Collider2D[] waterColliders = new Collider2D[8];
        if(abilityCollider.OverlapCollider(filter, waterColliders) != 0)
        {
            Debug.Log(string.Format("冻到了水域{0}", waterColliders[0].name));
            BoxCollider2D waterCollider = (BoxCollider2D)waterColliders[0];
            BoxCollider2D freezingCollider = poolManager.GetGameObject(IceFreezingZoneName).GetComponent<BoxCollider2D>();
            freezingCollider.gameObject.SetActive(true);
            freezingCollider.GetComponent<IceDisappear>().SetPoolManger(poolManager);
            ColliderBoundsCalculator.SetColliderSizeAndPositionByRect(freezingCollider, ColliderBoundsCalculator.GetColliderIntersection(abilityCollider, waterCollider));
        }
    }
}
