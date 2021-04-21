//<<<<<<< HEAD
//﻿/**
// * @Description: 主角的技能释放组件，依赖CanFight组件，接收control脚本的控制信息，负责技能释放的具体实现。
// *               通过向移动组件requestChangeControlStatus的返回值来判读自己能够进行想要是放的技能。
// *               如果能够，可以向移动组件请求改变移动状态，同时应该向动画组间申请播放技能动画。
// * @Author: ridger

// *
// * @Description: 添加了火雷攻击方法，攻击击退效果
// * @Author: 夜里猛

// * 

// * @Editor: ridger
// * @Edit: 增加了扔回旋镖的方法，给定目的地扔回旋镖，用来测试风属性瞄准镜。
// * 

// * @Editor: ridger
// * @Edit: 1.将AttackPlayer继承自myUpdate，设计想法是技能组件可能记录一些需要计时性质的技能，比如buff等，所以需要
// *          逐帧调用来实现计时器等功能。
// *        2.实现了水盾技能，水盾能够抵挡两点攻击，有最大时间限制，消失后能够回复1点hp
// *          水盾计时器逻辑在MyUpdate中实现，动画效果在DefencePlayer组件中委托动画管理组件实现。
// *          

// * @Editor: ridger
// * @Edit: 实现了残影闪回技能，实现逻辑如下:
// *           a.blinkBackTime参数设定闪回到多少s之前的位置，blinkBackDetectTime决定每隔多少s就一次残影的位置
// *           b.每隔blinkBackDetectTime，会将当前位置记录在一个数组的最新位置处，并将对应的残影位置设置为这个位置
// *           c.调用闪回时，申请主动传送，传送至位置记录数组的下一个位置，即即将被更新的位置，也就是n秒之前的位置
// */
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Text;

//[RequireComponent(typeof(CanFight))]
//[RequireComponent(typeof(MovementPlayer))]
//public class AttackPlayer : myUpdate
//{

//    private Cinemachine.CinemachineImpulseSource MyInpulse;

//    private PoolManager poolManager;
//    private CanFight canFight;
//    private MovementPlayer movementComponent;

//    private Player.Element mainElement;
//    private Player.Element firstOtherElement;
//    private Player.Element secondOtherElement;

//    private int priorityInType = 3;
//    private UpdateType type = UpdateType.Player;

//    //当前技能编号 用于人物技能动画帧事件判断处理哪一个动画
//    private int currentSkill = 0;

//    //技能施法动画类型 普通、吟唱、发射
//    public enum SkillType { Normal, Singing, Push , Unstoppable , Null }

//    //技能的基本参数有：技能gameObjcet
//    //技能编号(待添加)
//    //攻击范围
//    //攻击伤害
//    //处于施法状态的时间
//    //技能打断敌人的时间
//    //技能击退的力

//    //普通攻击范围、伤害、字符串标识(名字)
//    private Collider2D normalAttackArea;
//    private int normalAttackDamage = 1;
//    public const string NORMAL_ATTACK_NAME = "NormalAttack";
//    //该攻击打断敌人参数
//    private float normalInterruptTime = 0.3f;
//    private Vector2 normalInterruptVector = new Vector2(3f, 0);

//    //雷圈
//    private Collider2D thunderCircleArea;
//    //通过OnTriggerEnter和OnTriggerExit记录在雷圈中的敌人或者物体
//    private List<GameObject> targetInThunderCircle;

//    //雷主元素
//    private GameObject thunderElement;
//    //雷主长按 只用于记录当前可命中的敌人有哪些，真正造成伤害由雷球个体实现
//    private GameObject thunderLongMain;
//    private Collider2D thunderLongMainArea;
//    private int thunderLongMainDamage = 1;
//    private float thunderLongMainInterruptTime = 0.5f;
//    private Vector2 thunderLongMainInterruptVector = new Vector2(2.5f, 0f);
//    private float thunderLongMainTime = 0.4f;
//    public const string THUNDER_LONG_MAIN_NAME = "ThunderLongMain";


//    //雷冰 技能范围就是雷圈的范围，禁锢效果则无击退
//    private GameObject thunderIce;
//    private GameObject thunderIceSpecial;//施放特效
//    private int thunderIceDamage = 1;
//    private float thunderIceInterruptTime = 1f;
//    private Vector2 thunderIceInterruptVector = new Vector2(0f, 0f);
//    private float thunderIceTime = 0.4f;

//    //冰雷
//    private GameObject iceThunder;
//    private GameObject leftIceThunder;
//    private GameObject rightIceThunder;
//    private Collider2D iceThunderArea;
//    private int iceThunderDamage = 1;
//    private float iceThunderInterruptTime = 1f;
//    private Vector2 iceThunderInterruptVector = new Vector2(0f, 0f);
//    private float iceThunderTime = 0.4f;

//    //冰盾
//    private GameObject iceShield;
//    private GameObject iceShieldColl;
//    private Collider2D iceShieldAttackArea;
//    private int iceShieldDamage = 1;
//    private float iceShieldUseTime = 0.3f;
//    private float iceShieldInterruptTime = 0.4f;
//    private float iceShieldContinueTime = 1f;
//    private float iceShieldForce = 3f;

//    //火雷
//    private GameObject fireThunder;
//    //火雷攻击范围、伤害、字符串标识(名字)
//    private PolygonCollider2D fireThunderAttackArea;
//    private int fireThunderAttackDamage = 1;
//    public const string FIRE_THUNDER_NAME = "FireThunder";
//    //该技能攻击打断敌人参数
//    private float fireThunderInterruptTime = 0.5f;
//    private Vector2 fireThunderInterruptVector = new Vector2(4f, 0);
//    //使用该技能上升距离
//    private Vector2 fireThunderVector = new Vector2(0, 0.5f);
//    private float fireThunderMovementTime = 0.2f;
   

//    //短按风主空气炮
//    private Collider2D windShortMainAttackArea;
//    private int windShortMainAttackDamage = 1;
//    public const string WIND_NAME = "Wind";
//    private float windShortMainStatusTime = 0.2f;
//    //该技能攻击打断敌人参数
//    private float windInterruptTime = 0.2f;
//    private Vector2 windInterruptVector = new Vector2(5f, 0);
//    //使用该技能自身后退距离
//    private Vector2 windVector = new Vector2(-0.15f, 0);
//    private GameObject windShortMain;

//    //风雷
//    private GameObject windThunder;
//    private int windThunderAttackDamage = 1;
//    public const string WIND_THUNDER_NAME = "WindThunder";
//    private float windThunderInterruptTime = 0.2f;
//    private float windThunderStatusTime = 0.2f;
//    private Vector2 windThunderInterruptVector = new Vector2(2f, 0);

//    //闪烁距离、字符串表示(名字)
//    private Vector2 blinkVector = new Vector2(5, 0);
//    public const string BLINK_NAME = "Blink";

//    //冲刺距离、时间
//    private Vector2 dashVector = new Vector2(2f, 0);
//    private float dashTime = 0.3f;
//    public const string DASH_NAME = "Dash";

//    private string targetLayerName = "Enemy";

//    //Dart技能
//    private Vector2 dartTargetPosition = new Vector2(0, 0);
//    private int dartDistance = 10;

//    //水盾技能
//    private float waterShieldCastingTime = 0.2f;
//    private float WSDurationTime = 8f;
//    private int waterShieldPoint = 2;
//    private int waterShieldHealPoint = 1;
//    private bool canWSClock = true;
//    private DefencePlayer defencePlayer;
//    private bool isWaterShieldClockOn = false;
//    private float WSCurTime = 0f;

//    //BlinkBack技能
//    //Time应该为DetectTime的整数倍才有意义
//    private float blinkBackTime = 3f;
//    private float blinkBackDetectTime = 0.1f;
//    private int BBVectorSize;
//    private Vector2[] blinkBackPositions;
//    private bool canBlinkBack = true;
//    private float BBCurTime = 0f;
//    private int BBCurPointer = 0;
//    private Transform[] shadowPositions;
//    private GameObject shadowParent;
//    private GameObject shadow;

//<<<<<<< HEAD:Assets/Script/Player/AttackPlayer.cs
//=======

////    //飞行技能参数
////    private float flyingAbilityCastTime = 0.05f;
////    private Vector3 flyingDirection = Vector3.zero;
////    private Vector2 flyingStartPositon = Vector2.zero;
//>>>>>>> 8bc6439572d56e629f62763bcf41c1279022b514:Assets/Script/Deprecated/AttackPlayer.cs

//    public override void Initialize()
//    {
//        //相机抖动
//        MyInpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();

