using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderElfSpell : Spell
{
    private bool isOn = false;
    private ThunderElf thunderElf;
    private float totalTime;
    private float curTime;
    public override void Initialize()
    {
        base.Initialize();
        thunderElf = GameObject.Find("ThunderElf").GetComponent<ThunderElf>();
        totalTime = thunderElf.GetTotalTime();
        thunderElf.gameObject.SetActive(false);

        playerAnim.SetSpell(this, SkillType.ThunderElf);
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.ThunderElf);
    }
    public override void ReleaseSpell()
    {
        isOn = true;
        thunderElf.gameObject.SetActive(true);
        thunderElf.transform.position = player.transform.position;
    }
    public void ThunderElfClock()
    {
        if(isOn)
        {
            curTime += Time.deltaTime;
            thunderElf.AutoAttack();
            if(curTime >= totalTime)
            {
                isOn = false;
                curTime = 0f;
                thunderElf.gameObject.SetActive(false);
            }
        }
    }

    public override void Disable()
    {
    }

    public override void Enable()
    {
    }

}
