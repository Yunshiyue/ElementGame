using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireThunderAttack : SpellAttackEvent
{
    //火雷
    //private GameObject fireThunder;
    private PolygonCollider2D fireThunderAttackArea;
    private int fireThunderAttackDamage = 1;
    public const string FIRE_THUNDER_NAME = "FireThunder";
    //该技能攻击打断敌人参数
    private float fireThunderInterruptTime = 0.5f;
    private Vector2 fireThunderInterruptVector = new Vector2(4f, 0);
   
    protected override void Awake()
    {
        base.Awake();
        //fireThunder = GameObject.Find("FireThunder");
        fireThunderAttackArea = gameObject.transform.Find("FireThunderCollider").GetComponent<PolygonCollider2D>();
        
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    //火雷动画帧事件调用fireThunderEvent()
    public void FireThunderEvent()
    {
        Debug.Log("fireThunderEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(fireThunderAttackArea, fireThunderAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //击退效果
                a.BeatBack(transform, fireThunderInterruptTime, fireThunderInterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
    }
}