//        canFight = GetComponent<CanFight>();
//        if(canFight == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
//        }

//        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
//        if (poolManager == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，找不到poolManager组件");
//        }

//        //使用string数组初始化canFight能够检测到的层
//        string[] targets = new string[1];
//        targets[0] = targetLayerName;
//        canFight.Initiailize(targets);

//        movementComponent = GetComponent<MovementPlayer>();
//        if(movementComponent == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
//        }

//        defencePlayer = GetComponent<DefencePlayer>();
//        if (defencePlayer == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，找不到defencePlayer组件！");
//        }

//        BBVectorSize = (int) (blinkBackTime / blinkBackDetectTime);
//        blinkBackPositions = new Vector2[BBVectorSize];
//        for(int i = 0; i < BBVectorSize; i++)
//        {
//            blinkBackPositions[i] = new Vector2(transform.position.x, transform.position.y);
//        }

//        shadowParent = GameObject.Find("Shadows");
//        if (shadowParent == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，没有找到Shadows");
//        }
//        shadow = GameObject.Find("Shadow");
//        if (shadowParent == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，没有找到Shadow");
//        }
//        shadowPositions = new Transform[BBVectorSize];
//        GameObject tempShadow;
//        for(int i = 0; i < BBVectorSize; i++)
//        {
//            tempShadow = Instantiate(shadow);
//            shadowPositions[i] = tempShadow.transform;
//            shadowPositions[i].SetParent(shadowParent.transform);
//            shadowPositions[i].position = blinkBackPositions[i];
//        }
//        shadow.SetActive(false);

//        normalAttackArea = GameObject.Find("NormalAttack").GetComponent<Collider2D>();
//        if (normalAttackArea == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，没有把普通攻击组件拖动到AttackPlayer脚本中！");
//        }

//        //加载雷圈 雷主技能
//        thunderElement = gameObject.transform.Find("Thunder").gameObject;
//        thunderCircleArea = GameObject.Find("ThunderCircle").GetComponent<Collider2D>();

//        targetInThunderCircle = new List<GameObject>(32);

//        //长按雷主的技能aoe范围
//        thunderLongMain = thunderElement.transform.Find("ThunderLongMain").gameObject;
//        thunderLongMain.transform.SetParent(null);//解除与父物体的关系，防止释放位置受影响
//        thunderLongMainArea = thunderLongMain.GetComponent<Collider2D>();
//        thunderLongMain.SetActive(false);

//        //雷冰
//        thunderIce= thunderElement.transform.Find("ThunderIce").gameObject;
//        thunderIceSpecial= thunderElement.transform.Find("ThunderIceSpecial").gameObject;
//        thunderIce.transform.SetParent(null);
//        thunderIce.SetActive(false);
//        thunderIceSpecial.SetActive(false);

//        //thunderElement.SetActive(false);

//        //之后根据属性搭配来进行判断 是否设为true
//        //冰雷
//        iceThunder = GameObject.Find("IceThunder");
//        leftIceThunder = iceThunder.transform.Find("LeftIceThunder").gameObject;
//        rightIceThunder = iceThunder.transform.Find("RightIceThunder").gameObject;
//        iceThunderArea = iceThunder.transform.Find("IceThunderCollider").GetComponent<Collider2D>();
//        leftIceThunder.SetActive(false);
//        rightIceThunder.SetActive(false);

//        //火雷
//        fireThunder = gameObject.transform.Find("FireThunder").gameObject;
//        fireThunderAttackArea = GameObject.Find("FireThunderCollider").GetComponent<PolygonCollider2D>();
//        fireThunder.SetActive(false);
//        if (fireThunderAttackArea == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，没有把火雷攻击碰撞组件拖动到AttackPlayer脚本中！");
//        }

//        //风，短按
//        windShortMain = gameObject.transform.Find("WindShortMain").gameObject;
//        windShortMainAttackArea = GameObject.Find("WindShortMainCollider").GetComponent<Collider2D>();
//        windShortMain.SetActive(false);
//        if (windShortMainAttackArea == null)
//        {
//            Debug.LogError("在" + gameObject.name + "中，没有把风主短按攻击碰撞组件拖动到AttackPlayer脚本中！");
//        }
//        windThunder = gameObject.transform.Find("WindThunder").gameObject;
//        windThunder.SetActive(false);

//        //冰盾
//        iceShield = gameObject.transform.Find("IceShield").gameObject;
//        iceShieldAttackArea = iceShield.GetComponentsInChildren<Collider2D>()[0];
//        iceShieldColl = iceShield.transform.Find("IceShieldCollider").gameObject;
//        iceShieldColl.transform.SetParent(null);
//        iceShield.SetActive(false);

//    }

//    public override void MyUpdate()
//    {
//        if (canWSClock)
//            WaterShieldClock();
//        if (canBlinkBack)
//            BlinkBackClock();
//        if (isThunderLink)
//            ThunderLinkClock();
//    }

//    public void ChangeElement(Player.Element mainElement, Player.Element aElement, Player.Element bElement)
//    {
//        this.mainElement = mainElement;
//        firstOtherElement = aElement;
//        secondOtherElement = bElement;
//        if(mainElement == Player.Element.Fire)
//        {
//            if(aElement == Player.Element.Icy || bElement == Player.Element.Icy)
//            {
//                canWSClock = true;
//            }
//        }
//        else if(mainElement == Player.Element.Wind)
//        {
//            if(aElement == Player.Element.Fire || bElement == Player.Element.Fire)
//            {
//                canBlinkBack = true;
//            }
//        }
//    }


//    //debug出攻击到的单位
//    StringBuilder targetsName = new StringBuilder();
//    CanBeFighted[] targets;
//    public void NormalAttack()
//    {
//        //动画处理
//        if(movementComponent.RequestChangeControlStatus(0.1f, MovementPlayer.PlayerControlStatus.AbilityNeedControl)){
//            movementComponent.playerAnim.SetUseSkillType((int)SkillType.Normal);//状态改变，动画机处理攻击动画
//        }
//        else
//        {
//            Debug.Log("普通攻击请求失败");
//        }   
//    }
//    //帧事件调用攻击
//    public void AttackEvent()
//    {
//        Debug.Log("AttackEvent");
//        //找到攻击命中的单位 canFight.AttackArea实现了攻击
//        targets = canFight.AttackArea(normalAttackArea, normalAttackDamage);
//        if (targets != null)
//        {
//            foreach (CanBeFighted a in targets)
//            {
//                targetsName.Append(" ");
//                targetsName.Append(a.gameObject.name);
//                //BeatBack(a, normalInterruptTime, normalInterruptVector);
//                a.BeatBack(transform, normalInterruptTime, normalInterruptVector);
//            }
//            Debug.Log("打到了： " + targetsName.ToString());
//            targetsName.Clear();
//        }
//    }

//    //技能帧事件:根据当前技能数决定执行哪个技能特效动画
//    public void SkillEvent()
//    {
//        MyInpulse.GenerateImpulse();
//        switch (currentSkill)
//        {
//            case 1:
//                //fireThunder.GetComponent<Animator>().SetBool("effect", true);
//                fireThunder.SetActive(true);
               
//                Debug.Log("状态：火雷");
//                break;
//            case 2:
//                //windShortMain.GetComponent<Animator>().SetBool("effect", true);
//                windShortMain.SetActive(true);
                
//                Debug.Log("状态：空气炮");
//                break;
//            case 3:
//                //情况与之前两者不太一样，此处用于检测可攻击物体来生成雷击aoe个体，在雷击个体中的动画帧事件攻击才生效
//                ThunderLongMainCheckEvent();
                
//                Debug.Log("状态：长按雷主");
//                break;
//            case 4:
//                //延时0.5s执行ThunderIceEvent
//                //thunderIceSpecial.GetComponent<Animator>().SetBool("effect", true);
//                thunderIceSpecial.SetActive(true);
//                Invoke("ThunderIceEvent", 0.5f);
//                Debug.Log("状态：雷冰");
//                break;
//            case 5:
//                //leftIceThunder.GetComponent<Animator>().SetBool("effect", true);
//                //rightIceThunder.GetComponent<Animator>().SetBool("effect", true);
//                leftIceThunder.SetActive(true);
//                rightIceThunder.SetActive(true);
//                Debug.Log("状态：冰雷");
//                break;
//            case 6:
//                //需确定位置出现
//                IceShieldAppearEvent();              
//                Debug.Log("状态：长按雷主");
//                break;

//        }
//    }

