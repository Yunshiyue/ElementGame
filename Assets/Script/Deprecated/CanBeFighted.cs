/**
 * @Description: CanBeFighted类是所有能够“被攻击”的单位所拥有的组件。在每一帧中记录每次被攻击的信息，以数组方式储存。
 * @Author: ridger

 *           
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeFighted : MonoBehaviour
{
    //如果处于无敌状态，则任何受击效果失效
    private bool isImmune = false;
    /// <summary>
    /// 受击信息数组最大值，指代一帧中最大的被攻击的次数
    /// </summary>
    public const int ATTACK_LIST_MAX_SIZE = 32;
    /// <summary>
    /// 在这一帧里是否受到过攻击
    /// </summary>
    private bool isAttacked = false;
    /// <summary>
    /// 受击信息数组，记录这一帧中攻击对象、伤害值以及打断类型(强打断、弱打断、无打断)
    /// </summary>
    private AttackContent[] beAttackedList = new AttackContent[ATTACK_LIST_MAX_SIZE];
    /// <summary>
    /// 指向受击信息数组尾部的指针
    /// </summary>
    private int attackListPointer = 0;

    //private CanBeFightedManager magener;

    /// <summary>
    /// 被攻击时调用，将攻击信息记录到受击信息数组中去
    /// </summary>
    /// <param name="who">伤害来源</param>
    /// <param name="damage">造成的伤害，并不判定是否合法</param>
    /// <returns>返回具体造成的伤害</returns>
    public int BeAttacked(GameObject who, int damage, AttackInterruptType interruptType)
    {
        if(!isImmune)
        {
            if (attackListPointer >= ATTACK_LIST_MAX_SIZE)
            {
                Debug.LogError("在" + gameObject.name + "物体中，在一帧中beAttacked数量超过上限" + ATTACK_LIST_MAX_SIZE);
                attackListPointer = 0;
            }
            isAttacked = true;
            beAttackedList[attackListPointer] = new AttackContent(who, damage, interruptType);
            attackListPointer++;
            Debug.Log(gameObject.name + "收到了来自" + who.name + "的攻击，栈顶指针为" + attackListPointer);
        
            return damage;
        }
        return 0;
    }

    /// <summary>
    /// 被击退函数
    /// </summary>
    /// <param name="attackerPos">攻击者</param>
    /// <param name="interruptTime">打断时间</param>
    /// <param name="interruptVector">击退力的方向</param>
    public void BeatBack(Transform attackerPos, float interruptTime, Vector2 interruptVector)
    {
        if(!isImmune) { 
            //获取移动组件
            //如果被打的是player
            if (gameObject.name == "Player")
            {
                MovementPlayer movement = gameObject.GetComponent<MovementPlayer>();
                if (movement.RequestChangeControlStatus(interruptTime, MovementPlayer.PlayerControlStatus.Interrupt))
                {
                    if (transform.position.x > attackerPos.position.x)
                    {
                        movement.RequestMoveByTime(interruptVector, interruptTime, MovementPlayer.MovementMode.Attacked);
                    }
                    else
                    {
                        movement.RequestMoveByTime(-interruptVector, interruptTime, MovementPlayer.MovementMode.Attacked);
                    }
                }
            }
            else 
            {
                MovementEnemies movement = gameObject.GetComponent<MovementEnemies>();
                if (movement.RequestChangeControlStatus(interruptTime, MovementEnemies.EnemyStatus.Stun))
                {               
                    if (transform.position.x > attackerPos.position.x)
                    {
                        movement.RequestMoveByTime(interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
                    }
                    else
                    {
                        movement.RequestMoveByTime(-interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// 清空受击信息，应该在每一帧受击结算之后被调用
    /// </summary>
    public void Clear()
    {
        isAttacked = false;
        attackListPointer = 0;
    }
    public void SetImmune(bool a)
    {
        isImmune = a;
    }
    public int getAttackNum() { return attackListPointer; }
    public bool hasBeenAttacked() { return isAttacked; }
    public AttackContent[] GetAttackedList() { return beAttackedList; }

}
