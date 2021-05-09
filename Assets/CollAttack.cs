using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollAttack : MonoBehaviour
{
    public GameObject enemy;
    private CanFight canFight;
    private AttackEnemies attackComponent;
    //攻击打断
    public float interruptTime;
    public float interruptVectorX;
    public float interruptVectorY;
    public int damage;
    private Vector2 interruptVector = new Vector2(0.5f, 0);
    private void Start()
    {
        canFight = enemy.GetComponent<CanFight>();
        attackComponent = enemy.GetComponent<AttackEnemies>();
        interruptVector.x = interruptVectorX;
        interruptVector.y = interruptVectorY;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform player = collision.transform;
        if (player.gameObject.name == "Player")
        {
            //Debug.Log("碰到了主角！");
            canFight.Attack(player.GetComponent<CanBeFighted>(), damage, AttackInterruptType.WEAK);
            attackComponent.isHitPlayer = true;
            player.GetComponent<CanBeFighted>().BeatBack(transform, interruptTime, interruptVector);

        }
    }
}