//    //ThunderLongMain() 雷 长按主 ;
//    //自动追踪的雷击，有一定延迟，在成小范围aoe伤害。
//    public void ThunderLongMain()
//    {
//        //动画处理
//        //征求施法时间
//        if (movementComponent.RequestChangeControlStatus(thunderLongMainTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
//        {
//                currentSkill = 3;
//                movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
//        }
//        else
//        {
//            Debug.Log("雷主长按请求失败");
//        }
//    }

//    //雷主长按施法动画调用帧事件 SkillEvent()再调用 ThunderLongMainCheckEvent() 用于找到可以击中的目标
//    public void ThunderLongMainCheckEvent()
//    {
//        GameObject target = GetClosestTargetInThunderCircle();
//        if(target != null)
//        {
//            //雷击启动
//            thunderLongMain.transform.position = target.transform.position;
//            thunderLongMain.SetActive(true);
//            Debug.Log("雷主长按打到的怪物： " + target.name);
//        }
//        else
//        {
//            Debug.Log("雷圈范围内没有敌人");
//        }

//        //找到可命中的单位 
//        //targets = canFight.AttackArea(thunderCircleArea, 0);//检测不造成伤害，伤害为0
//        //if (targets != null)
//        //{
//        //    //雷击在第一个目标对象处出现
//        //    thunderLongMain.transform.position = targets[0].transform.position;
//        //    //雷击启动
//        //    thunderLongMain.GetComponent<Animator>().SetBool("ready", true);
//        //    Debug.Log("雷主长按打到的怪物： " + targets[0].ToString());
//        //}
//        //else
//        //{
//        //    Debug.Log("雷圈范围内没有敌人");
//        //}
//    }
//    //雷主长按技能攻击事件
//    public void ThunderLongMainAttackEvent()
//    {
//        targets = canFight.AttackArea(thunderLongMainArea, thunderLongMainDamage);
//        if (targets != null)
//        {
//            foreach (CanBeFighted a in targets)
//            {
//                targetsName.Append(" ");
//                targetsName.Append(a.gameObject.name);
//                //击退效果
//                a.BeatBack(thunderLongMain.transform, thunderLongMainInterruptTime, thunderLongMainInterruptVector);
//            }
//            Debug.Log("打到了： " + targetsName.ToString());

//            targetsName.Clear();
//        }
//        else
//        {
//            Debug.Log("雷圈范围内没有敌人");
//        }
//    }
//    //雷冰 延迟0.5s释放，对雷圈内的所有敌人造成伤害，并施加短时间禁锢
//    public void ThunderIce()
//    {
//        //动画处理
//        //征求施法时间
//        if (movementComponent.RequestChangeControlStatus(thunderIceTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
//        {
//            currentSkill = 4;
//            movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
//        }
//        else
//        {
//            Debug.Log("雷冰请求失败");
//        }
//    }
//    public void ThunderIceEvent()
//    {
//        Debug.Log("ThunderIceEvent");
//        //找到攻击命中的单位 canFight.AttackArea实现了攻击
//        targets = canFight.AttackArea(thunderCircleArea, thunderIceDamage);
//        if (targets != null)
//        {
//            foreach (CanBeFighted a in targets)
//            {
//                targetsName.Append(" ");
//                targetsName.Append(a.gameObject.name);
//                //禁锢效果 无法移动
//                //a.BeatBack(transform, thunderIceInterruptTime, thunderIceInterruptVector);
//                a.Encompass(thunderIceInterruptTime);
//                GameObject temp = Instantiate(thunderIce, a.transform.position, Quaternion.identity);
//                //temp.GetComponent<Animator>().SetBool("effect", true);
//                temp.SetActive(true);
//            }
//            Debug.Log("打到了： " + targetsName.ToString());
//            targetsName.Clear();
//        }
//    }
 
//    //冰雷 对左右方向的敌人造成伤害，并冻结
//    public void IceThunder()
//    {
//        //动画处理
//        //征求施法时间
//        if (movementComponent.RequestChangeControlStatus(iceThunderTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
//        {
//            //movementComponent.playerAnim.SetUse(true);
//            currentSkill = 5;
//            movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
//            //吟唱动画播放
//        }
//        else
//        {
//            Debug.Log("雷冰请求失败");
//        }
//    }
//    //public void SetUseNo()
//    //{
//    //    movementComponent.playerAnim.SetUse(false);
//    //}
//    public void IceThunderEvent()
//    {
//        Debug.Log("IceThunderEvent");
//        //找到攻击命中的单位 canFight.AttackArea实现了攻击
//        targets = canFight.AttackArea(iceThunderArea, iceThunderDamage);
//        if (targets != null)
//        {
//            foreach (CanBeFighted a in targets)
//            {
//                targetsName.Append(" ");
//                targetsName.Append(a.gameObject.name);
//                //禁锢效果 无法移动
//                a.Encompass(iceThunderInterruptTime);
//            }
//            Debug.Log("打到了： " + targetsName.ToString());


//            targetsName.Clear();
//        }
//    }

//    private List<GameObject> thunderLinsHasAttackedList = new List<GameObject>(16);
//    private float thunderLinkIntervalTime = 0.25f;
//    private float thunderLinkCurTime = 0f;
//    private int thunderLinkAttackTotalTimes = 6;
//    private int thunderLinkAttackedTimes = 0;
//    private bool isThunderLink = false;
//    private List<GameObject> tempTargetsInThunderCircle = new List<GameObject>(32);
//    //闪电链
//    public void ThunderLink()
//    {
//        isThunderLink = true;
//    }
//    private void ThunderLinkClock()
//    {
//        if (thunderLinkAttackedTimes >= thunderLinkAttackTotalTimes)
//        {
//            isThunderLink = false;
//            thunderLinkCurTime = 0f;
//            thunderLinsHasAttackedList.Clear();
//        }

//        thunderLinkCurTime += Time.deltaTime;
//        if(thunderLinkCurTime >= thunderLinkIntervalTime)
//        {
//            thunderLinkCurTime = 0f;
//            thunderLinkAttackedTimes++;

//            GameObject target = GetClosestTargetInThunderCircle();
//            while(targetInThunderCircle.Count > 0)
//            {
//                if(thunderLinsHasAttackedList.Contains(target))
//                {
//                    targetInThunderCircle.Remove(target);
//                    tempTargetsInThunderCircle.Add(target);
//                    target = GetClosestTargetInThunderCircle();
//                }
//                else
//                {
//                    break;
//                }
//            }
//            //如果当前找不到敌人，则终止闪电链技能
//            if(target == null)
//            {
//                isThunderLink = false;
//                thunderLinkCurTime = 0f;
//                thunderLinsHasAttackedList.Clear();
//            }
//            else
//            {
//                //通知函数
//            }
//            foreach(GameObject targetToBeRemoved in tempTargetsInThunderCircle)
//            {
//                targetInThunderCircle.Add(targetToBeRemoved);
//            }
//            tempTargetsInThunderCircle.Clear();

//        }
//    }

//    //ThunderAndFire()使人物状态改变调用人物释放技能动画，其释放技能动画帧事件再根据SkillEvent()开启火雷技能动画

//    public void FireThunder()
//    {
//        //动画处理

//        if (movementComponent.RequestChangeControlStatus(fireThunderMovementTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
//        {
//            //释放该技能上升一小段距离
//            if (movementComponent.RequestMoveByTime(fireThunderVector, fireThunderMovementTime, MovementPlayer.MovementMode.Ability))
//            {
//                currentSkill = 1; 
//                movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
//            }
//        }
//        else
//        {
//            Debug.Log("火雷攻击请求失败");
//        }
//    }
    
//    //火雷动画帧事件调用fireThunderEvent()
//    public void FireThunderEvent()
//    {
//        Debug.Log("fireThunderEvent");
//        //找到攻击命中的单位 canFight.AttackArea实现了攻击
//        targets = canFight.AttackArea(fireThunderAttackArea, fireThunderAttackDamage);
//        if (targets != null)
//        {
//            foreach (CanBeFighted a in targets)
//            {
//                targetsName.Append(" ");
//                targetsName.Append(a.gameObject.name);
//               //击退效果
//                a.BeatBack(transform, fireThunderInterruptTime, fireThunderInterruptVector);
//            }
//            Debug.Log("打到了： " + targetsName.ToString());

//            targetsName.Clear();
//        }
//    }

