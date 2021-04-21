using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderLongSpell : Spell
{
    private ThunderAbility thunderAbility;
    private GameObject thunderLongMain;
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.ThunderLong);
    }
   
    
    public override void Disable()
    {

    }

    public override void Enable()
    {

    }
    public override void Initialize()
    {
        base.Initialize();
        thunderAbility = player.GetComponent<ThunderAbility>();
        thunderLongMain = GameObject.Find("ThunderLongMain");
        playerAnim.SetSpell(this, SkillType.ThunderLong);
    }
    //雷主长按施法动画调用帧事件 SkillEvent()再调用 ThunderLongMainCheckEvent() 用于找到可以击中的目标
    public override void ReleaseSpell()
    {

        GameObject target = thunderAbility.GetClosestTargetInList(
            thunderAbility.GetTargetInThunderCircle());
        if (target != null)
        {
            //雷击启动
            thunderLongMain.transform.position = target.transform.position;
            thunderLongMain.SetActive(true);
            Debug.Log("雷主长按打到的怪物： " + target.name);
        }
        else
        {
            Debug.Log("雷圈范围内没有敌人");
        }
    }
}
