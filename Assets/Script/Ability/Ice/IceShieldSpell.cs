using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShieldSpell : Spell
{
    //private GameObject iceShield;
     private GameObject iceShieldColl;
    public override void Initialize()
    {
        base.Initialize();
        iceShieldColl = GameObject.Find("IceShieldCollider");
        //iceShield = GameObject.Find("IceShield");
        //iceShield.SetActive(false);
        //playerAnim.SetSpell(this, SkillType.IceShield);

    }
    public override void Cast()
    {
        //如果已有冰盾碰撞体则暂禁用碰撞体
        if (iceShieldColl.activeSelf)
            iceShieldColl.SetActive(false);

        playerAnim.SetUseSkillType(SkillType.IceShield);
    }

    public override void Disable()
    {

    }

    public override void Enable()
    {

    }

    public override void ReleaseSpell()
    {
            ////如果已有冰墙碰撞体则消毁
            //if (iceShieldColl.activeSelf) iceShieldColl.SetActive(false);
            ////设置冰墙碰撞体位置
            //iceShieldColl.transform.position = new Vector3((float)(player.transform.localScale.x * 1.5 + player.transform.position.x), (float)(player.transform.position.y + 0.05), 0);
            ////冰墙出现动画
            //iceShield.SetActive(true);   
    }
}
