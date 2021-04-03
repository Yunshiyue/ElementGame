/**
 * @Description: Trap类是陷阱类，为可触发机关的一种，是Machanism类的子类
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Machanism
{

    [Header("伤害参数")]
    protected int damage = 1;
    protected CanFight fight;


    // Start is called before the first frame update
    void Start()
    {
        fight = GetComponent<CanFight>();
        if (fight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取fight组件时出错");
        }

        if (collider == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到collider");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Trigger();
    }

    /// <summary>
    /// 触发机关，对范围内所有目标造成伤害
    /// </summary>
    public override void Trigger()
    {
        //对区域内目标进行攻击
        fight.AttackArea(collider, damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //检测到玩家，造成伤害
            CanBeFighted beFought;
            if (collision.TryGetComponent<CanBeFighted>(out beFought))
            {
                Debug.Log("Trap对玩家造成伤害");
                fight.Attack(beFought, damage);
            }
            
        }
    }
}
