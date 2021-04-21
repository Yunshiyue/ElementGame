/**
 * @Description: Trap类是陷阱类
 * @Author: CuteRed

 * 

 * @Editor: CuteRed
 * @Edit: 该类不再继承自Machanism类，需要重新设计（TODO）
 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Collider2D collider;

    [Header("伤害参数")]
    protected int damage = 1;
    public CanFight canFight;
    protected string targetLayerName = "Player";


    // Start is called before the first frame update
    void Awake()
    {
        canFight = GetComponent<CanFight>();

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);

        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取canFight组件时出错");
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
    public void Trigger()
    {
        //对区域内目标进行攻击
        canFight.AttackArea(collider, damage);
    }

    public void SetTargetLayerName(string layerName)
    {
        targetLayerName = layerName;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag == "Player")
    //    {
    //        //检测到玩家，造成伤害
    //        CanBeFighted beFought;
    //        if (collision.TryGetComponent<CanBeFighted>(out beFought))
    //        {
    //            Debug.Log("Trap对玩家造成伤害");
    //            canFight.Attack(beFought, damage);
    //        }        
    //    }
    //}
}
