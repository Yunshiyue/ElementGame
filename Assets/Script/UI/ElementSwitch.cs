/**
 * @Description: ElementSwitch类为负责显示元素的UI类，根据当前元素动态显示在UI上
 * @Author: CuteRed

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSwitch : MonoBehaviour
{
    private Dictionary<ElementAbilityManager.Element, Sprite> elements = 
        new Dictionary<ElementAbilityManager.Element, Sprite>();

    [Header("元素")]
    public Sprite fire;
    public Sprite wind;
    public Sprite thunder;
    public Sprite ice;
    public Sprite nullElement;

    private Image mainElement;
    private Image aElement;
    private Image bElement;


    private void Awake()
    {
        //初始化字典
        elements.Add(ElementAbilityManager.Element.Fire, fire);
        elements.Add(ElementAbilityManager.Element.Ice, ice);
        elements.Add(ElementAbilityManager.Element.Wind, wind);
        elements.Add(ElementAbilityManager.Element.Thunder, thunder);
        elements.Add(ElementAbilityManager.Element.NULL, nullElement);


        mainElement = GameObject.Find("MainElement").GetComponent<Image>();
        if (mainElement == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MainElement");
        }

        aElement = GameObject.Find("AElement").GetComponent<Image>();
        if (aElement == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到AElement");
        }

        bElement = GameObject.Find("BElement").GetComponent<Image>();
        if (bElement == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到BElement");
        }
    }

    /// <summary>
    /// 切换元素
    /// </summary>
    /// <param name="main">主元素</param>
    /// <param name="a">A元素</param>
    /// <param name="b">B元素</param>
    public void Switch(ElementAbilityManager.Element main, ElementAbilityManager.Element a, ElementAbilityManager.Element b)
    {
        //元素更换
        mainElement.overrideSprite = elements[main];
        aElement.overrideSprite = elements[a];
        bElement.overrideSprite = elements[b];

        //Debug.Log("切换元素");
    }
}
