using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(SlimerMovement))]
public class SlimerAttack : MonoBehaviour
{
    CanFight canFight;
    SlimerMovement movementComponent;
    private string targetLayerName = "Player";
    //攻击打断
    private float interruptTime = 0.2f;
    private Vector2 interruptVector = new Vector2(0.5f, 0);
    void Start()
    {
        canFight = GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
        }

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);

        movementComponent = GetComponent<SlimerMovement>();
        if (movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Transform player = collision.transform;
        if (player.gameObject.name == "Player")
        {
            //Debug.Log("碰到了主角！");
            canFight.Attack(collision.transform.GetComponent<CanBeFighted>(), 2, AttackInterruptType.WEAK);
            MovementPlayer movement = player.GetComponent<MovementPlayer>();
            if (movement.RequestChangeControlStatus(interruptTime, MovementPlayer.PlayerControlStatus.Interrupt))
            {
                if (player.position.x > transform.position.x)
                {
                    movement.RequestMoveByTime(interruptVector, interruptTime, MovementPlayer.MovementMode.Attacked);
                }
                else
                {
                    movement.RequestMoveByTime(-interruptVector, interruptTime, MovementPlayer.MovementMode.Attacked);
                }
            }
        }
    }

}
