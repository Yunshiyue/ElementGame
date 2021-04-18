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

public class PlayerAnim : myUpdate
{
    private Animator anim;
    private float xVelocity = 0;
    private float yVelocity = 0;
    private bool isOnGround;
    private bool isCrouch;
    
    private int skillType = 0;
    private int status;

    private int priorityInType = 10;
    private UpdateType updateType = UpdateType.Player;

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


    public void SetUseSkillType(int type)
    {
        skillType = type;
        anim.SetInteger("useSkillType", skillType);
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
