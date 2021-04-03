using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witcher : Enemies
{
    private WitcherMovemet movementComponent;
    private WitcherAttack attackComponent;


    public override void Initialize()
    {
        enemyAnim = GetComponent<Animator>();

        movementComponent = GetComponent<WitcherMovemet>();
        if (movementComponent == null)
        {
            Debug.LogError("在Player中，没有找到MovementPlayer脚本！");
        }
        attackComponent = GetComponent<WitcherAttack>();
        if (attackComponent == null)
        {
            Debug.LogError("在Player中，没有找到AttackPlayer脚本！");
        }

        defenceComponent = GetComponent<DefenceEnemies>();
        if (defenceComponent == null)
        {
            Debug.LogError("在Player中，没有找到DefencePlayer脚本！");
        }
        //设置最大生命值
        defenceComponent.Initialize(5);
    }
    //移动流程

    //攻击流程

}
