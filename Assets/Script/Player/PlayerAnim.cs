/**
 * @Description: 人物动画机 处理player动画,player调用该类的Set方法提供数据进行动画处理
 *               
 * @Author: 夜里猛

 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerAnim : myUpdate
{
    //public GameObject cameraMachine;
    private Animator anim;
    private float xVelocity = 0;
    private float yVelocity = 0;
    private bool isOnGround;
    private bool isCrouch;
    
    private int skillTypeToInt = 0;
    private Spell.SkillType skillType = Spell.SkillType.Null;
    private int status;

    private int priorityInType = 10;
    private UpdateType updateType = UpdateType.Player;

    //特效物体
    private GameObject fireThunderEffect;
    private GameObject selfExplosionEffect;
    private GameObject leftIceThunder;
    private GameObject rightIceThunder;
    private GameObject thunderIceSpecial;
    private GameObject iceShield;
    private GameObject iceHammer;
    private GameObject iceMashShield;
    private GameObject iceHeal;
    private GameObject windThunder;
    private GameObject windShort;
    private Cinemachine.CinemachineImpulseSource MyInpulse;

    public Transform cameraTransform;

    private void Awake()
    {
        fireThunderEffect = GameObject.Find("FireThunder");
        selfExplosionEffect = GameObject.Find("SelfExplosion");
        leftIceThunder = GameObject.Find("LeftIceThunder");
        rightIceThunder = GameObject.Find("RightIceThunder");
        thunderIceSpecial = GameObject.Find("ThunderIceSpecial");
        iceShield = GameObject.Find("IceShield");
        iceHammer = GameObject.Find("IceHammer");
        iceMashShield = GameObject.Find("IceMashShield");
        iceHeal = GameObject.Find("IceHeal");
        windThunder = GameObject.Find("WindThunder");
        windShort = GameObject.Find("WindShortMain");

        //相机抖动
        //MyInpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        cameraTransform = GameObject.Find("Main Camera").transform;

    }
    /// <summary>
    /// 提供动画机x方向速度
    /// </summary>
    /// <param name="x"></param>
    public void SetXvelocity(float x)
    {
        xVelocity = x;
    }

    /// <summary>
    /// 提供动画机y方向速度
    /// </summary>
    /// <param name="x"></param>
    public void SetYvelocity(float y)
    {
        yVelocity = y;
    }
    /// <summary>
    /// 提供动画机：是否在地面上
    /// </summary>
    /// <param name="x"></param>
    public void SetIsOnGround(bool on)
    {
        isOnGround = on;
    }
    /// <summary>
    /// 提供动画机：当前人物状态
    /// </summary>
    /// <param name="x"></param>
    public void SetStatus(MovementPlayer.PlayerControlStatus ControlStatus)
    {
        
        switch (ControlStatus)
        {
            case MovementPlayer.PlayerControlStatus.Normal:
                status = 0;
                break;
            case MovementPlayer.PlayerControlStatus.Crouch:
                status = 1;
                break;
            case MovementPlayer.PlayerControlStatus.AbilityWithMovement:
            case MovementPlayer.PlayerControlStatus.AbilityNeedControl:
                status = 2;
                break;
            case MovementPlayer.PlayerControlStatus.Interrupt:
                status = 3;
                break;
            case MovementPlayer.PlayerControlStatus.Stun:
                status = 4;
                break;
        }

        anim.SetInteger("status", status);
    }


    public void SetUseSkillType(Spell.SkillType type)
    {
        skillType = type;
        skillTypeToInt = (int)type;
        Debug.Log("skilType:" + skillTypeToInt);
        anim.SetInteger("useSkillType", skillTypeToInt);
    }
    public void EffectActive()
    {
        //cameraMachine.transform.rotation= Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //MyInpulse.GenerateImpulse();
        cameraTransform.DOShakeRotation(1,new Vector3(1,1,0));
        switch (skillType)
        {
            case Spell.SkillType.FireBall:
                fireBall.ReleaseSpell();
                break;
            case Spell.SkillType.Meteorite:
                meteorite.ReleaseSpell();
                break;
            case Spell.SkillType.FireThunder:
                fireThunderEffect.SetActive(true);
                break;
            case Spell.SkillType.ProtectiveFireBall:
                protectiveFireBall.ReleaseSpell();
                break;
            case Spell.SkillType.RocketPack:
                rocketPackSpell.ReleaseSpell();
                break;
            case Spell.SkillType.RemoteControlBomb:
                remoteControlBomb.ReleaseSpell();
                break;
            case Spell.SkillType.Lava:
                lava.ReleaseSpell();
                break;
            case Spell.SkillType.MeteorShower:
                meteorShowerSpell.ReleaseSpell();
                break;
            case Spell.SkillType.SelfExplosion:
                selfExplosionEffect.SetActive(true);
                selfExplosionSpell.ReleaseSpell();
                break;
            case Spell.SkillType.ThunderBall:
                thunderBall.ReleaseSpell();
                break;
            case Spell.SkillType.ThunderLong:
                thunderLong.ReleaseSpell();
                break;
            case Spell.SkillType.ThunderFire:
                thunderFire.ReleaseSpell();
                break;
            case Spell.SkillType.ThunderIce:
                thunderIceSpecial.SetActive(true);
                break;
            case Spell.SkillType.ThunderWind:
                //thunderIceSpecial.SetActive(true);
                break;
            case Spell.SkillType.ThunderElf:
                thunderElfSpell.ReleaseSpell();
                break;
            case Spell.SkillType.IceSword:
                iceSword.ReleaseSpell();
                break;
            case Spell.SkillType.IceArrow:
                iceArrow.ReleaseSpell();
                //thunderIceSpecial.SetActive(true);
                break;
            case Spell.SkillType.IceHammer:
                iceHammer.SetActive(true);
                break;
            case Spell.SkillType.IceShield:
                iceShield.SetActive(true);
                break;
           case Spell.SkillType.IceFire:
                iceFire.ReleaseSpell();
                break;
            case Spell.SkillType.IceThunder:
                leftIceThunder.SetActive(true);
                rightIceThunder.SetActive(true);
                break;
            case Spell.SkillType.IceBlink:
                iceBlink.ReleaseSpell();
                break;
            case Spell.SkillType.IceShot:
                iceShotSpell.ReleaseSpell();
                break;
            case Spell.SkillType.IceShieldMash:
                Debug.Log("mash!");
                iceMashShield.SetActive(true);
                break;
            case Spell.SkillType.IceHeal:
                iceHealSpell.ReleaseSpell();
                iceHeal.SetActive(true);
                break;
            case Spell.SkillType.WindShort:
                windShort.SetActive(true);
                break;
            case Spell.SkillType.Hurricane:
                hurricane.ReleaseSpell();
                break;
            case Spell.SkillType.WindFire:
                windFire.ReleaseSpell();
                break;      
            case Spell.SkillType.WindThunder:
                windThunder.SetActive(true);
                break;
            case Spell.SkillType.WindIce:
                windIce.ReleaseSpell();
                break;
            case Spell.SkillType.WindField:
                windField.ReleaseSpell();
                break;
            default:
                Debug.Log("该技能尚未完成！");
                break;
        }
        anim.SetInteger("useSkillType", 0);
    }
    private Spell fireBall;
    private Spell meteorite;
    private Spell thunderLong;
    private Spell protectiveFireBall;
    private Spell rocketPackSpell;
    private Spell remoteControlBomb;
    private Spell meteorShowerSpell;
    private Spell lava;
    private Spell selfExplosionSpell;
    private Spell thunderBall;
    private Spell thunderFire;
    private Spell thunderWind;
    private Spell thunderElfSpell;
    private Spell iceSword;
    private Spell iceArrow;
    private Spell iceFire;
    private Spell iceBlink;
    private Spell iceShotSpell;
    private Spell iceHealSpell;
    private Spell hurricane;
    private Spell windFire;
    private Spell windIce;
    private Spell windField;
    
    public void SetSpell(Spell spell, Spell.SkillType type)
    {
        switch (type)
        {
            case Spell.SkillType.FireBall:
                fireBall = spell;
                break;
            case Spell.SkillType.Meteorite:
                meteorite = spell;
                break;
            case Spell.SkillType.FireThunder:
                break;
            case Spell.SkillType.ProtectiveFireBall:
                protectiveFireBall = spell;
                break;
            case Spell.SkillType.RocketPack:
                rocketPackSpell = spell;
                break;
            case Spell.SkillType.RemoteControlBomb:
                remoteControlBomb = spell;
                break;
            case Spell.SkillType.Lava:
                lava = spell;
                break;
            case Spell.SkillType.MeteorShower:
                meteorShowerSpell = spell;
                break;
            case Spell.SkillType.SelfExplosion:
                selfExplosionSpell = spell;
                break;
            case Spell.SkillType.ThunderLong:
                thunderLong = spell;
                break;
            case Spell.SkillType.ThunderBall:
                thunderBall = spell;
                break;
            case Spell.SkillType.ThunderFire:
                thunderFire = spell;
                break;
            case Spell.SkillType.ThunderElf:
                thunderElfSpell = spell;
                break;
            case Spell.SkillType.IceSword:
                iceSword = spell;
                break; 
            case Spell.SkillType.IceArrow:
                iceArrow = spell;
                break;
            case Spell.SkillType.IceFire:
                iceFire = spell;
                break;
            case Spell.SkillType.IceBlink:
                iceBlink = spell;
                break;
            case Spell.SkillType.IceShot:
                iceShotSpell = spell;
                break;
            case Spell.SkillType.IceHeal:
                iceHealSpell = spell;
                break;
            case Spell.SkillType.Hurricane:
                hurricane = spell;
                break;
            case Spell.SkillType.WindFire:
                windFire = spell;
                break;
            case Spell.SkillType.WindIce:
                windIce = spell;
                break;
            case Spell.SkillType.WindField:
                windField = spell;
                break;
            default:
                Debug.Log("该技能尚未完成！");
                break;
        }
    }

    public override void Initialize()
    {
        anim = GetComponent<Animator>();
       
        //GetLengthByName("UseSkillByPush");
        //GetLengthByName("UseSkillBySinging");

    }

    public override void MyUpdate()
    {
        anim.SetFloat("running", Mathf.Abs(xVelocity));
        anim.SetBool("crouching", isCrouch);
        anim.SetBool("isOnGround", isOnGround);
        anim.SetFloat("verticalVelocity", yVelocity);
        //anim.SetInteger("status", status);
        //anim.SetInteger("abilityNum", abilityNum);
        //anim.SetInteger("useSkillType", skillType);
    }

    public void GetLengthByName(string name)
    {
        float length = 0;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(name))
            {
                length = clip.length;
                break;
            }
        }
        Debug.Log(name+":"+length);
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }

    //实时检测以显示水盾
    //private void CheckAndShowShield()
    //{
    //    if(defencePlayer.IsSieldUp())
    //    {
    //        waterShield.SetActive(true);
    //    }
    //    else
    //    {
    //        waterShield.SetActive(false);
    //    }
    //}

    // Start is called before the first frame update
    //void Start()
    //{
    //    anim = GetComponent<Animator>();

    //    //movement = GetComponent<PlayerMovement>();
    //    //rb = GetComponent<Rigidbody2D>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    anim.SetFloat("running", Mathf.Abs(xVelocity));
    //    anim.SetBool("crouching", isCrouch);
    //    anim.SetBool("isOnGround", isOnGround);
    //    anim.SetFloat("verticalVelocity", yVelocity);
    //    anim.SetInteger("status", status);
    //    anim.SetInteger("abilityNum", abilityNum);
    //}
}
