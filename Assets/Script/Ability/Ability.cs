/**
 * @Description: 元素接口，规定了元素类应该能够：
 *               a. 被激活，并指定激活的技能
 *               b. 被休眠，休眠所有技能冰释放资源
 *               c. 蓄力，某些元素在蓄力状态下有特殊效果
 *               d. 短按施法
 *               e. 长按施法
 * @Author: ridger

 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface Ability
{
    void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement);
    void DisActivate();
    bool Casting(bool isFullySpelt, ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement);
    void ShortSpell();
    void FullySpell(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement);
    int NextAuxiliarySpellCost();
    void AuxiliarySpell();
}
