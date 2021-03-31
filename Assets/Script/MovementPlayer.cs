using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : myUpdate
{
    private int priorityInType = 4;

    private void Awake()
    {
        Debug.Log("第" + x + "遍执行");
        x++;
        GameObject.Find("UpdateManager").GetComponent<UpdateManager>().Register(this);
    }
    private int x = 1;


    public UpdateType updateType = UpdateType.Player;
    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
    public override int GetPriorityInType()
    {
        Debug.Log("movementPlayer中，返回值" + priorityInType);
        return priorityInType;
    }

    public bool isGravity = true;
    public float gravity = 5.0f;
    public float jumpForce = 5.0f;
    public float crouchDivision = 3.0f;

    //移动方法 玩家控制 攻击后坐力 移动技能 挨打
    public enum MovementMode { PlayerControl, Ability, Attacked }

    public enum PlayerControlStatus { Normal, Crouch, AbilityWithMovement, AbilityNeedControl, Interrupt, Stun}
    private PlayerControlStatus controlStatus = PlayerControlStatus.Normal;
    private bool isInAbnormalStatus = false;

    private float controlStatusTotalTime = 0f;
    private float controlStatusCurTime = 0f;

    private bool isOnFloor = true;
    private bool isDownFloor = false;

    private bool isAttackedFalling = false;

    //下蹲相关
    private bool isRequestCrouch = false;
    public void RequestCrouch()
    {
        isRequestCrouch = true;
    }

    //起跳相关
    private bool isRequestJump = false;
    private int jumpNumberMax = 2;
    private int jumpNumberCur = 0;
    public void RequestJump()
    {
        isRequestJump = true;
    }

    override public void Initialize()
    {
    }

    //眩晕保持动画，眩晕解除时回复
    //在空中受到伤害进入下坠状态，直到着地时解除
    //起跳
    override public void MyUpdate()
    {
        //如果当前状态不是Normal或Crouch，则时间++，
        if(isInAbnormalStatus)
        {
            //如果在空中处于挨打，则处于受击下落状态，同时改变Interrupt时间为正无穷，直到落地才退出该状态
            if (controlStatus == PlayerControlStatus.Interrupt && !isOnFloor)
            {
                isAttackedFalling = true;
                controlStatusTotalTime = float.MaxValue;
            }
            //如果处于受击下落状态，且在这一帧着地，则进入普通状态(后面可以考虑改动为进入落地动画)
            if (isAttackedFalling && isOnFloor)
            {
                isAttackedFalling = false;
                ChangeControlStatus(0f, PlayerControlStatus.Normal);
            }

            //状态时间++
            controlStatusCurTime += Time.deltaTime;
            //状态到期发生的事情
            if (controlStatusCurTime >= controlStatusTotalTime)
            {
                ChangeControlStatus(0f, PlayerControlStatus.Normal);
            }
        }
        //在处理正常状态
        else
        {
            //如果在地面上
            if(isOnFloor)
            {
                //处理下蹲逻辑
                //如果在地面上按下了下蹲键，或者，没有按下下蹲，但是蹲的时候头上有东西，则保持下蹲
                if (isRequestCrouch || !isRequestCrouch && controlStatus == PlayerControlStatus.Crouch && isDownFloor)
                {
                    ChangeControlStatus(0f, PlayerControlStatus.Crouch);
                }
                //否则恢复Normal
                else
                {
                    ChangeControlStatus(0f, PlayerControlStatus.Normal);
                }
            }
        }

        //处理起跳逻辑
        //在某些技能状态下，允许玩家控制运动状态，我们也应该允许跳跃
        //跳跃计数在clear函数中恢复
        if(controlStatus == PlayerControlStatus.Normal || controlStatus == PlayerControlStatus.AbilityNeedControl)
        {
            if(isRequestJump && jumpNumberCur < jumpNumberMax)
            {
                ySpeed += jumpForce;
                jumpNumberCur++;
            }
        }

        //结算运动情况
        if(isPassiveTransport)
        {
            xMovementPerFrame += activeTransportPosition.x;
            yMovementPerFrame += activeTransportPosition.y;
        }
        else if(isActiveTransport)
        {
            xMovementPerFrame += passiveTransportPosition.x;
            yMovementPerFrame += passiveTransportPosition.y;
        }
        else if(isAbilityMovement)
        {
            xSpeed += abilityMovementSpeed.x;
            ySpeed += abilityMovementSpeed.y;

            abilityMovementCurTime += Time.deltaTime;
            if(abilityMovementCurTime >= abilityMovementTotalTime)
            {
                isAbilityMovement = false;
                abilityMovementCurTime = 0f;
                abilityMovementTotalTime = 0f;
            }
        }
        else if(isPassiveMovement)
        {
            xSpeed += passiveMovementSpeed.x;
            ySpeed += passiveMovementSpeed.y;

            passiveMovementCurTime += Time.deltaTime;
            if(passiveMovementCurTime >= passiveMovementTotalTime)
            {
                isPassiveMovement = false;
                passiveMovementCurTime = 0f;
                passiveMovementTotalTime = 0f;
            }
        }
        else if(isControllorMovement)
        {
            xSpeed += controllorMovement.x;
            ySpeed += controllorMovement.y;
        }

        //结算减速列表

        if(controlStatus == PlayerControlStatus.Crouch)
        {
            xSpeed /= crouchDivision;
        }

        //结算重力
        if(isGravity)
        {
            ySpeed -= gravity * Time.deltaTime;
        }

        xMovementPerFrame += xSpeed * Time.deltaTime;
        yMovementPerFrame += ySpeed * Time.deltaTime;

        newPosition.x = transform.position.x + xMovementPerFrame;
        newPosition.y = transform.position.y + yMovementPerFrame;
        transform.position = newPosition;

        Clear();
    }


    private void Clear()
    {
        isRequestCrouch = false;
        isRequestJump = false;
        if(isOnFloor)
        {
            jumpNumberCur = 0;
        }

        isPassiveTransport = false;
        isActiveTransport = false;
        isControllorMovement = false;

        xMovementPerFrame = 0f;
        yMovementPerFrame = 0f;
        xSpeed = 0f;
        ySpeed = 0f;
    }

    /// <summary>
    /// 探测器调用，设置主角是否在地面上
    /// </summary>
    /// <param name="sender">必须是主角的子物体才能生效</param>
    /// <param name="isOnFloor"></param>
    public void setOnFloor(GameObject sender, bool isOnFloor)
    {
        if(sender.transform.IsChildOf(transform))
        {
            this.isOnFloor = isOnFloor;
        }
        else
        {
            Debug.LogError("在" + gameObject.name + "中，非探测器物体调用了setOnFloor函数！");
        }
    }
    public void setDownFloor(GameObject sender, bool isDownFloor)
    {
        if (sender.transform.IsChildOf(transform))
        {
            this.isDownFloor = isDownFloor;
        }
        else
        {
            Debug.LogError("在" + gameObject.name + "中，非探测器物体调用了setDownFloor函数！");
        }
    }

    public bool RequestChangeControlStatus(float statusTime, PlayerControlStatus status)
    {
        switch(controlStatus)
        {
            case PlayerControlStatus.Normal:
                ChangeControlStatus(statusTime, status);
                return true;

            case PlayerControlStatus.Crouch:
                if( status == PlayerControlStatus.Normal ||
                    status == PlayerControlStatus.Interrupt ||
                    status == PlayerControlStatus.Stun ||
                    status == PlayerControlStatus.Crouch)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;

            case PlayerControlStatus.AbilityNeedControl:
            case PlayerControlStatus.AbilityWithMovement:
                if(status == PlayerControlStatus.Normal ||
                   status == PlayerControlStatus.Stun)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;

            //暂时设定打断\眩晕状态下不允许被打断，眩晕状态下眩晕允许且时间刷新
            case PlayerControlStatus.Interrupt:
            case PlayerControlStatus.Stun:
                if(status == PlayerControlStatus.Normal ||
                   status == PlayerControlStatus.Stun)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;
        }

        return false;
    }

    //传送统一使用local坐标系
    //被动传送 = 被动以帧为结算的位移
    private Vector2 passiveTransportPosition;
    private bool canPassiveTransport = false;
    private bool isPassiveTransport = false;

    //主动传送
    private Vector2 activeTransportPosition;
    private bool canActiveTransport = false;
    private bool isActiveTransport = false;

    //技能位移
    private Vector2 abilityMovement;
    private float abilityMovementTotalTime = 0f;
    private Vector2 abilityMovementSpeed = new Vector2(0,0);
    private float abilityMovementCurTime = 0f;
    private bool canAbilityMovement = false;
    private bool isAbilityMovement = false;

    //被动位移
    private Vector2 passiveMovement;
    private Vector2 passiveMovementSpeed = new Vector2(0, 0);
    private float passiveMovementTotalTime = 0f;
    private float passiveMovementCurTime = 0f;
    private bool canPassiveMovement = false;
    private bool isPassiveMovement = false;

    //控制位移，以帧结算
    private Vector2 controllorMovement = new Vector2(0,0);
    private bool isControllorMovement = false;
    private bool canControllorMovement = true;

    //变速状态管理(加速、减速的计时结构)
    private const int SPEED_RATIO_LIST_MAX_SIZE = 32;

    private float[] speedRatioList = new float[SPEED_RATIO_LIST_MAX_SIZE];
    private float[] speedClockList = new float[SPEED_RATIO_LIST_MAX_SIZE];
    private float[] speedTimeList = new float[SPEED_RATIO_LIST_MAX_SIZE];
    private int speedListPointer = 0;

    //当前x\y轴向速度，结算之后进行改变
    private float xSpeed = 0f;
    private float ySpeed = 0f;
    private float xMovementPerFrame = 0f;
    private float yMovementPerFrame = 0f;
    private Vector2 newPosition = new Vector2(0, 0);

    public bool RequestMoveByTime(Vector2 movement, float time, MovementMode mode)
    {
        switch(mode)
        {
            case MovementMode.PlayerControl:
                return false;

            case MovementMode.Ability:
                if (canAbilityMovement)
                {
                    isAbilityMovement = true;
                    abilityMovement = movement;
                    abilityMovementTotalTime = time;
                    abilityMovementCurTime = 0f;
                    abilityMovementSpeed = movement / time;
                    return true;
                }
                return false;

            case MovementMode.Attacked:
                if(canPassiveMovement)
                {
                    isPassiveMovement = true;
                    passiveMovement = movement;
                    passiveMovementTotalTime = time;
                    passiveMovementCurTime = 0f;
                    passiveMovementSpeed = movement / time;
                    return true;
                }
                return false;
        }
        return false;
    }

    public bool RequestMoveByFrame(Vector2 movement, MovementMode mode)
    {
        switch(mode)
        {
            case MovementMode.PlayerControl: 
                if(canControllorMovement)
                {
                    isControllorMovement = true;
                    controllorMovement = movement;
                    return true;
                }
                return false;

            case MovementMode.Ability:
                if(canActiveTransport)
                {
                    isActiveTransport = true;
                    activeTransportPosition = movement;
                    return true;
                }
                return false;

            case MovementMode.Attacked:
                if (canPassiveTransport)
                {
                    isPassiveTransport = true;
                    passiveTransportPosition = movement;
                    return true;
                }
                return false;
        }

        return false;
    }


    /// <summary>
    /// 修改mask参数，修改状态计时器
    /// </summary>
    /// <param name="statusTime"></param>
    /// <param name="status"></param>
    private void ChangeControlStatus(float statusTime, PlayerControlStatus status)
    {
        //如果是需要计时的状态，则重置计时器
        if (status != PlayerControlStatus.Normal && status != PlayerControlStatus.Crouch)
        {
            isInAbnormalStatus = true;
            controlStatusTotalTime = statusTime;
            controlStatusCurTime = 0f;
        }
        //否则不需要重置计时器
        else
        {
            isInAbnormalStatus = false;
            controlStatusTotalTime = 0f;
            controlStatusCurTime = 0f;
        }
        controlStatus = status;

        switch (status)
        {
            case PlayerControlStatus.Normal:
                canPassiveTransport = true;
                canActiveTransport = true;
                canAbilityMovement = true;
                canPassiveMovement = true;
                canControllorMovement = true;
                break;
            case PlayerControlStatus.Crouch:
                canPassiveMovement = true;
                canActiveTransport = false;
                canAbilityMovement = false;
                canPassiveMovement = true;
                canControllorMovement = true;
                break;
            case PlayerControlStatus.AbilityWithMovement:
                canPassiveMovement = true;
                canActiveTransport = true;
                canAbilityMovement = true;
                canPassiveMovement = false;
                canControllorMovement = false;
                break;
            case PlayerControlStatus.AbilityNeedControl:
                canPassiveMovement = true;
                canActiveTransport = true;
                canAbilityMovement = true;
                canPassiveMovement = false;
                canControllorMovement = true;
                break;
            case PlayerControlStatus.Interrupt:
            case PlayerControlStatus.Stun:
                canPassiveMovement = true;
                canActiveTransport = false;
                canAbilityMovement = false;
                canPassiveMovement = true;
                canControllorMovement = false;
                break;
        }
    }

}
