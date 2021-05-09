using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(MovementSlimer))]
public class SlimerAttack : AttackEnemies
{
    //CanFight canFight;
    MovementSlimer movementComponent;
    private string targetLayerName = "Player";
   
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

        movementComponent = GetComponent<MovementSlimer>();
        if (movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }
    }


}
