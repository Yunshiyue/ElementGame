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
