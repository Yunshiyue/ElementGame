using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShieldMashSpell : Spell
{
    //private GameObject iceShield;
    private GameObject iceShieldColl;
    private GameObject iceShieldColl1;
    private GameObject iceShieldColl2;
    private GameObject iceShieldColl3;

    public const float ICE_SHIELD_TIME = 1.5f;

    private BoxCollider2D IceShieldMashFreezingZone;
    private IceAbility iceAbility;
    public override void Initialize()
    {
        base.Initialize();
        iceShieldColl = GameObject.Find("IceShieldCollider");
        iceShieldColl1 = GameObject.Find("IceShieldCollider1");
        iceShieldColl2 = GameObject.Find("IceShieldCollider2");
        iceShieldColl3 = GameObject.Find("IceShieldCollider3");
        //iceShield = GameObject.Find("IceShield");
        //iceShield.SetActive(false);
        //playerAnim.SetSpell(this, SkillType.IceShield);
        iceAbility = player.GetComponent<IceAbility>();
        IceShieldMashFreezingZone = GameObject.Find("IceShieldMashFreezingZone").GetComponent<BoxCollider2D>();
        if (IceShieldMashFreezingZone == null)
        {
            Debug.LogError("IceShieldMashFreezingZone为空");
        }

    }
    public override void Cast()
    {
        //如果已有冰盾碰撞体则暂禁用碰撞体
        if (iceShieldColl.activeSelf)
        {
            iceShieldColl.SetActive(false);
        }
        iceShieldColl1.SetActive(false);
        iceShieldColl2.SetActive(false);
        iceShieldColl3.SetActive(false);

        playerAnim.SetUseSkillType(SkillType.IceShieldMash);
        iceAbility.FreezingZone(IceShieldMashFreezingZone);
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
