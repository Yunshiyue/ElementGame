using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(MovementFlyingEye))]
public class FlyingEyeAttack : AttackEnemies
{
    //CanFight canFight;
    MovementFlyingEye movementComponent;
    private string targetLayerName = "Player";

    //咬咬攻击 技能1
    private Collider2D biteColl;
    private int biteDamage = 2;
    private float biteAttackTime = 0.5f;
    private float biteAttackInterruptTime = 0.5f;
    private Vector2 biteAttackInterruptVetcor = new Vector2(1, 0);

    //冲刺 技能2
    private float dashAttackTime = 1f;
   
    void Awake()
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

        movementComponent = GetComponent<MovementFlyingEye>();
        if (movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }

        biteColl =GetComponentsInChildren<Collider2D>()[1];

        //技能cd
        skillCD1 = 3f;
        skillCD2 = 8f;
    }

    public override void Skill1()
    {
        if (movementComponent.RequestChangeControlStatus(biteAttackTime, MovementEnemies.EnemyStatus.AbilityWithNoMovement))
        {
            isInAction = true;
            movementComponent.RecoverScale();
            movementComponent.enemyAnim.SetInteger("skillType", 1);
        }
    }

    public void BiteAttackEvent()
    {
        targets = canFight.AttackArea(biteColl, biteDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                //击退效果
                a.BeatBack(transform, biteAttackInterruptTime, biteAttackInterruptVetcor);
            }
        }
    }
    //冲刺
    public override void Skill2()
    {
        if (movementComponent.RequestChangeControlStatus(dashAttackTime, MovementEnemies.EnemyStatus.AbilityWithNoMovement))
        {
            isInAction = true;
            movementComponent.RecoverScale();
            movementComponent.enemyAnim.SetInteger("skillType", 2);
        }
    }

}
