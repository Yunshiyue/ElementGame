/**
 * @Description: ElementMenu类是切换元素菜单界面的控制类
 * @Author: CuteRed
1-3-8 20:17
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementMenu : MonoBehaviour
{
    private Dictionary<int, ElementAbilityManager.Element> elements = 
        new Dictionary<int, ElementAbilityManager.Element>();

    /// <summary>
    /// 主元素下拉菜单
    /// </summary>
    private Dropdown mainDropdown;

    /// <summary>
    /// A元素下拉菜单
    /// </summary>
    private Dropdown aDropdown;

    /// <summary>
    /// B元素下拉菜单
    /// </summary>
    private Dropdown bDropdown;

    private Text elementTip;

    private ElementAbilityManager elementAbilityManager;

    private void Awake()
    {
        //初始化元素字典
        elements.Add(0, ElementAbilityManager.Element.Fire);
        elements.Add(1, ElementAbilityManager.Element.Ice);
        elements.Add(2, ElementAbilityManager.Element.Wind);
        elements.Add(3, ElementAbilityManager.Element.Thunder);
        elements.Add(4, ElementAbilityManager.Element.NULL);

        //初始化子选项
        mainDropdown = GameObject.Find("MainDropdown").GetComponent<Dropdown>();
        aDropdown = GameObject.Find("ADropdown").GetComponent<Dropdown>();
        bDropdown = GameObject.Find("BDropdown").GetComponent<Dropdown>();

        //初始化提示文本
        elementTip = GameObject.Find("ElementTipText").GetComponent<Text>();
        if (elementTip == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到ElementTipText组件");
        }
        //元素重复提示文本消失
        elementTip.gameObject.SetActive(false);

        //初始化技能管理器
        elementAbilityManager = GameObject.Find("Player").GetComponent<ElementAbilityManager>();
    }

    private void OnEnable()
    {
        //元素重复提示文本消失
        elementTip.gameObject.SetActive(false);
    }

    /// <summary>
    /// 确定元素
    /// </summary>
    public void ConfirmElement()
    {
        //有元素重复
        if (mainDropdown.value == aDropdown.value || mainDropdown.value == bDropdown.value ||
            (aDropdown.value == bDropdown.value && aDropdown.value != 4))
        {
            elementTip.gameObject.SetActive(true);
        }
        else
        {
            //换元素
            elementAbilityManager.SwitchElement
                (elements[mainDropdown.value], elements[aDropdown.value], elements[bDropdown.value]);

            //隐藏界面
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 更改下拉菜单
    /// </summary>
    public void RefreshItem()
    {
        //更改下拉菜单
        mainDropdown.value = (int)elementAbilityManager.GetMainElement();
        aDropdown.value = (int)elementAbilityManager.GetAElement();
        bDropdown.value = (int)elementAbilityManager.GetBElement();
    }
}