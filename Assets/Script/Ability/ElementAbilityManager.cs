/**
 * @Description: 元素管理组件，负责玩家施法长短按的判定、元素点的消耗和回复、元素及技能切换，以及技能函数的调用
 * @Author: ridger

 *

 * @Editor: CuteRed
 * @Edit: 增加了元素切换UI组件，新增3个元素的Get方法
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class ElementAbilityManager : MonoBehaviour
{
    //默认施法时间
    public const float DEFALT_CASTING_TIME = 0.3f;
    //完全蓄力所需要的时间
    public static float fullyCastTime = 0.3f;
    private bool isFullySpelt = false;
    public enum Element { Fire, Ice, Wind, Thunder, NULL }
    private Element mainElement = Element.Fire;
    private Element aElement = Element.Ice;
    private Element bElement = Element.Wind;

    private FireAbility fireAbility;
    private ThunderAbility thunderAbility;
    private IceAbility iceAbility;
    private WindAbility windAbility;

    private Ability mainAbility;

    /// <summary>
    /// 元素切换UI
    /// </summary>
    private ElementSwitch elementSwitch;
    private Ability aAbility;
    private Ability bAbility;
    private void Awake()
    {
        fireAbility = GetComponent<FireAbility>();
        thunderAbility = GetComponent<ThunderAbility>();
        iceAbility = GetComponent<IceAbility>();
        windAbility = GetComponent<WindAbility>();

        debugInfoUI = GameObject.Find("PlayerAbilityDebugInfo").GetComponent<Text>();
        if (debugInfoUI == null)
        {
            Debug.LogError("没找到PlayerAbilityDebugInfo这个ui物体");
        }

        elementSwitch = GameObject.Find("ElementSwitch").GetComponent<ElementSwitch>();
        if (elementSwitch == null)
        {
            Debug.LogError("没找到elementSwitch这个ui物体");
        }

    }
    //设置初始元素
    private void Start()
    {
        SwitchElement(Element.Fire, Element.Thunder, Element.Wind);
        //Debug.Log("a:" + aAbility.ToString());
    }
    //顺序切换元素接口
    public void NextMainElement()
    {
        SwitchElement((Element)(((int)mainElement + 1) % ((int)Element.NULL)), aElement, bElement);
    }
    public void NextAElement()
    {
        SwitchElement(mainElement, (Element)(((int)aElement + 1) % ((int)Element.NULL + 1)), bElement);
    }
    public void NextBElement()
    {
        SwitchElement(mainElement, aElement, (Element)(((int)bElement + 1) % ((int)Element.NULL + 1)));
    }
    //切换元素，需要将各元素的休眠，然后激活主元素的对应技能
    public void SwitchElement(Element m, Element a = Element.NULL, Element b = Element.NULL)
    {
        mainElement = m;
        aElement = a;
        bElement = b;

        //更新UI
        elementSwitch.Switch(m, a, b);

        mainElementPoint = originElementPoint;
        aElementPoint = originElementPoint;
        bElementPoint = originElementPoint;

        switch(m)
        {
            case Element.Fire:
                mainAbility = fireAbility;
                fireAbility.DisActivate();
                thunderAbility.DisActivate();
                iceAbility.DisActivate();
                windAbility.DisActivate();
                fireAbility.Activate(a, b);
                break;
            case Element.Thunder:
                mainAbility = thunderAbility;
                thunderAbility.DisActivate();
                thunderAbility.Activate(a, b);
                fireAbility.DisActivate();
                iceAbility.DisActivate();
                windAbility.DisActivate();
                break;
            case Element.Ice:
                mainAbility = iceAbility;
                iceAbility.DisActivate();
                iceAbility.Activate(a, b);
                fireAbility.DisActivate();
                thunderAbility.DisActivate();
                windAbility.DisActivate();
                break;
            case Element.Wind:
                mainAbility = windAbility;
                windAbility.DisActivate();
                windAbility.Activate(a, b);
                fireAbility.DisActivate();
                thunderAbility.DisActivate();
                iceAbility.DisActivate();
                break;
        }
        switch(a)
        {
            case Element.Fire:
                aAbility = fireAbility;
                break;
            case Element.Ice:
                aAbility = iceAbility;
                break;
            case Element.Thunder:
                aAbility = thunderAbility;
                break;
            case Element.Wind:
                aAbility = windAbility;
                break;
            case Element.NULL:
                aAbility = null;
                break;
        }
        switch (b)
        {
            case Element.Fire:
                bAbility = fireAbility;
                break;
            case Element.Ice:
                bAbility = iceAbility;
                break;
            case Element.Thunder:
                bAbility = thunderAbility;
                break;
            case Element.Wind:
                bAbility = windAbility;
                break;
            case Element.NULL:
                bAbility = null;
                break;
        }
    }
    //剩余的元素点
    private int mainElementPoint;
    private int aElementPoint;
    private int bElementPoint;
    private int originElementPoint = 999;

    //控制蓄力逻辑
    private bool isLastFrameCasting = false;
    private bool isAddFirstElement = false;
    private bool isAddSecondElement = false;
    private bool isAddMainElement = false;

    private float currentCastingTime = 0f;
    private int otherElementCost = 1;

    private Text debugInfoUI;
    private StringBuilder debugInfo = new StringBuilder(512);
    public void AbilityControl(bool isRequestMainElement, bool isRequestFirstOtherElement, bool isRequestSecondOtherElement)
    {
        //如果上一帧没有施法，则直接释放辅助技能
        if (!isLastFrameCasting)
        {
            if (isRequestFirstOtherElement && aAbility != null && canConsumeElement(1, aAbility.NextAuxiliarySpellCost()))
            {
                //剩余元素点够不够
                consumeElement(1, aAbility.NextAuxiliarySpellCost());
                //释放辅助元素A技能
                aAbility.AuxiliarySpell();
                //记录释放了A技能
                StatisticsCollector.ASpellTime++;
                //Debug.Log("释放了A");
                return;
            }
            if (isRequestSecondOtherElement && bAbility != null && canConsumeElement(2, bAbility.NextAuxiliarySpellCost()))
            {
                consumeElement(2, aAbility.NextAuxiliarySpellCost());
                //释放辅助元素B技能
                bAbility.AuxiliarySpell();
                StatisticsCollector.BSpellTime++;
                //Debug.Log("释放了B");
                return;
            }
        }
        //如果上一帧正在施法，则加入辅助元素
        else
        {
            if (isRequestFirstOtherElement && aElement != Element.NULL && canConsumeElement(1, 1) && !isAddFirstElement)
            {
                consumeElement(1, otherElementCost);
                isAddFirstElement = true;
                //播放消耗元素动画
            }
            if (isRequestSecondOtherElement && bElement != Element.NULL && canConsumeElement(2, 1) && !isAddSecondElement)
            {
                consumeElement(2, otherElementCost);
                isAddSecondElement = true;
                //播放消耗元素动画
            }
        }
        //有没有按下主元素请求继续施法/开始施法
        if (isRequestMainElement)
        {
            //Debug.Log("请求施法！");
            //请求施法成功，开始/继续施法
            if (mainAbility.Casting(isFullySpelt, isAddFirstElement ? aElement : Element.NULL, isAddSecondElement ? bElement : Element.NULL))
            {
                currentCastingTime += Time.deltaTime;
                isLastFrameCasting = true;

                if (currentCastingTime >= fullyCastTime)
                {
                    isFullySpelt = true;
                    if (!isAddMainElement)
                    {
                        if (canConsumeElement(0, 1))
                        {
                            consumeElement(0, 1);
                            isAddMainElement = true;
                            //施法完成动画
                        }
                        else
                        {
                            //主元素点不足，施法结束
                            mainAbility.ShortSpell();
                            StatisticsCollector.shortSpellTime++;
                            StatisticsCollector.wantFullyButFailedTimeForElementNotEnough++;
                            Debug.Log("主元素点不足，施法结束");
                            resumeElement();
                            isAddFirstElement = false;
                            isAddSecondElement = false;
                            isAddMainElement = false;
                            isLastFrameCasting = false;
                            isFullySpelt = false;
                            currentCastingTime = 0f;
                            mainAbility.Casting(isFullySpelt, isAddFirstElement ? aElement : Element.NULL, isAddSecondElement ? bElement : Element.NULL);
                            return;
                        }
                    }
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
                if (isLastFrameCasting)
                {
                    Debug.Log("施法被打断！");
                    StatisticsCollector.wantFullyButFailedTimeForBeInterrupted++;
                    resumeElement();
                    isAddFirstElement = false;
                    isAddSecondElement = false;
                    isAddMainElement = false;
                    isLastFrameCasting = false;
                    isFullySpelt = false;
                    mainAbility.Casting(isFullySpelt, isAddFirstElement ? aElement : Element.NULL, isAddSecondElement ? bElement : Element.NULL);
                    currentCastingTime = 0f;
                }
            }
        }
        //没有请求施法，进入主动中断/啥也不干
        else
        {
            //如果上一帧正在施法，则主动中断，法术放出
            if (isLastFrameCasting)
            {
                if (currentCastingTime >= fullyCastTime)
                {
                    StatisticsCollector.RecordSpell(currentCastingTime);
                    //其中调用sightHead的getposition方法
                    FullySpell();
                }
                else
                {
                    StatisticsCollector.shortSpellTime++;
                    ShortMainSpell();
                    //短暂施法，归还蓄力元素
                    resumeElement();
                }
                isAddFirstElement = false;
                isAddSecondElement = false;
                isAddMainElement = false;
                isLastFrameCasting = false;
                isFullySpelt = false;
                currentCastingTime = 0f;
                mainAbility.Casting(isFullySpelt, isAddFirstElement ? aElement : Element.NULL, isAddSecondElement ? bElement : Element.NULL);
            }
            //如果上一帧没有施法，则啥也不干
        }
    }

    public void SetCastDebugInfo()
    {
        debugInfo.AppendLine("元素搭配");
        debugInfo.Append("mainElement: ");
        debugInfo.AppendLine(mainElement.ToString());
        debugInfo.Append("firstOtherElement: ");
        debugInfo.AppendLine(aElement.ToString());
        debugInfo.Append("secondOtherElement: ");
        debugInfo.AppendLine(bElement.ToString());

        debugInfo.AppendLine("元素点");
        debugInfo.Append("mainElementPoint: ");
        debugInfo.AppendLine(mainElementPoint.ToString());
        debugInfo.Append("firstOtherElementPoint: ");
        debugInfo.AppendLine(aElementPoint.ToString());
        debugInfo.Append("secondOtherElementPoint: ");
        debugInfo.AppendLine(bElementPoint.ToString());

        debugInfo.AppendLine("其他参数");
        debugInfo.Append("currentCastingTime: ");
        debugInfo.AppendLine(currentCastingTime.ToString());

        debugInfo.Append("isLastFrameCasting: ");
        debugInfo.AppendLine(isLastFrameCasting.ToString());
        debugInfo.Append("isAddMainElement: ");
        debugInfo.AppendLine(isAddMainElement.ToString());
        debugInfo.Append("isAddFirstElement: ");
        debugInfo.AppendLine(isAddFirstElement.ToString());
        debugInfo.Append("isAddSecondElement: ");
        debugInfo.AppendLine(isAddSecondElement.ToString());
        debugInfo.Append("isRequestMainElement: ");

        debugInfoUI.text = debugInfo.ToString();
        debugInfo.Clear();
    }
    private bool canConsumeElement(int elementIndex, int consumeAmount)
    {
        switch (elementIndex)
        {
            case 0:
                if(mainElementPoint >= consumeAmount)
                {
                    return true;
                }
                return false;
            case 1:
                if (aElementPoint >= consumeAmount)
                {
                    return true;
                }
                return false;
            case 2:
                if (bElementPoint >= consumeAmount)
                {
                    return true;
                }
                return false;
        }
        return false;
    }
    private void consumeElement(int elementIndex, int consumeAmount)
    {
        switch (elementIndex)
        {
            case 0:
                mainElementPoint -= consumeAmount;
                switch(mainElement)
                {
                    case Element.Fire: StatisticsCollector.fireElementConsumption++; break;
                    case Element.Thunder: StatisticsCollector.thunderElementConsumption++; break;
                    case Element.Ice: StatisticsCollector.iceElementConsumption++; break;
                    case Element.Wind: StatisticsCollector.windElementConsumption++; break;
                }
                return;
            case 1:
                aElementPoint -= consumeAmount;
                switch (aElement)
                {
                    case Element.Fire: StatisticsCollector.fireElementConsumption++; break;
                    case Element.Thunder: StatisticsCollector.thunderElementConsumption++; break;
                    case Element.Ice: StatisticsCollector.iceElementConsumption++; break;
                    case Element.Wind: StatisticsCollector.windElementConsumption++; break;
                }
                return;
            case 2:
                bElementPoint -= consumeAmount;
                switch (bElement)
                {
                    case Element.Fire: StatisticsCollector.fireElementConsumption++; break;
                    case Element.Thunder: StatisticsCollector.thunderElementConsumption++; break;
                    case Element.Ice: StatisticsCollector.iceElementConsumption++; break;
                    case Element.Wind: StatisticsCollector.windElementConsumption++; break;
                }
                return;
        }
    }
    private void resumeElement()
    {
        if (isAddFirstElement)
        {
            aElementPoint++;
            switch (aElement)
            {
                case Element.Fire: StatisticsCollector.fireElementConsumption--; break;
                case Element.Thunder: StatisticsCollector.thunderElementConsumption--; break;
                case Element.Ice: StatisticsCollector.iceElementConsumption--; break;
                case Element.Wind: StatisticsCollector.windElementConsumption--; break;
            }
        }
        if (isAddSecondElement)
        {
            bElementPoint++; 
            switch (bElement)
            {
                case Element.Fire: StatisticsCollector.fireElementConsumption--; break;
                case Element.Thunder: StatisticsCollector.thunderElementConsumption--; break;
                case Element.Ice: StatisticsCollector.iceElementConsumption--; break;
                case Element.Wind: StatisticsCollector.windElementConsumption--; break;
            }
        }
        if(isAddMainElement)
        {
            mainElementPoint++;
            switch (mainElement)
            {
                case Element.Fire: StatisticsCollector.fireElementConsumption--; break;
                case Element.Thunder: StatisticsCollector.thunderElementConsumption--; break;
                case Element.Ice: StatisticsCollector.iceElementConsumption--; break;
                case Element.Wind: StatisticsCollector.windElementConsumption--; break;
            }
        }
    }
    


    public void ShortMainSpell()
    {
        mainAbility.ShortSpell();
    }
    public void FullySpell()
    {
        if (!isAddFirstElement && !isAddSecondElement)
        {
            StatisticsCollector.MSpellTime++;
            mainAbility.FullySpell(Element.NULL, Element.NULL);
        }
        else if (isAddFirstElement && !isAddSecondElement)
        {
            StatisticsCollector.MASpellTime++;
            mainAbility.FullySpell(aElement, Element.NULL);
        }
        else if (!isAddFirstElement && isAddSecondElement)
        {
            StatisticsCollector.MBSpellTime++;
            mainAbility.FullySpell(Element.NULL, bElement);
        }
        else
        {
            StatisticsCollector.MABSpellTime++;
            mainAbility.FullySpell(aElement, bElement);
        }
    }

    public void BigMainSpell()
    {

    }

    public Element GetMainElement()
    {
        return mainElement;
    }

    public Element GetAElement()
    {
        return aElement;
    }

    public Element GetBElement()
    {
        return bElement;
    }

    public void RestoreElement(Element type, int point)
    {
        if(mainElement == type)
        {
            mainElementPoint += point;
        }
        else if(aElement == type)
        {
            aElementPoint += point;
        }
        else if(bElement == type)
        {
            bElementPoint += point;
        }
    }
}

