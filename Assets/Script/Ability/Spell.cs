/**
 * @Description: 所有技能类的父类，既不是mono也不是update
 *               向子类提供四个接口：
 *               Cast：激活技能对应的施法动画，使主角进入施法动画
 *               ReleaseSpell：释放技能效果，当主角施法动画结束时，通过playerAnim回调该方法，实现施法结束后释放技能
 *               Enable：当主角切换元素到本技能时调用，加载该技能所需要的资源
 *               Disable：当主角切换到别的元素时调用，释放该技能所需要的资源
 *               向子类提供Player变量和playerAnim变量
 * @Author: ridger

 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Spell
{
    protected GameObject player;
    protected PlayerAnim playerAnim;

    
    //技能施法 火雷冰风
    public enum SkillType { Null,
                            FireBall,Meteorite,FireThunder,ProtectiveFireBall,RocketPack,RemoteControlBomb, Lava, MeteorShower, SelfExplosion,
                            ThunderBall,ThunderLong,ThunderFire,ThunderIce,ThunderWind, ThunderElf, T1, T2, T3,
                            IceSword,IceArrow,IceHammer,IceShield,IceFire,IceThunder,IceBlink,IceShot,IceShieldMash,IceHammerDance,IceHeal, I1, I2, I3,
                            WindShort, Hurricane,WindFire, WindThunder,WindIce,WindField, W1, W2, W3 }

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
