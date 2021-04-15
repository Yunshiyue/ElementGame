/**
 * @Description: 主角的控制脚本，继承自myUpdate类。拥有移动组件、攻击组件、防御组件等主要组件。
 *               负责输入控制和通知各组件控制信息，诸如移动、跳跃、释放技能等。
 * @Author: ridger

 * 

 * @Editor: ridger
 * @Edit: 下蹲修改为通过Request实现
 * 

 * @Eidtor: ridger
 * @Eidt: 增加了长按短按施法逻辑的控制，元素添加与归还的逻辑、施法中断、打断等逻辑。
 *        以及当主元素为风属性时，瞄准镜的控制逻辑。
 *        增加了当前主元素、辅元素等变量
 *        

 * @Eidtor: ridger
 * @Eidt: 将辅助技能first改为水盾以便测试
 * 

 * @Eidtor: ridger
 * @Eidt: 将辅助技能second改为残影闪回以便测试
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.UI;

[RequireComponent(typeof(MovementPlayer))]
[RequireComponent(typeof(AttackPlayer))]
[RequireComponent(typeof(DefencePlayer))]
public class Player : myUpdate
{
    public enum Element { Fire, Icy, Wind, Thunder }

    private float fullyCastTime = 0.3f;

    private MovementPlayer movementComponent;
    private AttackPlayer attackComponent;
    private DefencePlayer defenceComponent;
    private SightHead sightHead;

    private int priorityInType = 0;
    private UpdateType updateType = UpdateType.Player;
    //临时变量
    private Vector2 tempMovement = new Vector2(0, 0);

    //心心数组
    private HPItem[] hpArray;
    public override void Initialize()
    {
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

        sightHead = GameObject.Find("SightHead").GetComponent<SightHead>();
        if(sightHead == null)
        {
            Debug.LogError("没找到SightHead这个风属性瞄准镜");
        }
        sightHead.gameObject.SetActive(false);

        debugInfoUI = GameObject.Find("PlayerAbilityDebugInfo").GetComponent<Text>();
        if(debugInfoUI == null)
        {
            Debug.LogError("没找到PlayerAbilityDebugInfo这个ui物体");
        }

        //初始化心心数
        GameObject HpPanel = GameObject.Find("HP Panel");

        hpArray = new HPItem[defenceComponent.getHpMax()];
        
        for (int i = 0; i < defenceComponent.getHp(); i++)
        {
            Transform hpItem = HpPanel.transform.GetChild(i);
            hpArray[i] = hpItem.GetComponent<HPItem>();
            hpArray[i].Getting();
        }
    }

    private string temp;
    //根据设计，先进行受伤判断，再进行移动控制和技能控制
    public override void MyUpdate()
    {
        MenuCheck();
        DefenceCheck();
        AbilityControl();
        SetCastDebugInfo();
        MoveControl();

        if (Input.GetButtonDown(FireBallAbility.FIRE_BALL_ABILITY))
        {
            Debug.Log("火球");
            attackComponent.FireBallAbility();
        }

        if (Input.GetButtonDown(ThunderBall.Thunder_BALL))
        {
            Debug.Log("雷球");
            attackComponent.ThunderBall();
        }

        if (Input.GetButtonDown(Meteorite.METEORITE))
        {
            Debug.Log("陨石");
            attackComponent.Meteorite();
        }

        if (Input.GetButtonDown(Lava.LAVA))
        {
            Debug.Log("岩浆");
            attackComponent.Lava();
        }

        if (Input.GetButtonDown(Hurricane.HURRICANE))
        {
            Debug.Log("飓风");
            attackComponent.Hurricane();
        }

        if (Input.GetButtonDown(WindArrow.WIND_ARROW))
        {
            Debug.Log("风箭");
            attackComponent.WindArrow();
        }

        if (Input.GetButtonDown(ProtectiveFireBall.PROTECTIVE_FIRE_BALL))
        {
            Debug.Log("保护性火球");
            attackComponent.ProtectiveFireBall();
        }

        if (Input.GetButtonDown("Dart"))
        {
            attackComponent.Dart();
        }

        //将技能控制的结果输出到控制台
        //temp = AttackControl();
        //if(temp != null)
        //{
        //    Debug.Log(temp);
        //}
    }

    private Element mainElement = Element.Wind;
    private Element firstOtherElement = Element.Icy;
    private Element secondOtherElement = Element.Thunder;
    private int firstOtherElementPoint = 999;
    private int secondOtherElementPoint = 999;

    private bool isLastFrameCasting = false;
    private bool isAddFirstElement = false;
    private bool isAddSecondElement = false;

    private bool isRequestMainElement = false;
    private bool isRequestFirstOtherElement = false;
    private bool isRequestSecondOtherElement = false;

    private float currentCastingTime = 0f;
    private int otherElementCost = 1;

    private Text debugInfoUI;
    private StringBuilder debugInfo = new StringBuilder(512);
    private void AbilityControl()
    {
        if(movementComponent.IsOnFloor()) // 或当前元素为风属性
        {
            isRequestFirstOtherElement = Input.GetButtonDown("FirstOtherElement");
            isRequestSecondOtherElement = Input.GetButtonDown("SecondOtherElement");
            isRequestMainElement = Input.GetButton("MainElement");
        }
        else
        {
            isRequestFirstOtherElement = false;
            isRequestSecondOtherElement = false;
            isRequestMainElement = false;
        }

        //如果上一帧没有施法，则直接释放辅助技能
        if (!isLastFrameCasting)
        {
            if(isRequestFirstOtherElement && canConsumeElement(1, 1))
            {
                //剩余元素点够不够
                consumeElement(1, otherElementCost);
                //释放辅助元素A技能
                CastFirstOtherSpell();
                return;
            }
            if(isRequestSecondOtherElement && canConsumeElement(2, 1))
            {
                consumeElement(2, otherElementCost);
                //释放辅助元素B技能
                CastSecondOtherSpell();
                return;
            }
        }
        //如果上一帧正在施法，则加入辅助元素
        else
        {
            if (isRequestFirstOtherElement && canConsumeElement(1, 1) && !isAddFirstElement)
            {
                consumeElement(1, otherElementCost);
                isAddFirstElement = true;
                //播放消耗元素动画
            }
            if (isRequestSecondOtherElement && canConsumeElement(2, 1) && !isAddSecondElement)
            {
                consumeElement(2, otherElementCost);
                isAddSecondElement = true;
                //播放消耗元素动画
            }
        }
        //有没有按下主元素请求继续施法/开始施法
        if (isRequestMainElement)
        {
            Debug.Log("请求施法！");
            //请求施法成功，开始/继续施法
            if(movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Casting))
            {
                currentCastingTime += Time.deltaTime;
                isLastFrameCasting = true;


                if(currentCastingTime >= fullyCastTime)
                {
                    //出现瞄准镜
                    if (!sightHead.gameObject.activeSelf)
                    {
                        sightHead.gameObject.SetActive(true);
                        sightHead.SetPosition(transform.position);
                    }
                    //施法完成动画
                }
                else
                {
                    //施法未完成动画
                }
            }
            //请求施法失败，啥也不干/被打断
            else
            {
                //打断施法
                if(isLastFrameCasting)
                {
                    Debug.Log("施法被打断！");
                    resumeElement();
                    isAddFirstElement = false;
                    isAddSecondElement = false;
                    isLastFrameCasting = false;
                    currentCastingTime = 0f;

                    sightHead.gameObject.SetActive(false);
                }
            }
        }
        //没有请求施法，进入主动中断/啥也不干
        else
        {
            //如果上一帧正在施法，则主动中断，法术放出
            if(isLastFrameCasting)
            {
                if(currentCastingTime >= fullyCastTime)
                {
                    //其中调用sightHead的getposition方法
                    CastFullyMainSpell();
                }
                else
                {
                    CastShortMainSpell();
                    //短暂施法，归还蓄力元素
                    resumeElement();
                }
                isAddFirstElement = false;
                isAddSecondElement = false;
                isLastFrameCasting = false;
                currentCastingTime = 0f;

                sightHead.gameObject.SetActive(false);
            }
            //如果上一帧没有施法，则啥也不干
        }
    }

    private void SetCastDebugInfo()
    {
        debugInfo.AppendLine("元素搭配");
        debugInfo.Append("mainElement: ");
        debugInfo.AppendLine(mainElement.ToString());
        debugInfo.Append("firstOtherElement: ");
        debugInfo.AppendLine(firstOtherElement.ToString());
        debugInfo.Append("secondOtherElement: ");
        debugInfo.AppendLine(secondOtherElement.ToString());
        
        debugInfo.AppendLine("元素点");
        debugInfo.Append("firstOtherElementPoint: ");
        debugInfo.AppendLine(firstOtherElementPoint.ToString());
        debugInfo.Append("secondOtherElementPoint: ");
        debugInfo.AppendLine(secondOtherElementPoint.ToString());

        debugInfo.AppendLine("其他参数");
        debugInfo.Append("currentCastingTime: ");
        debugInfo.AppendLine(currentCastingTime.ToString());

        debugInfo.Append("isLastFrameCasting: ");
        debugInfo.AppendLine(isLastFrameCasting.ToString());
        debugInfo.Append("isAddFirstElement: ");
        debugInfo.AppendLine(isAddFirstElement.ToString());
        debugInfo.Append("isAddSecondElement: ");
        debugInfo.AppendLine(isAddSecondElement.ToString());
        debugInfo.Append("isRequestMainElement: ");
        debugInfo.AppendLine(isRequestMainElement.ToString());
        debugInfo.Append("isRequestFirstOtherElement: ");
        debugInfo.AppendLine(isRequestFirstOtherElement.ToString());
        debugInfo.Append("isRequestSecondOtherElement: ");
        debugInfo.AppendLine(isRequestSecondOtherElement.ToString());

        debugInfoUI.text = debugInfo.ToString();
        debugInfo.Clear();
    }
    private bool canConsumeElement(int otherElementIndex, int consumeAmount)
    {
        switch(otherElementIndex)
        {
            case 1:
                if(firstOtherElementPoint >= consumeAmount)
                {
                    return true;
                }
                return false;
            case 2:
                if(secondOtherElementPoint >= consumeAmount)
                {
                    return true;
                }
                return false;
        }
        return false;
    }
    private void consumeElement(int otherElementIndex, int consumeAmount)
    {
        switch (otherElementIndex)
        {
            case 1:
                firstOtherElementPoint -= consumeAmount;
                return;
            case 2:
                secondOtherElementPoint -= consumeAmount;
                return;
        }
    }
    private void resumeElement()
    {
        if(isAddFirstElement)
        {
            firstOtherElementPoint++;
        }
        if(isAddSecondElement)
        {
            secondOtherElementPoint++;
        }
    }

    private void CastFullyMainSpell()
    {
        attackComponent.Dart(sightHead.GetPosition());
        Debug.Log("释放主要蓄力法术！");
        //测试雷主长按攻击
        //attackComponent.ThunderLongMain();
    }
    private void CastShortMainSpell()
    {
        //attackComponent.NormalAttack();
        Debug.Log("释放主要短按法术！");

        //测试雷火攻击
        attackComponent.ThunderAndFire();
        
    }
    private void CastFirstOtherSpell()
    {
        //attackComponent.dash();

        //测试水盾
        //attackComponent.WaterShield();

        Debug.Log("释放辅助A元素法术！");
        //测试空气炮攻击
        attackComponent.WindShortMain();
    }
    private void CastSecondOtherSpell()
    {
        //attackComponent.blink();
        //测试闪回
        attackComponent.BlinkBack();
        Debug.Log("释放辅助B元素法术！");
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
        if (defenceComponent.getHpReduction() > 0)
        {
            ChangeHpUI();
        }
        defenceComponent.Clear();
    }
    public void ChangeHpUI()
    {
        for (int i = 0; i < defenceComponent.getRealDamage(); i++)
        {
            int index = defenceComponent.getHp() - i;
            Debug.Log("lost-- HP:" + defenceComponent.getHp() + ";index:" + index);
            hpArray[index].Lost();
        }
    }
    //移动控制，包括X轴移动、跳跃、下蹲；通过向移动组件“请求”实现。
    private void MoveControl()
    {
        tempMovement.x = Input.GetAxis("Horizontal");
        movementComponent.RequestMoveByFrame(tempMovement, MovementPlayer.MovementMode.PlayerControl, Space.Self);

        if(Input.GetButtonDown("Jump"))
        {
            movementComponent.RequestJump();
        }
        if(Input.GetButton("Crouch"))
        {
            movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Crouch);
        }
    }

    ////技能控制，在一帧中只能释放一个技能
    //private string AttackControl()
    //{
    //    if (Input.GetButtonDown(AttackPlayer.BLINK_NAME))
    //    {
    //        attackComponent.blink();
    //        return AttackPlayer.BLINK_NAME;
    //    }

    //    if (Input.GetButtonDown(AttackPlayer.DASH_NAME))
    //    {
    //        attackComponent.dash();
    //        return AttackPlayer.DASH_NAME;
    //    }

    //    if (Input.GetButtonDown(AttackPlayer.NORMAL_ATTACK_NAME))
    //    {

    //        attackComponent.NormalAttack();
    //        return AttackPlayer.NORMAL_ATTACK_NAME;
    //    }

    //    if (Input.GetButtonDown("Dart"))
    //    {
    //        attackComponent.Dart();
    //        return "Dart";
    //    }
    //    return null;
    //}

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
}
