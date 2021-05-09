/**
 * @Description: 元素管理组件，负责玩家施法长短按的判定、元素点的消耗和回复、元素及技能切换，以及技能函数的调用
 * @Author: ridger

 * 
 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

abstract public class SpellAttackEvent : MonoBehaviour
{
    protected CanFight canFight;
    protected CanBeFighted[] targets;
    protected StringBuilder targetsName = new StringBuilder();
    protected GameObject player;
    protected virtual void Awake()
    {
        player = GameObject.Find("Player");
        
        canFight = player.GetComponent<CanFight>();
    }
    
}
