using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Spell
{
    protected GameObject player;
    protected PlayerAnim playerAnim;

    
    //技能施法 火雷冰风
    public enum SkillType { Null,
                            FireBall,Meteorite,FireThunder,ProtectiveFireBall,Lava,
                            ThunderBall,ThunderLong,ThunderFire,ThunderIce,ThunderWind,
                            IceSword,IceArrow,IceHammer,IceShield,IceFire,IceThunder,IceWind,
                            WindShort, Hurricane,WindFire, WindThunder,WindIce }

    public virtual void Initialize()
    {
        player = GameObject.Find("Player");
        playerAnim = player.GetComponent<PlayerAnim>();

    }
    //当该技能为主角当前可释放技能时调用，加载技能所需要的资源
    abstract public void Enable();
    //当该技能不为主角当前可是放技能时调用，释放技能所需要的资源
    abstract public void Disable();
    //调用施法动画
    abstract public void Cast();
    //帧事件回调函数
    abstract public void ReleaseSpell();
}
