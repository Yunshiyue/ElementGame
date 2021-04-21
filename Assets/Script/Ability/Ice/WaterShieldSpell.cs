using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShieldSpell : Spell
{

    //水盾技能
    private float WSDurationTime = 8f;
    private int waterShieldPoint = 2;
    private int waterShieldHealPoint = 1;
    private DefencePlayer defencePlayer;
    private bool isWaterShieldClockOn = false;
    private float WSCurTime = 0f;

    private GameObject waterShield;

    public void WaterShieldClock()
    {
        if (isWaterShieldClockOn)
        {
            WSCurTime += Time.deltaTime;
            if (WSCurTime >= WSDurationTime)
            {
                if (defencePlayer.IsSieldUp())
                {
                    defencePlayer.Heal(waterShieldHealPoint);
                }
                defencePlayer.ShieldDown();
                waterShield.SetActive(false);
                isWaterShieldClockOn = false;
                WSCurTime = 0f;
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        defencePlayer = player.GetComponent<DefencePlayer>();

        waterShield = GameObject.Find("WaterShield");
        waterShield.SetActive(false);
        if (waterShield == null)
        {
            Debug.Log("在PlayerAnim中没有找到waterShield!");
        }
        playerAnim.SetSpell(this,SkillType.IceFire);

    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceFire);
    }
    public override void ReleaseSpell()
    {
        defencePlayer.ShieldUp(waterShieldPoint);
        waterShield.SetActive(true);
        //开启计时器
        isWaterShieldClockOn = true;
    }

    public override void Disable()
    {
    }

    public override void Enable()
    {
    }
}
