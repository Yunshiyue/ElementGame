using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderIceAttack : SpellAttackEvent
{
    private GameObject thunderIce;
    //private GameObject thunderIceSpecial;//施放特效
    private Collider2D thunderCircleArea;
    private int thunderIceDamage = 1;
    private float thunderIceInterruptTime = 1f;
    private Vector2 thunderIceInterruptVector = new Vector2(0f, 0f);
    private float thunderIceTime = 0.4f;
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        thunderIce = GameObject.Find("ThunderIce");
        thunderCircleArea = GameObject.Find("ThunderCircle").GetComponent<Collider2D>();
        //thunderIceSpecial = GameObject.Find("ThunderIceSpecial");
        thunderIce.transform.SetParent(null);
        
        
    }
    private void Start()
    {
        gameObject.SetActive(false);
        thunderIce.SetActive(false);
    }

    //攻击事件代码
    public void ThunderIceEvent()
    {
        Debug.Log("ThunderIceEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(thunderCircleArea, thunderIceDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //禁锢效果 无法移动
                //a.BeatBack(transform, thunderIceInterruptTime, thunderIceInterruptVector);
                a.Encompass(thunderIceInterruptTime);
                GameObject temp = Instantiate(thunderIce, a.transform.position, Quaternion.identity);
                //temp.GetComponent<Animator>().SetBool("effect", true);
                temp.SetActive(true);
            }
            Debug.Log("打到了： " + targetsName.ToString());
            targetsName.Clear();
        }
    }
}