//    //风 短按主
//    //空气炮：
//    //对怪物、物体：击退
//    //对自己：反作用力
//    public void WindShortMain()
//    {
//        //动画处理
//        if (movementComponent.RequestChangeControlStatus(windShortMainStatusTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
//        {
//            //释放该技能后退一小段距离
//            //windVector.x *= transform.localScale.x;
//            if (movementComponent.RequestMoveByTime(windVector, windShortMainStatusTime, MovementPlayer.MovementMode.Ability))
//            {
//                currentSkill = 2;
//                //movementComponent.playerAnim.SetAbilityNum(currentSkill);//状态改变，动画机处理攻击动画   
//                movementComponent.playerAnim.SetUseSkillType((int)SkillType.Push);
//            }
//        }
//        else
//        {
//            Debug.Log("空气炮攻击请求失败");
//        }
//    }
//    public void WindShortMainEvent()
//    {
//        Debug.Log("WindShortMainEvent");
//        //找到攻击命中的单位 canFight.AttackArea实现了攻击
//        targets = canFight.AttackArea(windShortMainAttackArea, windShortMainAttackDamage);
//        if (targets != null)
//        {
//            foreach (CanBeFighted a in targets)
//            {
//                targetsName.Append(" ");
//                targetsName.Append(a.gameObject.name);
//                //击退效果
//                //BeatBack(a, windInterruptTime, windInterruptVector);
//                a.BeatBack(transform, windInterruptTime, windInterruptVector);
//            }
//            Debug.Log("打到了： " + targetsName.ToString());

//            targetsName.Clear();
//        }
//    }

//    private float windThunderDistance = 7f;
//    private bool unstoppable = false;//冲刺时是否有伤害
//    //风雷
//    //朝向瞄准方向(默认朝向方向)进行一段冲刺，对途径的敌人造成雷电伤害
//    public void WindThunder(Vector2 position)
//    {
//        Vector2 temp = transform.position;
//        Vector2 targetPosition = position - temp;
//        if (targetPosition.magnitude > windThunderDistance)//冲刺最长距离为7
//        {
//            targetPosition.Normalize();
//            targetPosition *= windThunderDistance;
//        }

//        movementComponent.SetGravity(false);

//        if(movementComponent.IsFacingLeft())
//        {
//            targetPosition.x = - targetPosition.x;
//        }

//        unstoppable = true;
//        Invoke("UnstoppedEnd", 0.3f);
//        if (movementComponent.RequestChangeControlStatus(windThunderStatusTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
//        {
//            movementComponent.RequestMoveByTime(targetPosition, windThunderStatusTime, MovementPlayer.MovementMode.Ability);
//        }
//        windThunder.SetActive(true);
//    }
//    private void UnstoppedEnd()
//    {
//        unstoppable = false;
//    }
//    //风雷冲刺碰撞伤害
//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (unstoppable)
//        {
//            Debug.Log("unstoppable");
//            if(LayerMask.LayerToName(collision.gameObject.layer) == "Enemy")
//            {
//                Debug.Log("unstoppable!!!!");
//                Transform enemy = collision.transform;
//                canFight.Attack(enemy.GetComponent<CanBeFighted>(), windThunderAttackDamage, AttackInterruptType.WEAK);
//                enemy.GetComponent<CanBeFighted>().BeatBack(transform, windThunderInterruptTime, windThunderInterruptVector);            
//            }
//        }
//    }
//    //冰盾
//    public void IceShield()
//    {
//        if (movementComponent.RequestChangeControlStatus(iceShieldUseTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
//        {
//            if (iceShield.activeSelf)//如果之前还有冰墙则销毁
//            {
//                iceShield.SetActive(false);
//            }
            
//            currentSkill = 6;
//            movementComponent.playerAnim.SetUseSkillType((int)SkillType.Push);
//        }
//    } 

//    public void IceShieldAppearEvent()
//    {
//        //暂禁用冰盾碰撞体
//        if (iceShieldColl.activeSelf) iceShieldColl.SetActive(false);

//        iceShieldColl.transform.position = new Vector3((float)(transform.localScale.x * 1.5 + transform.position.x), (float)(transform.position.y + 0.05), 0);
//        iceShield.SetActive(true);
         
//    }
//    public void IceShieldAttackEvent()
//    {
//        Debug.Log("IceShieldAttackEvent");
//        //找到攻击命中的单位 canFight.AttackArea实现了攻击
//        targets = canFight.AttackArea(iceShieldAttackArea, iceShieldDamage);
//        if (targets != null)
//        {
//            foreach (CanBeFighted a in targets)
//            {
//                targetsName.Append(" ");
//                targetsName.Append(a.gameObject.name);
//                //击退效果 
//                a.BeatBack(transform,iceShieldInterruptTime,iceShieldForce);
//                //a.BeatBack(transform,iceShieldUseTime, iceShieldVector);
//            }
//            Debug.Log("打到了： " + targetsName.ToString());

//            targetsName.Clear();
//        }
//        //冰盾碰撞体可用
//        iceShieldColl.SetActive(true);
//    }

//    public void blink()
//    {
//        if(movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.AbilityNeedControl) )
//        {
//            movementComponent.RequestMoveByFrame(blinkVector, MovementPlayer.MovementMode.Ability, Space.Self);
//        }
//    }

//    public void dash()
//    {
//        if(movementComponent.RequestChangeControlStatus(dashTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
//        {
//            movementComponent.RequestMoveByTime(dashVector, dashTime, MovementPlayer.MovementMode.Ability);
//        }
//    }

//    public void Dart()
//    {
//        if (movementComponent.RequestChangeControlStatus(0.05f, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
//        {
//            GameObject dart = poolManager.GetGameObject("Dart", gameObject.transform.position);

//            //修改位置
//            dartTargetPosition.x = gameObject.transform.position.x + dartDistance * gameObject.transform.localScale.x;
//            dartTargetPosition.y = gameObject.transform.position.y;
//=======
//﻿///**
//// * @Description: 主角的技能释放组件，依赖CanFight组件，接收control脚本的控制信息，负责技能释放的具体实现。
//// *               通过向移动组件requestChangeControlStatus的返回值来判读自己能够进行想要是放的技能。
//// *               如果能够，可以向移动组件请求改变移动状态，同时应该向动画组间申请播放技能动画。
//// * @Author: ridger

//// *
//// * @Description: 添加了火雷攻击方法，攻击击退效果
//// * @Author: 夜里猛

//// * 

//// * @Editor: ridger
//// * @Edit: 增加了扔回旋镖的方法，给定目的地扔回旋镖，用来测试风属性瞄准镜。
//// * 

//// * @Editor: ridger
//// * @Edit: 1.将AttackPlayer继承自myUpdate，设计想法是技能组件可能记录一些需要计时性质的技能，比如buff等，所以需要
//// *          逐帧调用来实现计时器等功能。
//// *        2.实现了水盾技能，水盾能够抵挡两点攻击，有最大时间限制，消失后能够回复1点hp
//// *          水盾计时器逻辑在MyUpdate中实现，动画效果在DefencePlayer组件中委托动画管理组件实现。
//// *          

//// * @Editor: ridger
//// * @Edit: 实现了残影闪回技能，实现逻辑如下:
//// *           a.blinkBackTime参数设定闪回到多少s之前的位置，blinkBackDetectTime决定每隔多少s就一次残影的位置
//// *           b.每隔blinkBackDetectTime，会将当前位置记录在一个数组的最新位置处，并将对应的残影位置设置为这个位置
//// *           c.调用闪回时，申请主动传送，传送至位置记录数组的下一个位置，即即将被更新的位置，也就是n秒之前的位置
//// *

//// * @Editor: ridger
//// * @Edit: 实现了闪电链，逻辑如下：
//// *           a.闪电链每隔interval秒，就会通过算法从雷圈中找到最近的没有攻击过的敌人，并把这个敌人的引用set给闪电链物体
//// *           b.闪电链物体在Set后就会寻找敌人，在飞到敌人周围后对敌人造成伤害，并待机等待下一次Set敌人
//// *           c.查看是否攻击次数达到上限，没有则回到a，重新寻敌
//// */
////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;
////using System.Text;

////[RequireComponent(typeof(CanFight))]
////[RequireComponent(typeof(MovementPlayer))]
////public class AttackPlayer : myUpdate
////{

////    private PoolManager poolManager;
////    private CanFight canFight;
////    private MovementPlayer movementComponent;

////    private Player.Element mainElement;
////    private Player.Element firstOtherElement;
////    private Player.Element secondOtherElement;

////    private int priorityInType = 3;
////    private UpdateType type = UpdateType.Player;

////    //当前技能编号 用于人物技能动画帧事件判断处理哪一个动画
////    private int currentSkill = 0;

////    //技能施法动画类型 普通、吟唱、发射
////    public enum SkillType { Normal, Singing, Push }

