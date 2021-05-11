using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShieldAttack : SpellAttackEvent
{
    //冰盾 消失由帧事件或再次施放完成
    //private GameObject iceShield;
    private GameObject iceShieldColl;
    private Collider2D iceShieldAttackArea;
    private int iceShieldDamage = 1;
    private float iceShieldUseTime = 0.3f;
    private float iceShieldInterruptTime = 0.4f;
    private float iceShieldContinueTime = 1f;
    private float iceShieldForce = 2f;
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        //冰盾
        //iceShield = gameObject.transform.Find("IceShield").gameObject;
        //iceShieldAttackArea = gameObject.GetComponent<Collider2D>();
        iceShieldColl = GameObject.Find("IceShieldCollider");
        iceShieldAttackArea = iceShieldColl.GetComponents<Collider2D>()[1];

    }
    private void Start()
    {
        gameObject.SetActive(false);
        //iceShieldColl.SetActive(false);
    }
    //调整碰撞体位置
    public void IceShieldAppearEvent()
    {
        //确定冰墙位置
        iceShieldColl.transform.position = new Vector3((float)(player.transform.localScale.x * 1.5 + player.transform.position.x), (float)(player.transform.position.y + 0.05), 0);
        iceShieldColl.SetActive(true);
        Debug.Log("icecoll:"+iceShieldColl.activeSelf);
    }
    //攻击事件代码
    public void IceShieldAttackEvent()
    {
        
        Debug.Log("IceShieldAttackEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(iceShieldAttackArea, iceShieldDamage, AttackInterruptType.NONE, ElementAbilityManager.Element.Ice);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //击退效果 
                a.BeatBack(transform, iceShieldInterruptTime, iceShieldForce);
                //a.BeatBack(transform,iceShieldUseTime, iceShieldVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
        
    }
}






