using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderLongAttack : SpellAttackEvent
{
    //private GameObject thunderLongMain;
    private Collider2D thunderLongMainArea;
    private int thunderLongMainDamage = 1;
    private float thunderLongMainInterruptTime = 0.5f;
    private Vector2 thunderLongMainInterruptVector = new Vector2(2.5f, 0f);
    public const string THUNDER_LONG_MAIN_NAME = "ThunderLongMain";
    // 加载collider
    protected override void Awake()
    {
        base.Awake();
        gameObject.transform.SetParent(null);//解除与父物体的关系，防止释放位置受影响
        thunderLongMainArea = gameObject.GetComponent<Collider2D>();
        
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    //攻击事件代码
    //雷主长按技能攻击事件
    public void ThunderLongMainAttackEvent()
    {
        targets = canFight.AttackArea(thunderLongMainArea, thunderLongMainDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //击退效果
                a.BeatBack(transform, thunderLongMainInterruptTime, thunderLongMainInterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
        else
        {
            Debug.Log("雷圈范围内没有敌人");
        }
    }
}