////    //技能的基本参数有：技能gameObjcet
////    //技能编号(待添加)
////    //攻击范围
////    //攻击伤害
////    //处于施法状态的时间
////    //技能打断敌人的时间
////    //技能击退的力

////    //普通攻击范围、伤害、字符串标识(名字)
////    private Collider2D normalAttackArea;
////    private int normalAttackDamage = 1;
////    public const string NORMAL_ATTACK_NAME = "NormalAttack";
////    //该攻击打断敌人参数
////    private float normalInterruptTime = 0.3f;
////    private Vector2 normalInterruptVector = new Vector2(3f, 0);

////    //雷圈
////    private Collider2D thunderCircleArea;
////    //通过OnTriggerEnter和OnTriggerExit记录在雷圈中的敌人或者物体
////    private List<GameObject> targetInThunderCircle;

////    //雷主元素
////    private GameObject thunderElement;
////    //雷主长按 只用于记录当前可命中的敌人有哪些，真正造成伤害由雷球个体实现
////    private GameObject thunderLongMain;
////    private Collider2D thunderLongMainArea;
////    private int thunderLongMainDamage = 1;
////    private float thunderLongMainInterruptTime = 0.5f;
////    private Vector2 thunderLongMainInterruptVector = new Vector2(2.5f, 0f);
////    private float thunderLongMainTime = 0.4f;
////    public const string THUNDER_LONG_MAIN_NAME = "ThunderLongMain";


////    //雷冰 技能范围就是雷圈的范围，禁锢效果则无击退
////    private GameObject thunderIce;
////    private GameObject thunderIceSpecial;//施放特效
////    private int thunderIceDamage = 1;
////    private float thunderIceInterruptTime = 1f;
////    private Vector2 thunderIceInterruptVector = new Vector2(0f, 0f);
////    private float thunderIceTime = 0.4f;

////    //冰雷
////    private GameObject iceThunder;
////    private GameObject leftIceThunder;
////    private GameObject rightIceThunder;
////    private Collider2D iceThunderArea;
////    private int iceThunderDamage = 1;
////    private float iceThunderInterruptTime = 1f;
////    private Vector2 iceThunderInterruptVector = new Vector2(0f, 0f);
////    private float iceThunderTime = 0.4f;

////    //火雷
////    private GameObject fireThunder;
////    //火雷攻击范围、伤害、字符串标识(名字)
////    private PolygonCollider2D fireThunderAttackArea;
////    private int fireThunderAttackDamage = 1;
////    public const string FIRE_THUNDER_NAME = "FireThunder";

////    //该技能攻击打断敌人参数
////    private float fireThunderInterruptTime = 0.5f;
////    private Vector2 fireThunderInterruptVector = new Vector2(4f, 0);
////    //使用该技能上升距离
////    private Vector2 fireThunderVector = new Vector2(0, 0.5f);
////    private float fireThunderMovementTime = 0.2f;
   

////    //短按风主空气炮
////    private Collider2D windShortMainAttackArea;
////    private int windShortMainAttackDamage = 1;
////    public const string WIND_NAME = "Wind";
////    //该技能攻击打断敌人参数
////    private float windInterruptTime = 0.2f;
////    private Vector2 windInterruptVector = new Vector2(5f, 0);
////    //使用该技能自身后退距离
////    private Vector2 windVector = new Vector2(-0.15f, 0);
////    private GameObject windShortMain;

////    //闪烁距离、字符串表示(名字)
////    private Vector2 blinkVector = new Vector2(5, 0);
////    public const string BLINK_NAME = "Blink";

////    //冲刺距离、时间
////    private Vector2 dashVector = new Vector2(2f, 0);
////    private float dashTime = 0.3f;
////    public const string DASH_NAME = "Dash";

////    private string targetLayerName = "Enemy";

////    //Dart技能
////    private Vector2 dartTargetPosition = new Vector2(0, 0);
////    private int dartDistance = 10;

////    //水盾技能
////    private float waterShieldCastingTime = 0.2f;
////    private float WSDurationTime = 8f;
////    private int waterShieldPoint = 2;
////    private int waterShieldHealPoint = 1;
////    private bool canWSClock = true;
////    private DefencePlayer defencePlayer;
////    private bool isWaterShieldClockOn = false;
////    private float WSCurTime = 0f;

////    //BlinkBack技能
////    //Time应该为DetectTime的整数倍才有意义
////    private float blinkBackTime = 3f;
////    private float blinkBackDetectTime = 0.1f;
////    private int BBVectorSize;
////    private Vector2[] blinkBackPositions;
////    private bool canBlinkBack = true;
////    private float BBCurTime = 0f;
////    private int BBCurPointer = 0;
////    private Transform[] shadowPositions;
////    private GameObject shadowParent;
////    private GameObject shadow;

////    //飞行技能参数
////    private float flyingAbilityCastTime = 0.05f;
////    private Vector3 flyingDirection = Vector3.zero;
////    private Vector2 flyingStartPositon = Vector2.zero;

////    public override void Initialize()
////    {
////        canFight = GetComponent<CanFight>();
////        if(canFight == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
////        }

////        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
////        if (poolManager == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，找不到poolManager组件");
////        }

////        //使用string数组初始化canFight能够检测到的层
////        string[] targets = new string[1];
////        targets[0] = targetLayerName;
////        canFight.Initiailize(targets);

////        movementComponent = GetComponent<MovementPlayer>();
////        if(movementComponent == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
////        }

////        defencePlayer = GetComponent<DefencePlayer>();
////        if (defencePlayer == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，找不到defencePlayer组件！");
////        }

////        BBVectorSize = (int) (blinkBackTime / blinkBackDetectTime);
////        blinkBackPositions = new Vector2[BBVectorSize];
////        for(int i = 0; i < BBVectorSize; i++)
////        {
////            blinkBackPositions[i] = new Vector2(transform.position.x, transform.position.y);
////        }

////        shadowParent = GameObject.Find("Shadows");
////        if (shadowParent == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，没有找到Shadows");
////        }
////        shadow = GameObject.Find("Shadow");
////        if (shadowParent == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，没有找到Shadow");
////        }
////        shadowPositions = new Transform[BBVectorSize];
////        GameObject tempShadow;
////        for(int i = 0; i < BBVectorSize; i++)
////        {
////            tempShadow = Instantiate(shadow);
////            shadowPositions[i] = tempShadow.transform;
////            shadowPositions[i].SetParent(shadowParent.transform);
////            shadowPositions[i].position = blinkBackPositions[i];
////        }
////        shadow.SetActive(false);

////        normalAttackArea = GameObject.Find("NormalAttack").GetComponent<Collider2D>();
////        if (normalAttackArea == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，没有把普通攻击组件拖动到AttackPlayer脚本中！");
////        }

////        //加载雷圈 雷主技能
////        thunderElement = gameObject.transform.Find("Thunder").gameObject;
////        thunderElement.SetActive(true);
////        thunderCircleArea = GameObject.Find("ThunderCircle").GetComponent<Collider2D>();

////        //加载闪电链物体
////        thunderLinkObject = GameObject.Find("ThunderLink").GetComponent<ThunderLink>();
////        thunderLinkObject.gameObject.SetActive(false);
////        thunderLinkObject.SetCanFight(canFight);

////        targetInThunderCircle = new List<GameObject>(32);

////        //长按雷主的技能aoe范围
////        thunderLongMain = thunderElement.transform.Find("ThunderLongMain").gameObject;
////        thunderLongMain.transform.SetParent(null);//解除与父物体的关系，防止释放位置受影响
////        thunderLongMainArea = thunderLongMain.GetComponent<Collider2D>();

////        //雷冰
////        thunderIce= thunderElement.transform.Find("ThunderIce").gameObject;
////        thunderIceSpecial= thunderElement.transform.Find("ThunderIceSpecial").gameObject;
////        thunderIce.transform.SetParent(null);

////        //之后根据属性搭配来进行判断 是否设为true
////        //冰雷
////        iceThunder = gameObject.transform.Find("IceThunder").gameObject;
////        iceThunder.SetActive(true);
////        leftIceThunder = iceThunder.transform.Find("LeftIceThunder").gameObject;
////        //rightIceThunder = iceThunder.transform.Find("RightIceThunder").gameObject;
////        iceThunderArea = iceThunder.transform.Find("IceThunderCollider").GetComponent<Collider2D>();

////        //火雷
////        fireThunder = gameObject.transform.Find("FireThunder").gameObject;
////        fireThunder.SetActive(true);
////        fireThunderAttackArea = GameObject.Find("FireThunderCollider").GetComponent<PolygonCollider2D>();
////        if (fireThunderAttackArea == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，没有把火雷攻击碰撞组件拖动到AttackPlayer脚本中！");
////        }

