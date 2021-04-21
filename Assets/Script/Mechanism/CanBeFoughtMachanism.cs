/**
 * @Description: CanBeFoughtMachanism类是机关被击类，继承CanBeFighted类，主要用于检测机关的触发条件
 * @Author: CuteRed

 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeFoughtMachanism : CanBeFighted
{
    /// <summary>
    /// 该机关要接收的元素
    /// </summary>
    private Mechanism mechanism;

    private void Start()
    {
        mechanism = GetComponent<Mechanism>();
        if (mechanism == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到Machanism组件");
        }    
    }

    /// <summary>
    /// 判断攻击类型，并触发机关
    /// </summary>
    /// <param name="who"></param>
    /// <param name="damage"></param>
    /// <param name="interruptType"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public override int BeAttacked(GameObject who, int damage, AttackInterruptType interruptType, ElementAbilityManager.Element element = ElementAbilityManager.Element.NULL)
    {
        if (element == ElementAbilityManager.Element.Fire)
        {
            mechanism.Trigger(Mechanism.TiggerType.Fire);
        }
        else if (element == ElementAbilityManager.Element.Ice)
        {
            mechanism.Trigger(Mechanism.TiggerType.Ice);
        }
        else if (element == ElementAbilityManager.Element.Wind)
        {
            mechanism.Trigger(Mechanism.TiggerType.Wind);
        }
        else if (element == ElementAbilityManager.Element.Thunder)
        {
            mechanism.Trigger(Mechanism.TiggerType.Thunder);
        }

        return 0;
    }
}