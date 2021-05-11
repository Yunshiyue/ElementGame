using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceHammerAttack : SpellAttackEvent
{
    private Collider2D iceHammerArea;
    private int iceHammerDamage = 1;
    private float iceHammerInterruptTime = 1f;
    private float iceHammerForce = 5f;
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        iceHammerArea = GetComponent<Collider2D>();

    }
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void IceHammerAttackEvent()
    {
        Debug.Log("IceHammerEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(iceHammerArea, iceHammerDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                //禁锢效果 无法移动
                a.BeatBack(transform, iceHammerInterruptTime, iceHammerForce);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
        }
    }
}