////        //风，短按
////        windShortMain = gameObject.transform.Find("WindShortMain").gameObject;
////        windShortMain.SetActive(true);
////        windShortMainAttackArea = GameObject.Find("WindShortMainCollider").GetComponent<Collider2D>();
////        if (windShortMainAttackArea == null)
////        {
////            Debug.LogError("在" + gameObject.name + "中，没有把风主短按攻击碰撞组件拖动到AttackPlayer脚本中！");
////        }
////    }

////    public override void MyUpdate()
////    {
////        if (canWSClock)
////            WaterShieldClock();
////        if (canBlinkBack)
////            BlinkBackClock();
////        if (isThunderLink)
////            ThunderLinkClock();
////    }

////    public void ChangeElement(Player.Element mainElement, Player.Element aElement, Player.Element bElement)
////    {
////        this.mainElement = mainElement;
////        firstOtherElement = aElement;
////        secondOtherElement = bElement;
////        if(mainElement == Player.Element.Fire)
////        {
////            if(aElement == Player.Element.Ice || bElement == Player.Element.Ice)
////            {
////                canWSClock = true;
////            }
////        }
////        else if(mainElement == Player.Element.Wind)
////        {
////            if(aElement == Player.Element.Fire || bElement == Player.Element.Fire)
////            {
////                canBlinkBack = true;
////            }
////        }
////    }


////    //debug出攻击到的单位
////    StringBuilder targetsName = new StringBuilder();
////    CanBeFighted[] targets;
////    public void NormalAttack()
////    {
////        //动画处理
////        if(movementComponent.RequestChangeControlStatus(0.1f, MovementPlayer.PlayerControlStatus.AbilityNeedControl)){
////            movementComponent.playerAnim.SetUseSkillType((int)SkillType.Normal);//状态改变，动画机处理攻击动画
////        }
////        else
////        {
////            Debug.Log("普通攻击请求失败");
////        }   
////    }
////    //帧事件调用攻击
////    public void AttackEvent()
////    {
////        Debug.Log("AttackEvent");
////        //找到攻击命中的单位 canFight.AttackArea实现了攻击
////        targets = canFight.AttackArea(normalAttackArea, normalAttackDamage);
////        if (targets != null)
////        {
////            foreach (CanBeFighted a in targets)
////            {
////                targetsName.Append(" ");
////                targetsName.Append(a.gameObject.name);
////                //BeatBack(a, normalInterruptTime, normalInterruptVector);
////                a.BeatBack(transform, normalInterruptTime, normalInterruptVector);
////            }
////            Debug.Log("打到了： " + targetsName.ToString());
////            targetsName.Clear();
////        }
////    }

////    //技能帧事件:根据当前技能数决定执行哪个技能特效动画
////    public void SkillEvent()
////    {
////        switch (currentSkill)
////        {
////            case 1:
////                fireThunder.GetComponent<Animator>().SetBool("effect", true);
////                Debug.Log("状态：火雷");
////                break;
////            case 2:
////                windShortMain.GetComponent<Animator>().SetBool("effect", true);
////                Debug.Log("状态：空气炮");
////                break;
////            case 3:
////                //情况与之前两者不太一样，此处用于生成雷击aoe个体，在雷击个体中的动画帧事件攻击生效
////                ThunderLongMainCheckEvent();
////                Debug.Log("状态：长按雷主");
////                break;
////            case 4:
////                //延时0.5s执行ThunderIceEvent
////                thunderIceSpecial.GetComponent<Animator>().SetBool("effect", true);
////                Invoke("ThunderIceEvent", 0.5f);
////                Debug.Log("状态：雷冰");
////                break;
////            case 5:
////                leftIceThunder.GetComponent<Animator>().SetBool("effect", true);
////                rightIceThunder.GetComponent<Animator>().SetBool("effect", true);
////                Debug.Log("状态：冰雷");
////                break;
////        }
////    }

////    //ThunderLongMain() 雷 长按主 ;
////    //自动追踪的雷击，有一定延迟，在成小范围aoe伤害。
////    public void ThunderLongMain()
////    {
////        //动画处理
////        //征求施法时间
////        if (movementComponent.RequestChangeControlStatus(thunderLongMainTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////                currentSkill = 3;
////                movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
////        }
////        else
////        {
////            Debug.Log("雷主长按请求失败");
////        }
////    }

////    //雷主长按施法动画调用帧事件 SkillEvent()再调用 ThunderLongMainCheckEvent() 用于找到可以击中的目标
////    public void ThunderLongMainCheckEvent()
////    {
////        GameObject target = GetClosestTargetInList(targetInThunderCircle);
////        if(target != null)
////        {
////            //雷击启动
////            thunderLongMain.transform.position = target.transform.position;
////            thunderLongMain.GetComponent<Animator>().SetBool("ready", true);
////            Debug.Log("雷主长按打到的怪物： " + target.name);
////        }
////        else
////        {
////            Debug.Log("雷圈范围内没有敌人");
////        }

////        //找到可命中的单位 
////        //targets = canFight.AttackArea(thunderCircleArea, 0);//检测不造成伤害，伤害为0
////        //if (targets != null)
////        //{
////        //    //雷击在第一个目标对象处出现
////        //    thunderLongMain.transform.position = targets[0].transform.position;
////        //    //雷击启动
////        //    thunderLongMain.GetComponent<Animator>().SetBool("ready", true);
////        //    Debug.Log("雷主长按打到的怪物： " + targets[0].ToString());
////        //}
////        //else
////        //{
////        //    Debug.Log("雷圈范围内没有敌人");
////        //}
////    }
////    //雷主长按技能攻击事件
////    public void ThunderLongMainAttackEvent()
////    {
////        targets = canFight.AttackArea(thunderLongMainArea, thunderLongMainDamage);
////        if (targets != null)
////        {
////            foreach (CanBeFighted a in targets)
////            {
////                targetsName.Append(" ");
////                targetsName.Append(a.gameObject.name);
////                //击退效果
////                a.BeatBack(thunderLongMain.transform, thunderLongMainInterruptTime, thunderLongMainInterruptVector);
////            }
////            Debug.Log("打到了： " + targetsName.ToString());

////            targetsName.Clear();
////        }
////        else
////        {
////            Debug.Log("雷圈范围内没有敌人");
////        }
////    }
////    //雷冰 延迟0.5s释放，对雷圈内的所有敌人造成伤害，并施加短时间禁锢
////    public void ThunderIce()
////    {
////        //动画处理
////        //征求施法时间
////        if (movementComponent.RequestChangeControlStatus(thunderIceTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            currentSkill = 4;
////            movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
////        }
////        else
////        {
////            Debug.Log("雷冰请求失败");
////        }
////    }
////    public void ThunderIceEvent()
////    {
////        Debug.Log("ThunderIceEvent");
////        //找到攻击命中的单位 canFight.AttackArea实现了攻击
////        targets = canFight.AttackArea(thunderCircleArea, thunderIceDamage);
////        if (targets != null)
////        {
////            foreach (CanBeFighted a in targets)
////            {
////                targetsName.Append(" ");
////                targetsName.Append(a.gameObject.name);
////                //禁锢效果 无法移动
////                a.BeatBack(transform, thunderIceInterruptTime, thunderIceInterruptVector);
////                //位置有点点差距，如何让敌人立刻不移动？
////                GameObject temp = Instantiate(thunderIce, a.transform.position, Quaternion.identity);
////                temp.GetComponent<Animator>().SetBool("effect", true);
////            }
////            Debug.Log("打到了： " + targetsName.ToString());
////            targetsName.Clear();
////        }
////    }
 
////    //冰雷 对左右方向的敌人造成伤害，并冻结
////    public void IceThunder()
////    {
////        //动画处理
////        //征求施法时间
////        if (movementComponent.RequestChangeControlStatus(iceThunderTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
////        {
////            //movementComponent.playerAnim.SetUse(true);
////            currentSkill = 5;
////            movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
////            //吟唱动画播放
////        }
////        else
////        {
////            Debug.Log("雷冰请求失败");
////        }
////    }
////    //public void SetUseNo()
////    //{
////    //    movementComponent.playerAnim.SetUse(false);
////    //}
////    public void IceThunderEvent()
////    {
////        Debug.Log("IceThunderEvent");
////        //找到攻击命中的单位 canFight.AttackArea实现了攻击
////        targets = canFight.AttackArea(iceThunderArea, iceThunderDamage);
////        if (targets != null)
////        {
////            foreach (CanBeFighted a in targets)
////            {
////                targetsName.Append(" ");
////                targetsName.Append(a.gameObject.name);
////                //禁锢效果 无法移动
////                a.BeatBack(transform, iceThunderInterruptTime, iceThunderInterruptVector);
////            }
////            Debug.Log("打到了： " + targetsName.ToString());


