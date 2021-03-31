using UnityEngine;

/// <summary>
/// 表示攻击打断类型，分为无打断、弱打断、强打断
/// </summary>
public enum AttackInterruptType { NONE, WEAK, STRONG }

/// <summary>
/// 记录每个攻击的信息
/// </summary>
public struct AttackContent
{
    public GameObject who;
    public int damage;
    public AttackInterruptType interruptType;
    public AttackContent(GameObject who, int damage, AttackInterruptType interruptType)
    {
        this.who = who;
        this.damage = damage;
        this.interruptType = interruptType;
    }

}