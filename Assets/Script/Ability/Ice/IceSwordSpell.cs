using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSwordSpell : Spell
{
    public const float ICE_SWORD_TIME = 0.55f;
    //普通攻击范围、伤害、字符串标识(名字)
    private Collider2D normalAttackArea;
    private int normalAttackDamage = 1;
    public const string NORMAL_ATTACK_NAME = "NormalAttack";
    //该攻击打断敌人参数
    private float normalInterruptTime = 0.3f;
    private Vector2 normalInterruptVector = new Vector2(3f, 0);

    private CanFight canFight;
    private CanBeFighted[] targets;
    private string targetLayerName = "Enemy";

    private BoxCollider2D iceShotFreezingZone;
    private IceAbility iceAbility;
    //StringBuilder targetsName = new StringBuilder();
    public override void Initialize()
    {
        base.Initialize();
        normalAttackArea = GameObject.Find("NormalAttack").GetComponent<Collider2D>();
        canFight = player.GetComponent<CanFight>();
        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);
        playerAnim.SetSpell(this, SkillType.IceSword);

        iceAbility = player.GetComponent<IceAbility>();
        iceShotFreezingZone = GameObject.Find("IceShotFreezingZone").GetComponent<BoxCollider2D>();
        if (iceShotFreezingZone == null)
        {
            Debug.LogError("iceShotFreezingZone为空");
        }
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceSword);
        iceAbility.FreezingZone(iceShotFreezingZone);
    }

    public override void Disable()
    {
       
    }

    public override void Enable()
    {
        
    }

    public override void ReleaseSpell()
    {
        Debug.Log("AttackEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(normalAttackArea, normalAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                a.BeatBack(player.transform, normalInterruptTime, normalInterruptVector);
            }
            //Debug.Log("打到了： " + targetsName.ToString());
            //targetsName.Clear();
        }
    }    
}
