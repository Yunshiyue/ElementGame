/**
 * @Description: Trap类是陷阱类
 * @Author: CuteRed

 * 

 * @Editor: CuteRed
 * @Edit: 该类不再继承自Machanism类，需要重新设计（TODO）
 * 

 * @Editor: CuteRed
 * @Edit: 该类重新设计完成，加入了可攻击层级的string数组，同时变为abstact类
 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    private Collider2D coll;

    [Header("伤害参数")]
    public int damage = 1;
    /// <summary>
    /// 存放陷阱的攻击层级
    /// </summary>
    public string[] targetLayer;
    /// <summary>
    /// 存放攻击层级的LayerMask
    /// </summary>
    protected List<LayerMask> layerMasks = new List<LayerMask>();
    protected CanFight canFight;
    protected string playerName = "Player";
    protected string enemyName = "Enemy";


    // Start is called before the first frame update
    protected virtual void Start()
    {
        //初始化CanFight
        canFight = GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取canFight组件时出错");
        }

        //初始化碰撞体
        coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到collider");
        }

        //检查攻击层级
        if (targetLayer.Length == 0)
        {
            Debug.LogError("在" + gameObject.name + "中，攻击层级数组未初始化");
        }

        //初始化攻击层级LayerMask列表
        for (int i = 0; i < targetLayer.Length; i++)
        {
            layerMasks.Add(LayerMask.NameToLayer(targetLayer[i]));
        }
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