////            targetsName.Clear();
////        }
////    }

////    private ThunderLink thunderLinkObject;
////    private List<GameObject> thunderLinkHasAttackedList = new List<GameObject>(16);
////    private float thunderLinkIntervalTime = 1f;
////    private float thunderLinkCurTime = 0f;
////    private int thunderLinkAttackTotalTimes = 6;
////    private int thunderLinkAttackedTimes = 0;
////    private bool isThunderLink = false;
////    private List<GameObject> tempTargetsInThunderCircle = new List<GameObject>(32);
////    Vector2 thunderLinkStartPosition = new Vector2(0,0);
////    //闪电链
////    public void ThunderLink()
////    {
////        isThunderLink = true;
////        thunderLinkObject.gameObject.SetActive(true);

////        thunderLinkStartPosition.x = transform.localScale.x < 0 ? transform.position.x - 1 : transform.position.x + 1;
////        thunderLinkStartPosition.y = transform.position.y;

////        thunderLinkObject.SetStartPosition(thunderLinkStartPosition);
////    }
////    private void ThunderLinkClock()
////    {
////        if (thunderLinkAttackedTimes >= thunderLinkAttackTotalTimes)
////        {
////            isThunderLink = false;
////            thunderLinkCurTime = 0f;
////            thunderLinkAttackedTimes = 0;
////            thunderLinkHasAttackedList.Clear();
////            thunderLinkObject.gameObject.SetActive(false);
////            Debug.Log("闪电链攻击达到上限次数，结束");
////        }

////        thunderLinkCurTime += Time.deltaTime;
////        if(thunderLinkCurTime >= thunderLinkIntervalTime)
////        {
////            thunderLinkCurTime = 0f;
////            thunderLinkAttackedTimes++;

////            Except(targetInThunderCircle, thunderLinkHasAttackedList, out tempTargetsInThunderCircle);
////            GameObject target = GetClosestTargetInList(tempTargetsInThunderCircle);
////            if(target != null)
////            {
////                thunderLinkHasAttackedList.Add(target);
////            }
////            tempTargetsInThunderCircle = null;

////            //while(targetInThunderCircle.Count > 0)
////            //{
////            //    if(thunderLinsHasAttackedList.Contains(target))
////            //    {
////            //        targetInThunderCircle.Remove(target);
////            //        tempTargetsInThunderCircle.Add(target);
////            //        target = GetClosestTargetInThunderCircle();
////            //    }
////            //    else
////            //    {
////            //        break;
////            //    }
////            //}
////            //如果当前找不到敌人，则终止闪电链技能
////            if (target == null)
////            {
////                isThunderLink = false;
////                thunderLinkCurTime = 0f;
////                thunderLinkAttackedTimes = 0;
////                thunderLinkHasAttackedList.Clear();
////                thunderLinkObject.gameObject.SetActive(false);
////                Debug.Log("闪电链攻击找不到敌人，结束");
////            }
////            else
////            {
////                //通知函数
////                Debug.Log("闪电链攻击敌人：" + target.gameObject.name);
////                thunderLinkObject.SetTarget(target);
////            }
////            //foreach(GameObject targetToBeRemoved in tempTargetsInThunderCircle)
////            //{
////            //    targetInThunderCircle.Add(targetToBeRemoved);
////            //}

////        }
////    }

////    private void Except(List<GameObject> allList, List<GameObject> excpetList, out List<GameObject> resultList)
////    {
////        resultList = new List<GameObject>(allList.Count);
////        GameObject temp;
////        for(int i = 0;i < allList.Count; i ++)
////        {
////            temp = allList[i];
////            if (!excpetList.Contains(temp))
////            {
////                resultList.Add(temp);
////            }
////        }
////    }

////    //ThunderAndFire()使人物状态改变调用人物释放技能动画，其释放技能动画帧事件再根据SkillEvent()开启火雷技能动画

////    public void FireThunder()
////    {
////        //动画处理

////        if (movementComponent.RequestChangeControlStatus(fireThunderMovementTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
////        {
////            //释放该技能上升一小段距离
////            if (movementComponent.RequestMoveByTime(fireThunderVector, fireThunderMovementTime, MovementPlayer.MovementMode.Ability))
////            {
////                currentSkill = 1; 
////                movementComponent.playerAnim.SetUseSkillType((int)SkillType.Singing);//决定释放的技能动画是哪个 
////            }
////        }
////        else
////        {
////            Debug.Log("火雷攻击请求失败");
////        }
////    }
    
////    //火雷动画帧事件调用fireThunderEvent()
////    public void FireThunderEvent()
////    {
////        Debug.Log("fireThunderEvent");
////        //找到攻击命中的单位 canFight.AttackArea实现了攻击
////        targets = canFight.AttackArea(fireThunderAttackArea, fireThunderAttackDamage);
////        if (targets != null)
////        {
////            foreach (CanBeFighted a in targets)
////            {
////                targetsName.Append(" ");
////                targetsName.Append(a.gameObject.name);
////               //击退效果

////                a.BeatBack(transform, fireThunderInterruptTime, fireThunderInterruptVector);
////            }
////            Debug.Log("打到了： " + targetsName.ToString());

////            targetsName.Clear();
////        }
////    }

////    //风 短按主
////    //空气炮：
////    //对怪物、物体：击退
////    //对自己：反作用力
////    public void WindShortMain()
////    {
////        //动画处理
////        if (movementComponent.RequestChangeControlStatus(0.2f, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
////        {
////            //释放该技能后退一小段距离
////            //windVector.x *= transform.localScale.x;
////            if (movementComponent.RequestMoveByTime(windVector, 0.2f, MovementPlayer.MovementMode.Ability))
////            {
////                currentSkill = 2;
////                //movementComponent.playerAnim.SetAbilityNum(currentSkill);//状态改变，动画机处理攻击动画   
////                movementComponent.playerAnim.SetUseSkillType((int)SkillType.Normal);
////            }
////        }
////        else
////        {
////            Debug.Log("空气炮攻击请求失败");
////        }
////    }
////    public void WindShortMainEvent()
////    {
////        Debug.Log("WindShortMainEvent");
////        //找到攻击命中的单位 canFight.AttackArea实现了攻击
////        targets = canFight.AttackArea(windShortMainAttackArea, windShortMainAttackDamage);
////        if (targets != null)
////        {
////            foreach (CanBeFighted a in targets)
////            {
////                targetsName.Append(" ");
////                targetsName.Append(a.gameObject.name);
////                //击退效果
////                //BeatBack(a, windInterruptTime, windInterruptVector);
////                a.BeatBack(transform, windInterruptTime, windInterruptVector);
////            }
////            Debug.Log("打到了： " + targetsName.ToString());

////            targetsName.Clear();
////        }
////    }


////    public void blink()
////    {
////        if(movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.AbilityNeedControl) )
////        {
////            movementComponent.RequestMoveByFrame(blinkVector, MovementPlayer.MovementMode.Ability, Space.Self);
////        }
////    }

////    public void dash()
////    {
////        if(movementComponent.RequestChangeControlStatus(dashTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            movementComponent.RequestMoveByTime(dashVector, dashTime, MovementPlayer.MovementMode.Ability);
////        }
////    }

////    public void Dart()
////    {
////        if (movementComponent.RequestChangeControlStatus(0.05f, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject dart = poolManager.GetGameObject("Dart", gameObject.transform.position);

////            //修改位置
////            dartTargetPosition.x = gameObject.transform.position.x + dartDistance * gameObject.transform.localScale.x;
////            dartTargetPosition.y = gameObject.transform.position.y;
//>>>>>>> 3ebb45914b458dda109335730751ae7b50763c09
            
////              dart.GetComponent<Dart>().Init(gameObject, dartTargetPosition);
////        }
////    }
////    public void Dart(Vector2 targetPosition)
////    {
////        if (movementComponent.RequestChangeControlStatus(0.05f, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject dart = GameObject.Find("PoolManager").GetComponent<PoolManager>().GetGameObject("Dart", gameObject.transform.position);

////            dart.GetComponent<Dart>().Init(gameObject, targetPosition);
////        }
////    }

