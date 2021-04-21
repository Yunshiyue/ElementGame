using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindThunderAttack : SpellAttackEvent
{
    //风雷
    //private GameObject windThunder;
    private int windThunderAttackDamage = 1;
    public const string WIND_THUNDER_NAME = "WindThunder";
    private float windThunderInterruptTime = 0.2f;
    private float windThunderStatusTime = 0.2f;
    private Vector2 windThunderInterruptVector = new Vector2(10f, 0);
    private float windThunderInterruptForce = 3f;
    private MovementPlayer movementComponent;
    private DefencePlayer defenceComponent;
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        movementComponent = player.GetComponent<MovementPlayer>();
        defenceComponent = player.GetComponent<DefencePlayer>();
        
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    //攻击事件代码
    private float windThunderDistance = 7f;
    private bool isUnstoppable = false;//冲刺时是否有伤害
    
    private void UnstoppedEnd()
    {
        isUnstoppable = false;
        defenceComponent.SetImmune(false);
    }
    private void UnstoppedStart()
    {
        isUnstoppable = true;
        defenceComponent.SetImmune(true);
    }
    //风雷冲刺碰撞伤害
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("unstoppable");
        if (isUnstoppable)
        {

            if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemy")
            {
                Debug.Log("unstoppable!!!!");
                Transform enemy = collision.transform;
                canFight.Attack(enemy.GetComponent<CanBeFighted>(), windThunderAttackDamage, AttackInterruptType.WEAK);
                enemy.GetComponent<CanBeFighted>().BeatBack(transform, windThunderInterruptTime, windThunderInterruptForce);
            }
        }
    }
}






