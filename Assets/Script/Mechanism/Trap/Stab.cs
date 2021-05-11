/**
 * @Description: Stab类是地刺类，对碰撞到地刺的物体造成伤害（目前对玩家和敌人都会造成伤害）
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stab : Trap
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //使用string数组初始化canFight能够检测到的层
        canFight.Initiailize(targetLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //层级检测
        if (layerMasks.Contains(collision.gameObject.layer))
        {
            //检测是否可以被攻击
            CanBeFighted beFought;
            if (collision.TryGetComponent<CanBeFighted>(out beFought))
            {
                canFight.Attack(beFought, damage);
                Debug.Log(gameObject.name + "攻击到了" + beFought.gameObject.name);
            }
        }
    }
}
