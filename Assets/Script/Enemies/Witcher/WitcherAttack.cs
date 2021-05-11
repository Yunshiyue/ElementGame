using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(MovementWitcher))]
public class WitcherAttack : AttackEnemies
{
    //CanFight canFight;
    MovementWitcher movementComponent;
    private string targetLayerName = "Player";
   
    protected PoolManager poolManager;
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

        movementComponent = GetComponent<MovementWitcher>();
        if (movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }

        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();

        //用于行为树的一些参数
        canActiveAttack = true;
        attackLength = 6f;
    }
    public override void AttackControl()
    {
        
        PushFire();
    }
    public void PushFire()
    {
        if (movementComponent.RequestChangeControlStatus(0.05f, MovementEnemies.EnemyStatus.AbilityWithNoMovement))
        {
            movementComponent.RecoverScale();
            movementComponent.enemyAnim.SetBool("firing", true);
            //帧事件 FireBall
            //Debug.Log("fire");
        }
    }
    private Vector2 flyingDirection = new Vector2(1, 0);
    public void FireBall()
    {
       
        GameObject fireBallAbility = poolManager.GetGameObject("FireBallAbility");
        FireBallAbility a = fireBallAbility.GetComponent<FireBallAbility>();
        a.SetThrower(gameObject);
        a.SetStartPosition(transform.position);
        
        flyingDirection.x = transform.localScale.x;
        a.SetDirection(flyingDirection);
        a.SetTargetLayerName("Player");
        a.SetDamage(2);
        
        movementComponent.enemyAnim.SetBool("firing", false);
        //rb.bodyType = RigidbodyType2D.Dynamic;
    }


}
