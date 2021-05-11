using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceThunderAttack : SpellAttackEvent
{
    private Collider2D iceThunderArea;
    private int iceThunderDamage = 1;
    private float iceThunderInterruptTime = 1f;
    private GameObject leftIceThunder;
    private GameObject rightIceThunder;
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        iceThunderArea = GameObject.Find("IceThunderCollider").GetComponent<Collider2D>();

        leftIceThunder = GameObject.Find("LeftIceThunder");
        rightIceThunder = GameObject.Find("RightIceThunder");
        
    }
    private void Start()
    {
        leftIceThunder.SetActive(false);
        rightIceThunder.SetActive(false);
    }
    //攻击事件代码
    public void IceThunderEvent()
    {
        Debug.Log("IceThunderEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(iceThunderArea, iceThunderDamage, AttackInterruptType.NONE, ElementAbilityManager.Element.Ice);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //禁锢效果 无法移动
                a.Encompass(iceThunderInterruptTime);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
    }
}
