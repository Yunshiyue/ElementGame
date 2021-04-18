using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface Ability
{
    void Activate(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement);
    void DisActivate();
    bool Casting();
    void ShortSpell();
    void FullySpell(ElementAbilityManager.Element aElement, ElementAbilityManager.Element bElement);
}