////    public void WaterShield()
////    {
////        if (movementComponent.RequestChangeControlStatus(waterShieldCastingTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
////        {
////            defencePlayer.ShieldUp(waterShieldPoint);
////            //开启计时器
////            isWaterShieldClockOn = true;
////            WSCurTime = 0f;
////        }
////    }
////    private void WaterShieldClock()
////    {
////        if (isWaterShieldClockOn)
////        {
////            WSCurTime += Time.deltaTime;
////            if (WSCurTime >= WSDurationTime)
////            {
////                if(defencePlayer.IsSieldUp())
////                {
////                    defencePlayer.Heal(waterShieldHealPoint);
////                }
////                defencePlayer.ShieldDown();
////                isWaterShieldClockOn = false;
////                WSCurTime = 0f;
////            }
////        }
////    }


////    public void BlinkBack()
////    {
////        if (canBlinkBack && movementComponent.RequestChangeControlStatus(0.1f, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
////        {
////            movementComponent.RequestMoveByFrame(
////                blinkBackPositions[(BBCurPointer + 1) % BBVectorSize],
////                MovementPlayer.MovementMode.Ability, Space.World);
////        }
////    }
////    private void BlinkBackClock()
////    {
////        BBCurTime += Time.deltaTime;
////        if (BBCurTime >= blinkBackDetectTime)
////        {
////            blinkBackPositions[BBCurPointer].x = transform.position.x;
////            blinkBackPositions[BBCurPointer].y = transform.position.y;
////            shadowPositions[BBCurPointer].position = blinkBackPositions[BBCurPointer];

////            BBCurPointer = (BBCurPointer + 1) % BBVectorSize;
////            BBCurTime = 0f;
////        }
////    }

////    /// <summary>
////    /// 火球，短按火
////    /// </summary>
////    public void FireBallAbility()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject fireBallAbility = poolManager.GetGameObject("FireBallAbility");
////            FireBallAbility a = fireBallAbility.GetComponent<FireBallAbility>();
////            a.SetThrower(gameObject);
////            a.SetStartPosition(transform.position);

////            flyingDirection.x = transform.localScale.x;
////            flyingDirection.y = 0;
////            a.SetDirection(flyingDirection);
////        }
////    }

////    /// <summary>
////    /// 雷球，短按雷
////    /// </summary>
////    public void ThunderBall()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject thunderBall = poolManager.GetGameObject("ThunderBall");
////            ThunderBall a = thunderBall.GetComponent<ThunderBall>();
////            a.SetThrower(gameObject);
////            a.SetStartPosition(transform.position);

////            flyingDirection.x = transform.localScale.x;
////            flyingDirection.y = 0;
////            a.SetDirection(flyingDirection);
////        }
////    }

////    public void Meteorite()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject meteorite = poolManager.GetGameObject("Meteorite");
////            Meteorite a = meteorite.GetComponent<Meteorite>();
////            a.SetThrower(gameObject);

////            //设置生成位置
////            flyingStartPositon.x = transform.position.x;
////            flyingStartPositon.y = transform.position.y + 5.0f;
////            a.SetStartPosition(flyingStartPositon);

////            //设置方向
////            flyingDirection.x = transform.localScale.x;
////            flyingDirection.y = -1;
////            a.SetDirection(flyingDirection);
////        }
////    }

////    /// <summary>
////    /// 岩浆
////    /// </summary>
////    public void Lava()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            int num = (int)(Random.Range(1, 10) + 0.5);
////            for (int i = 0; i < num; i++)
////            {
////                GameObject lava = poolManager.GetGameObject("Lava");
////                Lava a = lava.GetComponent<Lava>();


////                a.SetThrower(gameObject);

////                //设置生成位置
////                flyingStartPositon.x = transform.position.x;
////                flyingStartPositon.y = transform.position.y + 1;
////                a.SetStartPosition(flyingStartPositon);

////                a.SetDirection(Mathf.Pow(-1, (int)(Random.Range(1, 10) + 0.5)));
////            }
////        }
////    }

////    /// <summary>
////    /// 飓风
////    /// </summary>
////    public void Hurricane()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject hurricane = poolManager.GetGameObject("Hurricane");
////            Hurricane a = hurricane.GetComponent<Hurricane>();
////            a.SetThrower(gameObject);

////            //设置生成位置
////            flyingStartPositon.y = transform.position.y;
////            flyingStartPositon.x = transform.position.x + 2.0f * transform.localScale.x;
////            a.SetStartPosition(flyingStartPositon);
////        }
////    }

////    /// <summary>
////    /// 风箭
////    /// </summary>
////    public void WindArrow()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject windArrow = poolManager.GetGameObject("WindArrow");
////            WindArrow a = windArrow.GetComponent<WindArrow>();
////            a.SetThrower(gameObject);

////            //设置生成位置
////            flyingStartPositon.y = transform.position.y;
////            flyingStartPositon.x = transform.position.x + 1.0f * transform.localScale.x;
////            a.SetStartPosition(flyingStartPositon);

////            //设置方向
////            flyingDirection.x = transform.localScale.x;
////            flyingDirection.y = 0;
////            a.SetDirection(flyingDirection);
////        }
////    }

////    /// <summary>
////    /// 保护性火球
////    /// </summary>
////    public void ProtectiveFireBall()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            /**第1个**/
////            GameObject protectiveFireBall1 = poolManager.GetGameObject("ProtectiveFireBall");
////            ProtectiveFireBall a = protectiveFireBall1.GetComponent<ProtectiveFireBall>();
////            a.SetThrower(gameObject);

////            //设置生成位置
////            flyingStartPositon.y = transform.position.y + 1.0f;
////            flyingStartPositon.x = transform.position.x;
////            a.SetStartPosition(flyingStartPositon);

////            /**第2个**/
////            GameObject protectiveFireBall2 = poolManager.GetGameObject("ProtectiveFireBall");
////            ProtectiveFireBall b = protectiveFireBall2.GetComponent<ProtectiveFireBall>();
////            b.SetThrower(gameObject);

////            //设置生成位置
////            flyingStartPositon.y = transform.position.y - 1.0f;
////            flyingStartPositon.x = transform.position.x - 1.0f;
////            b.SetStartPosition(flyingStartPositon);

////            /**第3个**/
////            GameObject protectiveFireBall3 = poolManager.GetGameObject("ProtectiveFireBall");
////            ProtectiveFireBall c = protectiveFireBall3.GetComponent<ProtectiveFireBall>();
////            c.SetThrower(gameObject);

////            //设置生成位置
////            flyingStartPositon.y = transform.position.y - 1.0f;
////            flyingStartPositon.x = transform.position.x + 1.0f;
////            c.SetStartPosition(flyingStartPositon);
////        }
////    }

////    /// <summary>
////    /// 冰箭
////    /// </summary>
////    public void IceArrow()
////    {
////        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
////        {
////            GameObject iceArrow = poolManager.GetGameObject("IceArrow");
////            IceArrow a = iceArrow.GetComponent<IceArrow>();
////            a.SetThrower(gameObject);
////            a.SetStartPosition(transform.position);

////            flyingDirection.x = transform.localScale.x;
////            flyingDirection.y = 0;
////            a.SetDirection(flyingDirection);
////        }
////    }

////    private GameObject GetClosestTargetInList(List<GameObject> gameObjects)
////    {
////        float minDistance = float.MaxValue;
////        float tempDistance;
////        GameObject cloestTarget = null;
////        foreach(GameObject target in gameObjects)
////        {
////            tempDistance = Vector2.Distance(target.transform.position, transform.position);
////            if(tempDistance < minDistance)
////            {
////                minDistance = tempDistance;
////                cloestTarget = target;
////            }
////        }
////        return cloestTarget;
////    }

////    public void AddInThunderCircleTarget(GameObject target)
////    {
////        targetInThunderCircle.Add(target);
////        printObjectInThunderCircle();
////    }
////    public void RemoveInThunderCircleTarget(GameObject target)
////    {
////        targetInThunderCircle.Remove(target);
////        printObjectInThunderCircle();
////    }
////    StringBuilder lala = new StringBuilder(128);
////    private void printObjectInThunderCircle()
////    {
////        lala.Append("在雷圈里有：");
////        foreach(GameObject a in targetInThunderCircle)
////        {
////            lala.Append(a.name);
////            lala.Append("-");
////            lala.Append(a.GetInstanceID());
////            lala.Append(" ");
////        }
////        Debug.Log(lala.ToString());
////        lala.Clear();
////    }

////    public override int GetPriorityInType()
////    {
////        return priorityInType;
////    }
////    public override UpdateType GetUpdateType()
////    {
////        return type;
////    }
////}
