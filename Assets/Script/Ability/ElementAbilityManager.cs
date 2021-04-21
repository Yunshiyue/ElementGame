using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class ElementAbilityManager : MonoBehaviour
{
    public const float DEFALT_CASTING_TIME = 0.3f;
    private float fullyCastTime = 0.3f;
    public enum Element { Fire, Ice, Wind, Thunder, NULL }
    private Element mainElement;
    private Element aElement;
    private Element bElement;

    private FireAbility fireAbility;
    private ThunderAbility thunderAbility;
    private IceAbility iceAbility;
    private WindAbility windAbility;

    private Ability mainAbility;
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

    }
    private void Start()
    {
        SwitchElement(Element.Fire, Element.Ice, Element.Wind);
    }
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
    public void SwitchElement(Element m, Element a = Element.NULL, Element b = Element.NULL)
    {
        mainElement = m;
        aElement = a;
        bElement = b;

        switch(m)
        {
            case Element.Fire:
                mainAbility = fireAbility;
                fireAbility.Activate(a, b);
                thunderAbility.DisActivate();
                iceAbility.DisActivate();
                windAbility.DisActivate();
                break;
            case Element.Thunder:
                mainAbility = thunderAbility;
                thunderAbility.Activate(a, b);
                fireAbility.DisActivate();
                iceAbility.DisActivate();
                windAbility.DisActivate();
                break;
            case Element.Ice:
                mainAbility = iceAbility;
                iceAbility.Activate(a, b);
                fireAbility.DisActivate();
                thunderAbility.DisActivate();
                windAbility.DisActivate();
                break;
            case Element.Wind:
                mainAbility = windAbility;
                windAbility.Activate(a, b);
                fireAbility.DisActivate();
                thunderAbility.DisActivate();
                iceAbility.DisActivate();
                break;
        }
    }

    private int aElementPoint = 999;
    private int bElementPoint = 999;

    private bool isLastFrameCasting = false;
    private bool isAddFirstElement = false;
    private bool isAddSecondElement = false;

    private float currentCastingTime = 0f;
    private int otherElementCost = 1;

    private Text debugInfoUI;
    private StringBuilder debugInfo = new StringBuilder(512);
    public void AbilityControl(bool isRequestMainElement, bool isRequestFirstOtherElement, bool isRequestSecondOtherElement)
    {
        //如果上一帧没有施法，则直接释放辅助技能
        if (!isLastFrameCasting)
        {
            if (isRequestFirstOtherElement && canConsumeElement(1, 1))
            {
                //剩余元素点够不够
                consumeElement(1, otherElementCost);
                //释放辅助元素A技能
                ASpell();
                return;
            }
            if (isRequestSecondOtherElement && canConsumeElement(2, 1))
            {
                consumeElement(2, otherElementCost);
                //释放辅助元素B技能
                BSpell();
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
            //Debug.Log("请求施法！");
            //请求施法成功，开始/继续施法
            if (mainAbility.Casting())
            {
                currentCastingTime += Time.deltaTime;
                isLastFrameCasting = true;

                if (currentCastingTime >= fullyCastTime)
                {
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
                if (isLastFrameCasting)
                {
                    Debug.Log("施法被打断！");
                    resumeElement();
                    isAddFirstElement = false;
                    isAddSecondElement = false;
                    isLastFrameCasting = false;
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
                    //其中调用sightHead的getposition方法
                    FullySpell();
                }
                else
                {
                    ShortMainSpell();
                    //短暂施法，归还蓄力元素
                    resumeElement();
                }
                isAddFirstElement = false;
                isAddSecondElement = false;
                isLastFrameCasting = false;
                currentCastingTime = 0f;
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
        debugInfo.Append("firstOtherElementPoint: ");
        debugInfo.AppendLine(aElementPoint.ToString());
        debugInfo.Append("secondOtherElementPoint: ");
        debugInfo.AppendLine(bElementPoint.ToString());

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

        debugInfoUI.text = debugInfo.ToString();
        debugInfo.Clear();
    }
    private bool canConsumeElement(int otherElementIndex, int consumeAmount)
    {
        switch (otherElementIndex)
        {
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
    private void consumeElement(int otherElementIndex, int consumeAmount)
    {
        switch (otherElementIndex)
        {
            case 1:
                aElementPoint -= consumeAmount;
                return;
            case 2:
                bElementPoint -= consumeAmount;
                return;
        }
    }
    private void resumeElement()
    {
        if (isAddFirstElement)
        {
            aElementPoint++;
        }
        if (isAddSecondElement)
        {
            bElementPoint++;
        }
    }


    public void Casting()
    {
        mainAbility.Casting();
    }
    public void ShortMainSpell()
    {
        mainAbility.ShortSpell();
    }
    public void FullySpell()
    {
        if (!isAddFirstElement && !isAddSecondElement)
        {
            mainAbility.FullySpell(Element.NULL, Element.NULL);
        }
        else if (isAddFirstElement && !isAddSecondElement)
        {
            mainAbility.FullySpell(aElement, Element.NULL);
        }
        else if (!isAddFirstElement && isAddSecondElement)
        {
            mainAbility.FullySpell(Element.NULL, bElement);
        }
        else
        {
            mainAbility.FullySpell(aElement, bElement);
        }
    }
    public void ASpell()
    {

    }
    public void BSpell()
    {

    }
    public void BigMainSpell()
    {

    }
}

