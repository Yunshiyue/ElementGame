using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShieldMashAttack : SpellAttackEvent
{
    //冰盾 消失由帧事件或再次施放完成
    //private GameObject iceShield;
    //private GameObject iceShieldCollS;
    private GameObject iceShieldColl1;
    private GameObject iceShieldColl2;
    private GameObject iceShieldColl3;
    private Collider2D iceShieldAttackArea1;
    private Collider2D iceShieldAttackArea2;
    private Collider2D iceShieldAttackArea3;
    private int iceShieldDamage = 1;
    private float iceShieldUseTime = 0.3f;
    private float iceShieldInterruptTime = 0.4f;
    private float iceShieldContinueTime = 1f;
    private float iceShieldForce = 2f;
    private int curColl = 1;
    private Collider2D iceShieldAttackArea;
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        //冰盾
        //iceShield = gameObject.transform.Find("IceShield").gameObject;
        //iceShieldAttackArea = gameObject.GetComponent<Collider2D>();
        //iceShieldCollS = GameObject.Find("IceShieldColliderS");
        iceShieldColl1 = GameObject.Find("IceShieldCollider1");
        iceShieldColl2 = GameObject.Find("IceShieldCollider2");
        iceShieldColl3 = GameObject.Find("IceShieldCollider3");
        iceShieldAttackArea1 = iceShieldColl1.GetComponents<Collider2D>()[1];
        iceShieldAttackArea2 = iceShieldColl2.GetComponents<Collider2D>()[1];
        iceShieldAttackArea3 = iceShieldColl3.GetComponents<Collider2D>()[1];
    }
    private void Start()
    {
        gameObject.SetActive(false);
        //iceShieldCollS.SetActive(false);
    }
    //调整碰撞体位置
    public void IceShield1AppearEvent()
    {
        //确定冰墙位置
        iceShieldColl1.transform.position = new Vector3((float)(player.transform.localScale.x * 1.5 + player.transform.position.x), (float)(player.transform.position.y + 0.05), 0);
        //冰盾碰撞体可用
        iceShieldColl1.SetActive(true);

        curColl = 1;
    }
    public void IceShield2AppearEvent()
    {
        //确定冰墙位置
        iceShieldColl2.transform.position = new Vector3((float)(player.transform.localScale.x * 2.5 + player.transform.position.x), (float)(player.transform.position.y + 0.05), 0);
        //冰盾碰撞体可用
        iceShieldColl2.SetActive(true);
        curColl = 2;
    }
    public void IceShield3AppearEvent()
    {
        //确定冰墙位置
        iceShieldColl3.transform.position = new Vector3((float)(player.transform.localScale.x * 3.5 + player.transform.position.x), (float)(player.transform.position.y + 0.05), 0);
        //冰盾碰撞体可用
        iceShieldColl3.SetActive(true);
        curColl = 3;
    }
    //攻击事件代码
    public void IceShieldAttackEvent()
    {
        Debug.Log("IceShieldAttackEvent");

        switch (curColl)
        {
            case 1:
                iceShieldAttackArea = iceShieldAttackArea1;
                break;
            case 2:
                iceShieldAttackArea = iceShieldAttackArea2;
                break;
            case 3:
                iceShieldAttackArea = iceShieldAttackArea3;
                break;
        }

        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(iceShieldAttackArea, iceShieldDamage);
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






