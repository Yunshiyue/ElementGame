/**
 * @Description: 主角的技能释放组件，依赖CanFight组件，接收control脚本的控制信息，负责技能释放的具体实现。
 *               通过向移动组件requestChangeControlStatus的返回值来判读自己能够进行想要是放的技能。
 *               如果能够，可以向移动组件请求改变移动状态，同时应该向动画组间申请播放技能动画。
 * @Author: ridger

 *
 * @Description: 添加了雷火攻击方法，攻击击退效果
 * @Author: 夜里猛

 * 

 * @Editor: ridger
 * @Edit: 增加了扔回旋镖的方法，给定目的地扔回旋镖，用来测试风属性瞄准镜。
 * 

 * @Editor: ridger
 * @Edit: 1.将AttackPlayer继承自myUpdate，设计想法是技能组件可能记录一些需要计时性质的技能，比如buff等，所以需要
 *          逐帧调用来实现计时器等功能。
 *        2.实现了水盾技能，水盾能够抵挡两点攻击，有最大时间限制，消失后能够回复1点hp
 *          水盾计时器逻辑在MyUpdate中实现，动画效果在DefencePlayer组件中委托动画管理组件实现。
 *          

 * @Editor: ridger
 * @Edit: 实现了残影闪回技能，实现逻辑如下:
 *           a.blinkBackTime参数设定闪回到多少s之前的位置，blinkBackDetectTime决定每隔多少s就一次残影的位置
 *           b.每隔blinkBackDetectTime，会将当前位置记录在一个数组的最新位置处，并将对应的残影位置设置为这个位置
 *           c.调用闪回时，申请主动传送，传送至位置记录数组的下一个位置，即即将被更新的位置，也就是n秒之前的位置
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(MovementPlayer))]
public class AttackPlayer : myUpdate
{

    private PoolManager poolManager;
    private CanFight canFight;
    private MovementPlayer movementComponent;

    private Player.Element mainElement;
    private Player.Element firstOtherElement;
    private Player.Element secondOtherElement;

    private int priorityInType = 3;
    private UpdateType type = UpdateType.Player;

    //当前技能编号 用于人物技能动画帧事件判断处理哪一个动画
    private int currentSkill = 0;

    //被击中单位的位置
    private Vector2 attackedPos = new Vector2(0, 0);
    //普通攻击范围、伤害、字符串标识(名字)
    private Collider2D normalAttackArea;
    private int normalAttackDamage = 1;
    public const string NORMAL_ATTACK_NAME = "NormalAttack";
    //该攻击打断敌人参数
    private float normalInterruptTime = 0.3f;
    private Vector2 normalInterruptVector = new Vector2(3f, 0);

    //雷圈
    private Collider2D thunderCircleArea;
    //雷主元素
    private GameObject thunderElement;
    //雷主长按 只用于记录当前可命中的敌人有哪些，真正造成伤害由雷球个体实现
    private GameObject thunderLongMain;
    private Collider2D thunderLongMainArea;
    private int thunderLongMainDamage = 1;
    private float thunderLongMainInterruptTime = 0.5f;
    private Vector2 thunderLongMainInterruptVector = new Vector2(2.5f, 0f);
    private float thunderLongMainTime = 0.2f;
    public const string THUNDER_LONG_MAIN_NAME = "ThunderLongMain";
    //雷火
    private GameObject thunderFire;
    //雷火攻击范围、伤害、字符串标识(名字)
    private PolygonCollider2D thunderFireAttackArea;
    private int thunderFireAttackDamage = 1;
    public const string THUNDER_FIRE_NAME = "ThunderFire";
    //该技能攻击打断敌人参数
    private float thunderFireInterruptTime = 0.5f;
    private Vector2 thunderFireInterruptVector = new Vector2(4f, 0);
    //使用该技能上升距离
    private Vector2 ThunderFireVector = new Vector2(0, 0.5f);
    private float thunderFireMovementTime = 0.2f;
   

    //短按风主空气炮
    private Collider2D windShortMainAttackArea;
    private int windShortMainAttackDamage = 1;
    public const string WIND_NAME = "Wind";
    //该技能攻击打断敌人参数
    private float windInterruptTime = 0.2f;
    private Vector2 windInterruptVector = new Vector2(5f, 0);
    //使用该技能自身后退距离
    private Vector2 windVector = new Vector2(-0.15f, 0);
    private GameObject windShortMain;

    //闪烁距离、字符串表示(名字)
    private Vector2 blinkVector = new Vector2(5, 0);
    public const string BLINK_NAME = "Blink";

    //冲刺距离、时间
    private Vector2 dashVector = new Vector2(2f, 0);
    private float dashTime = 0.3f;
    public const string DASH_NAME = "Dash";

    private string targetLayerName = "Enemy";

    //Dart技能
    private Vector2 dartTargetPosition = new Vector2(0, 0);
    private int dartDistance = 10;

    //水盾技能
    private float waterShieldCastingTime = 0.2f;
    private float WSDurationTime = 8f;
    private int waterShieldPoint = 2;
    private int waterShieldHealPoint = 1;
    private bool canWSClock = true;
    private DefencePlayer defencePlayer;
    private bool isWaterShieldClockOn = false;
    private float WSCurTime = 0f;

    //BlinkBack技能
    //Time应该为DetectTime的整数倍才有意义
    private float blinkBackTime = 3f;
    private float blinkBackDetectTime = 0.1f;
    private int BBVectorSize;
    private Vector2[] blinkBackPositions;
    private bool canBlinkBack = true;
    private float BBCurTime = 0f;
    private int BBCurPointer = 0;
    private Transform[] shadowPositions;
    private GameObject shadowParent;
    private GameObject shadow;

    //飞行技能参数
    private float flyingAbilityCastTime = 0.05f;
    private Vector3 flyingDirection = Vector3.zero;
    private Vector2 flyingStartPositon = Vector2.zero;

    public override void Initialize()
    {
        canFight = GetComponent<CanFight>();
        if(canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
        }

        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        if (poolManager == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到poolManager组件");
        }

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);

        movementComponent = GetComponent<MovementPlayer>();
        if(movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }

        defencePlayer = GetComponent<DefencePlayer>();
        if (defencePlayer == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到defencePlayer组件！");
        }

        BBVectorSize = (int) (blinkBackTime / blinkBackDetectTime);
        blinkBackPositions = new Vector2[BBVectorSize];
        for(int i = 0; i < BBVectorSize; i++)
        {
            blinkBackPositions[i] = new Vector2(transform.position.x, transform.position.y);
        }

        shadowParent = GameObject.Find("Shadows");
        if (shadowParent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到Shadows");
        }
        shadow = GameObject.Find("Shadow");
        if (shadowParent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到Shadow");
        }
        shadowPositions = new Transform[BBVectorSize];
        GameObject tempShadow;
        for(int i = 0; i < BBVectorSize; i++)
        {
            tempShadow = Instantiate(shadow);
            shadowPositions[i] = tempShadow.transform;
            shadowPositions[i].SetParent(shadowParent.transform);
            shadowPositions[i].position = blinkBackPositions[i];
        }
        shadow.SetActive(false);

        normalAttackArea = GameObject.Find("NormalAttack").GetComponent<Collider2D>();
        if (normalAttackArea == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有把普通攻击组件拖动到AttackPlayer脚本中！");
        }

        //如果选择的主属性是雷 加载雷圈 雷主技能
        thunderElement = gameObject.transform.Find("Thunder").gameObject;
        thunderElement.SetActive(true);
        thunderCircleArea = thunderElement.GetComponent<Collider2D>();
        //长按雷主的技能范围
        thunderLongMain = thunderElement.transform.Find("ThunderLongMain").gameObject;
        thunderLongMainArea = thunderLongMain.GetComponent<Collider2D>();
        //之后根据属性搭配来进行判断 是否设为true
        //雷火
        thunderFire = thunderElement.transform.Find("ThunderFire").gameObject;
        thunderFire.SetActive(true);
        thunderFireAttackArea = GameObject.Find("ThunderFireCollider").GetComponent<PolygonCollider2D>();
        if (thunderFireAttackArea == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有把雷火攻击碰撞组件拖动到AttackPlayer脚本中！");
        }

        //风，短按
        windShortMain = gameObject.transform.Find("WindShortMain").gameObject;
        windShortMain.SetActive(true);
        windShortMainAttackArea = GameObject.Find("WindShortMainCollider").GetComponent<Collider2D>();
        if (windShortMainAttackArea == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有把风主短按攻击碰撞组件拖动到AttackPlayer脚本中！");
        }
    }

    public override void MyUpdate()
    {
        if (canWSClock)
            WaterShieldClock();
        if (canBlinkBack)
            BlinkBackClock();
    }

    public void ChangeElement(Player.Element mainElement, Player.Element aElement, Player.Element bElement)
    {
        this.mainElement = mainElement;
        firstOtherElement = aElement;
        secondOtherElement = bElement;
        if(mainElement == Player.Element.Fire)
        {
            if(aElement == Player.Element.Icy || bElement == Player.Element.Icy)
            {
                canWSClock = true;
            }
        }
        else if(mainElement == Player.Element.Wind)
        {
            if(aElement == Player.Element.Fire || bElement == Player.Element.Fire)
            {
                canBlinkBack = true;
            }
        }
    }


    //debug出攻击到的单位
    StringBuilder targetsName = new StringBuilder();
    CanBeFighted[] targets;
    public void NormalAttack()
    {
        //动画处理
        if(movementComponent.RequestChangeControlStatus(0.1f, MovementPlayer.PlayerControlStatus.AbilityNeedControl)){
            movementComponent.playerAnim.SetAbilityNum(0);//状态改变，动画机处理攻击动画
        }
        else
        {
            Debug.Log("普通攻击请求失败");
        }   
    }
    //帧事件调用攻击
    public void AttackEvent()
    {
        Debug.Log("AttackEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(normalAttackArea, normalAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                BeatBack(a, normalInterruptTime, normalInterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());
            targetsName.Clear();
        }
    }

    //攻击击退效果
    private void BeatBack(CanBeFighted a,float interruptTime,Vector2 interruptVector)
    {
        //被打的敌人位置
        MovementEnemies movement = a.gameObject.GetComponent<MovementEnemies>();
        attackedPos.x = a.transform.position.x;
        attackedPos.y = a.transform.position.y;

        if (movement.RequestChangeControlStatus(interruptTime, MovementEnemies.EnemyStatus.Stun))
        {
            Debug.Log("击退");
            if (a.transform.position.x > transform.position.x)
            {
                movement.RequestMoveByTime(  interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
            }
            else
            {
                movement.RequestMoveByTime(- interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
            }
        }
    }
    //重载击退方法 ，用于player身外的物体实现击退敌人
    private void BeatBack(CanBeFighted a,Transform b,float interruptTime,Vector2 interruptVector)
    {
        //被打的敌人位置
        MovementEnemies movement = a.gameObject.GetComponent<MovementEnemies>();
        attackedPos.x = a.transform.position.x;
        attackedPos.y = a.transform.position.y;

        if (movement.RequestChangeControlStatus(interruptTime, MovementEnemies.EnemyStatus.Stun))
        {
            Debug.Log("击退");
            if (a.transform.position.x > b.position.x)
            {
                movement.RequestMoveByTime( interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
            }
            else
            {
                movement.RequestMoveByTime( - interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
            }
        }
    }


    //技能帧事件:根据当前技能数决定执行哪个技能特效动画
    public void SkillEvent()
    {
        switch (currentSkill)
        {
            case 1:
                thunderFire.GetComponent<Animator>().SetBool("effect", true);
                Debug.Log("状态：雷火");
                break;
            case 2:
                windShortMain.GetComponent<Animator>().SetBool("effect", true);
                Debug.Log("状态：空气炮");
                break;
            case 3:
                //情况与之前两者不太一样，此处用于生成雷击aoe个体，在雷击个体中的动画帧事件攻击生效
                ThunderLongMainCheckEvent();
                Debug.Log("状态：长按雷主");
                break;
        }
    }

    //ThunderLongMain() 雷 长按主 ;
    //自动追踪的雷击，有一定延迟，在成小范围aoe伤害。
    public void ThunderLongMain()
    {
        //动画处理
        //征求施法时间
        if (movementComponent.RequestChangeControlStatus(thunderLongMainTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
                currentSkill = 3;
                movementComponent.playerAnim.SetAbilityNum(currentSkill);//状态改变，动画机处理攻击动画   
                movementComponent.playerAnim.SetUseSkillType(0);//决定释放的技能动画是哪个 
        }
        else
        {
            Debug.Log("雷主长按请求失败");
        }
    }

    //雷主长按施法动画调用帧事件 SkillEvent()再调用 ThunderLongMainCheckEvent() 用于找到可以集中的目标
    public void ThunderLongMainCheckEvent()
    {
        //找到可命中的单位 
        targets = canFight.AttackArea(thunderCircleArea, 0);//检测不造成伤害，伤害为0
        if (targets != null)
        {
            //雷击在第一个目标对象处出现
            thunderLongMain.transform.position = targets[0].transform.position;
            //雷击启动
            thunderLongMain.GetComponent<Animator>().SetBool("ready", true);
            Debug.Log("雷主长按打到的怪物： " + targets[0].ToString());
        }
        else
        {
            Debug.Log("雷圈范围内没有敌人");
        }
    }
    //雷主长按技能攻击事件
    public void ThunderLongMainAttackEvent()
    {
        targets = canFight.AttackArea(thunderLongMainArea, thunderLongMainDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //击退效果
                BeatBack(a,thunderLongMain.transform ,thunderLongMainInterruptTime, thunderLongMainInterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
        else
        {
            Debug.Log("雷圈范围内没有敌人");
        }
    }

    //ThunderAndFire()使人物状态改变调用人物释放技能动画，其释放技能动画帧事件再根据SkillEvent()开启雷火技能动画
    public void ThunderAndFire()
    {
        //动画处理

        if (movementComponent.RequestChangeControlStatus(thunderFireMovementTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            //释放该技能上升一小段距离
            if (movementComponent.RequestMoveByTime(ThunderFireVector, thunderFireMovementTime, MovementPlayer.MovementMode.Ability))
            {
                currentSkill = 1;
                movementComponent.playerAnim.SetAbilityNum(currentSkill);//状态改变，动画机处理攻击动画   
                movementComponent.playerAnim.SetUseSkillType(0);//决定释放的技能动画是哪个 

            }
        }
        else
        {
            Debug.Log("雷火攻击请求失败");
        }
    }
    
    //雷火动画帧事件调用ThunderFireEvent()
    public void ThunderFireEvent()
    {
        Debug.Log("ThunderFireEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(thunderFireAttackArea, thunderFireAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
               //击退效果
                BeatBack(a, thunderFireInterruptTime, thunderFireInterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
    }

    //风 短按主
    //空气炮：
    //对怪物、物体：击退
    //对自己：反作用力
    public void WindShortMain()
    {
        //动画处理
        if (movementComponent.RequestChangeControlStatus(0.2f, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            //释放该技能后退一小段距离
            //windVector.x *= transform.localScale.x;
            if (movementComponent.RequestMoveByTime(windVector, 0.2f, MovementPlayer.MovementMode.Ability))
            {
                currentSkill = 2;
                movementComponent.playerAnim.SetAbilityNum(currentSkill);//状态改变，动画机处理攻击动画   
                movementComponent.playerAnim.SetUseSkillType(1);
            }
        }
        else
        {
            Debug.Log("空气炮攻击请求失败");
        }
    }
    public void WindShortMainEvent()
    {
        Debug.Log("WindShortMainEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(windShortMainAttackArea, windShortMainAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //击退效果
                BeatBack(a, windInterruptTime, windInterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
    }


    public void blink()
    {
        if(movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.AbilityNeedControl) )
        {
            movementComponent.RequestMoveByFrame(blinkVector, MovementPlayer.MovementMode.Ability, Space.Self);
        }
    }

    public void dash()
    {
        if(movementComponent.RequestChangeControlStatus(dashTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            movementComponent.RequestMoveByTime(dashVector, dashTime, MovementPlayer.MovementMode.Ability);
        }
    }

    public void Dart()
    {
        if (movementComponent.RequestChangeControlStatus(0.05f, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject dart = poolManager.GetGameObject("Dart", gameObject.transform.position);

            //修改位置
            dartTargetPosition.x = gameObject.transform.position.x + dartDistance * gameObject.transform.localScale.x;
            dartTargetPosition.y = gameObject.transform.position.y;
            
            dart.GetComponent<Dart>().Init(gameObject, dartTargetPosition);
        }
    }
    public void Dart(Vector2 targetPosition)
    {
        if (movementComponent.RequestChangeControlStatus(0.05f, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject dart = GameObject.Find("PoolManager").GetComponent<PoolManager>().GetGameObject("Dart", gameObject.transform.position);

            dart.GetComponent<Dart>().Init(gameObject, targetPosition);
        }
    }

    public void WaterShield()
    {
        if (movementComponent.RequestChangeControlStatus(waterShieldCastingTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            defencePlayer.ShieldUp(waterShieldPoint);
            //开启计时器
            isWaterShieldClockOn = true;
            WSCurTime = 0f;
        }
    }
    private void WaterShieldClock()
    {
        if (isWaterShieldClockOn)
        {
            WSCurTime += Time.deltaTime;
            if (WSCurTime >= WSDurationTime)
            {
                if(defencePlayer.IsSieldUp())
                {
                    defencePlayer.Heal(waterShieldHealPoint);
                }
                defencePlayer.ShieldDown();
                isWaterShieldClockOn = false;
                WSCurTime = 0f;
            }
        }
    }


    public void BlinkBack()
    {
        if (canBlinkBack && movementComponent.RequestChangeControlStatus(0.1f, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            movementComponent.RequestMoveByFrame(
                blinkBackPositions[(BBCurPointer + 1) % BBVectorSize],
                MovementPlayer.MovementMode.Ability, Space.World);
        }
    }
    private void BlinkBackClock()
    {
        BBCurTime += Time.deltaTime;
        if (BBCurTime >= blinkBackDetectTime)
        {
            blinkBackPositions[BBCurPointer].x = transform.position.x;
            blinkBackPositions[BBCurPointer].y = transform.position.y;
            shadowPositions[BBCurPointer].position = blinkBackPositions[BBCurPointer];

            BBCurPointer = (BBCurPointer + 1) % BBVectorSize;
            BBCurTime = 0f;
        }
    }

    /// <summary>
    /// 火球，短按火
    /// </summary>
    public void FireBallAbility()
    {
        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject fireBallAbility = poolManager.GetGameObject("FireBallAbility");
            FireBallAbility a = fireBallAbility.GetComponent<FireBallAbility>();
            a.SetThrower(gameObject);
            a.SetStartPosition(transform.position);

            flyingDirection.x = transform.localScale.x;
            flyingDirection.y = 0;
            a.SetDirection(flyingDirection);
        }
    }

    /// <summary>
    /// 雷球，短按雷
    /// </summary>
    public void ThunderBall()
    {
        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject thunderBall = poolManager.GetGameObject("ThunderBall");
            ThunderBall a = thunderBall.GetComponent<ThunderBall>();
            a.SetThrower(gameObject);
            a.SetStartPosition(transform.position);

            flyingDirection.x = transform.localScale.x;
            flyingDirection.y = 0;
            a.SetDirection(flyingDirection);
        }
    }

    public void Meteorite()
    {
        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject meteorite = poolManager.GetGameObject("Meteorite");
            Meteorite a = meteorite.GetComponent<Meteorite>();
            a.SetThrower(gameObject);

            //设置生成位置
            flyingStartPositon.x = transform.position.x;
            flyingStartPositon.y = transform.position.y + 5.0f;
            a.SetStartPosition(flyingStartPositon);

            //设置方向
            flyingDirection.x = transform.localScale.x;
            flyingDirection.y = -1;
            a.SetDirection(flyingDirection);
        }
    }

    /// <summary>
    /// 岩浆
    /// </summary>
    public void Lava()
    {
        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject lava = poolManager.GetGameObject("Lava");
            Lava a = lava.GetComponent<Lava>();
            a.SetThrower(gameObject);

            //设置生成位置
            a.SetStartPosition(transform.position);

            //设置方向
            a.SetDirection(transform.localScale.x);
        }
    }

    /// <summary>
    /// 飓风
    /// </summary>
    public void Hurricane()
    {
        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject hurricane = poolManager.GetGameObject("Hurricane");
            Hurricane a = hurricane.GetComponent<Hurricane>();
            a.SetThrower(gameObject);

            //设置生成位置
            flyingStartPositon.y = transform.position.y;
            flyingStartPositon.x = transform.position.x + 2.0f * transform.localScale.x;
            a.SetStartPosition(flyingStartPositon);
        }
    }

    /// <summary>
    /// 风箭
    /// </summary>
    public void WindArrow()
    {
        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject windArrow = poolManager.GetGameObject("WindArrow");
            WindArrow a = windArrow.GetComponent<WindArrow>();
            a.SetThrower(gameObject);

            //设置生成位置
            flyingStartPositon.y = transform.position.y;
            flyingStartPositon.x = transform.position.x + 1.0f * transform.localScale.x;
            a.SetStartPosition(flyingStartPositon);

            //设置方向
            flyingDirection.x = transform.localScale.x;
            flyingDirection.y = 0;
            a.SetDirection(flyingDirection);
        }
    }

    /// <summary>
    /// 保护性火球
    /// </summary>
    public void ProtectiveFireBall()
    {
        if (movementComponent.RequestChangeControlStatus(flyingAbilityCastTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            /**第1个**/
            GameObject protectiveFireBall1 = poolManager.GetGameObject("ProtectiveFireBall");
            ProtectiveFireBall a = protectiveFireBall1.GetComponent<ProtectiveFireBall>();
            a.SetThrower(gameObject);

            //设置生成位置
            flyingStartPositon.y = transform.position.y + 1.0f;
            flyingStartPositon.x = transform.position.x;
            a.SetStartPosition(flyingStartPositon);

            /**第2个**/
            GameObject protectiveFireBall2 = poolManager.GetGameObject("ProtectiveFireBall");
            ProtectiveFireBall b = protectiveFireBall2.GetComponent<ProtectiveFireBall>();
            b.SetThrower(gameObject);

            //设置生成位置
            flyingStartPositon.y = transform.position.y - 1.0f;
            flyingStartPositon.x = transform.position.x - 1.0f;
            b.SetStartPosition(flyingStartPositon);

            /**第3个**/
            GameObject protectiveFireBall3 = poolManager.GetGameObject("ProtectiveFireBall");
            ProtectiveFireBall c = protectiveFireBall3.GetComponent<ProtectiveFireBall>();
            c.SetThrower(gameObject);

            //设置生成位置
            flyingStartPositon.y = transform.position.y - 1.0f;
            flyingStartPositon.x = transform.position.x + 1.0f;
            c.SetStartPosition(flyingStartPositon);
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
}
