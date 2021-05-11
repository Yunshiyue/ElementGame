/**
 * @Description: 主角的移动组件，继承自myUpdate类。主要负责对每一帧的控制状态改变请求和移动请求进行处理，根据每一帧
 *               的请求和状态计算出这一帧的player位移。并通知动画管理组件进行动画管理。
 *               
 * @Author: ridger

 * 

 * @Editor: ridger
 * @Eidt: 1.修正了下蹲请求的方式，变为了在控制组件中RequestChangeStatus
 *        2.调整了跳跃导致的y轴的速度计算方式：
 *              a.跳跃不再直接增加速度变化量，而是ySpeed = jumpForce + ySpeed * jumpOriRatio;
 *              b.下落速度设置最大下界yFallingMaxSpeed
 *              c.调整了重力参数和起跳力jumpForce的参数，使得手感优化
 *              d.跳跃次数改成了2次
 *              

 * @Editor: ridger
 * @Edit: 1.修正了blink会卡墙的bug，现在的算法是，每当有瞬间移动请求的时候，都会检测传送地点是否有墙体，如果有
 *          则向主角方向移动一点距离再次检测，直到没有墙体为止
 *          

 * @Editor: ridger
 * @Edit: 1.修改了y轴速度计算逻辑，只有当状态为Normal时，才会受到重力作用，同时y轴速度才会不进行更新。
 *          当状态位其他时，y轴速度计算方法与x轴速度一致，每帧清空为0，有多少速度改变量就为多少，同时不受重力作用。
 *          

 * @Editor: ridger
 * @Edit: 修正了异常转身的bug，只有在normal\crouch态下才回转身，后坐力和挨打动画不会导致转身了
 * 

 * @Editor: ridger
 * @Edit: 改变了申请传送的接口RequestMoveByFrame，增加了一个参数Space，用于说明传送的坐标系是世界坐标还是local坐标：
 *          a.如果是世界坐标，则直接传送到指定地点
 *          b.如果是local坐标，则传送到当前朝向下的相对位置，比如朝左就会相对向左传送，朝右就会向右传送
 *          

 * @Editor: ridger
 * @Edit: 1. 增加了SetYFloorOffset方法，让探测器调用来实现陷入地板的调整
 *        2. 在update逻辑中增加了调整陷入地板的逻辑，如果这一帧需要进行调整，而且当前状态为normal则进行距离补偿
 *        

 * @Edittor: 夜里猛
 * @Edit: 增加了设置重力的接口，同时，在主动、被动movement计时器到时间时，会将isGravity设为true
 * 

 * @Editor: ridger
 * @Edit: 修改了x轴y轴陷入墙中距离补偿的bug，现在只要检测到陷入墙中就会进行距离补偿，而不是和速度挂钩，主角不再会卡墙了
 * 

 * @Editor: CuteRed
 * @Edit: 增加了事件管理器，跳跃会触发事件
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class MovementPlayer : myUpdate
{
    //public部分
    //public GameObject cameraMachine;
    //unity编辑器中可以进行调试的参数，包括是否受重力、重力大小、跳跃初速度、下蹲的减速系数等
    private bool isGravity = true;
    private float gravity = 20.0f;
    private float DefalutGravity = 20f;
    private float jumpForce = 12.0f;
    //计算下一次跳跃时的速度，详见引用处
    private float jumpOriRatio = 0.15f;
    //下蹲后减速系数(除法)
    private float crouchDivision = 2f;
    //+1 = 2次
    private int jumpNumberMax = 1;
    //X轴速度倍数(乘法)
    private float xSpeedRatio = 3f;
    //Y轴最大速度
    private float yFallingMaxSpeed = -15f;
    //传送时，每隔该距离，检测有无墙体；
    private float transportStuckDetectionSnap = 0.2f;

    /// <summary>
    /// 游戏菜单管理组件
    /// </summary>
    private GameMenu gameMenu;

    private EventManager eventManager;
    private object[] jumpArray = new object[1];


    //该组件主要食用方法：通过RequestChangeControlStatus判断能否执行下一个状态，如果可以，通过RequestMove申请移动
    //请求执行下一个状态，switch-case分支列举了不同状态下，能够转换成哪些状态的规则(考虑表示成状态转移矩阵)
    //同时会改变can系列参数，控制在某种状态下能够接受哪些移动，不能接受哪些移动
    //Time参数表示该状态最多持续多长时间，如果时间到了则会转移成Normal状态
    //当status == 0时，表示单帧申请状态改变，现在只有申请持续施法会这样做
    public bool RequestChangeControlStatus(float statusTime, PlayerControlStatus status)
    {
        if(status == PlayerControlStatus.Crouch)
        {
            if(controlStatus == PlayerControlStatus.Normal ||
               controlStatus == PlayerControlStatus.Crouch)
            {
                isRequestCrouch = true;
                return true;
            }
        }
        switch (controlStatus)
        {
            case PlayerControlStatus.Normal:
                if(isOnFloor || isInWater)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                else
                {
                    if(status == PlayerControlStatus.Crouch)
                    {
                        return false;
                    }
                    else
                    {
                        ChangeControlStatus(statusTime, status);
                        return true;
                    }
                }

            //case PlayerControlStatus.InWater:
            //    if(status == PlayerControlStatus.Normal ||
            //       status == PlayerControlStatus.Crouch ||
            //       status == PlayerControlStatus.AbilityNeedControl ||
            //       status == PlayerControlStatus.AbilityWithMovement ||
            //       status == PlayerControlStatus.Stun ||
            //       status == PlayerControlStatus.Interrupt)
            //    {
            //        ChangeControlStatus(statusTime, status);
            //        return true;
            //    }
            //    return false;
            case PlayerControlStatus.Crouch:
                if (status == PlayerControlStatus.Normal ||
                    status == PlayerControlStatus.Interrupt ||
                    status == PlayerControlStatus.Stun ||
                    status == PlayerControlStatus.Crouch)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;

            case PlayerControlStatus.Casting:
                if (status == PlayerControlStatus.Normal ||
                    status == PlayerControlStatus.Stun ||
                    status == PlayerControlStatus.AbilityWithMovement ||
                    status == PlayerControlStatus.AbilityNeedControl )
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;

            case PlayerControlStatus.AbilityNeedControl:
            case PlayerControlStatus.AbilityWithMovement:
                if (status == PlayerControlStatus.Normal ||
                   status == PlayerControlStatus.Stun)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;

            //暂时设定打断\眩晕状态下不允许被打断，眩晕状态下眩晕允许且时间刷新
            case PlayerControlStatus.Interrupt:
            case PlayerControlStatus.Stun:
                if (status == PlayerControlStatus.Normal ||
                   status == PlayerControlStatus.Stun)
                {
                    ChangeControlStatus(statusTime, status);
                    return true;
                }
                return false;
        }

        return false;
    }

    public enum MovementMode { PlayerControl, Ability, Attacked }
    //请求在规定时间内执行给定的位移，如果当前状态允许，则记录请求，在update中实现
    //mode表示该请求来自何种源头，比如：玩家控制、技能移动、挨打后退
    //其中，技能控制会根据当前主角朝向改变方向，而挨打则不会
    public bool RequestMoveByTime(Vector2 movement, float time, MovementMode mode)
    {
        switch (mode)
        {
            //只考虑世界坐标系位移
            case MovementMode.PlayerControl:
                //return false;
                if(canControllorMovement)
                {
                    isPlayerControlMovement = true;
                    playerControlMovement = movement;
                    playerControlMovementTotalTime = time;
                    playerControlMovementSpeed = movement / time;
                    playerControlMovementCurTime = 0f;
                    return true;
                }
                return false;

            case MovementMode.Ability:
                if (canAbilityMovement)
                {
                    isAbilityMovement = true;
                    if(!isFacingLeft)
                    {
                        abilityMovement = movement;
                        abilityMovementSpeed = movement / time;
                    }
                    else
                    {
                        abilityMovement.x = - movement.x;
                        abilityMovement.y = movement.y;
                        abilityMovementSpeed = abilityMovement / time;
                    }
                    abilityMovementTotalTime = time;
                    abilityMovementCurTime = 0f;
                    return true;
                }
                return false;

            case MovementMode.Attacked:
                if (canPassiveMovement)
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

    //请求在这一帧中进行position的跳跃，movement为相对位移改变量
    //同理，技能控制会根据当前主角朝向改变方向，而被动传送则不会
    public bool RequestMoveByFrame(Vector2 movement, MovementMode mode, Space space)
    {
        switch (mode)
        {
            case MovementMode.PlayerControl:
                if (canControllorMovement)
                {
                    isControllorMovement = true;
                    controllorMovement = movement;
                    return true;
                }
                return false;

            case MovementMode.Ability:
                TpSpace = space;
                if (canActiveTransport)
                {
                    isActiveTransport = true;
                    if(space == Space.Self)
                    {
                        if (isFacingLeft)
                        {
                            activeTransportPosition.x = -movement.x;
                            activeTransportPosition.y = movement.y;
                        }
                        else
                        {
                            activeTransportPosition = movement;
                        }
                    }
                    else if(space == Space.World)
                    {
                        activeTransportPosition = movement;
                    }

                    return true;
                }
                return false;

            case MovementMode.Attacked:
                TpSpace = space;
                if (canPassiveTransport)
                {
                    isPassiveTransport = true;
                    if (space == Space.Self)
                    {
                        if (isFacingLeft)
                        {
                            passiveTransportPosition.x = -movement.x;
                            passiveTransportPosition.y = movement.y;
                        }
                        else
                        {
                            passiveTransportPosition = movement;
                        }
                    }
                    else if (space == Space.World)
                    {
                        passiveTransportPosition = movement;
                    }

                    return true;
                }
                return false;
        }

        return false;
    }

    public void SetGravity(float gravity)
    {
        this.gravity = gravity;
    }
    public void SetIsDead(bool dead)
    {
        isDead = dead;
    }
    public void SetInWater(bool isInWater)
    {
        this.isInWater = isInWater;
        gravity = isInWater ? waterGravity : gravity;
    }
    private bool isLastTouchingWall;
    //在该方法中，如果第一次触墙则y轴速度为0
    public void SetSideWall(int sideNumber)
    {
        if (isInWater || controlStatus != PlayerControlStatus.Normal)
        {
            isSideLeftWall = false;
            isSideRightWall = false;
            isLastTouchingWall = false;
            return;
        }
        switch (sideNumber)
        {
            case 0:
                isSideLeftWall = false;
                isSideRightWall = false;
                gravity = DefalutGravity;
                isLastTouchingWall = false;
                return;
            case 1:
                isSideLeftWall = true;
                isSideRightWall = false;
                gravity = climbingGravity;
                if(!isLastTouchingWall)
                {
                    ySpeed = ySpeed > 0 ? 0 : ySpeed * 0.4f;
                    isLastTouchingWall = true;
                }
                return;
            case 2:
                isSideRightWall = true;
                isSideLeftWall = false;
                gravity = climbingGravity;
                if (!isLastTouchingWall)
                {
                    ySpeed = ySpeed > 0 ? 0 : ySpeed * 0.4f;
                    isLastTouchingWall = true;
                }
                return;
        }
    }
    public void RequestJump()
    {
        isRequestJump = true;
    }
    public bool IsOnFloor()
    {
        return isOnFloor;
    }

    public float GetYSpeed()
    {
        return ySpeed;
    }
    public void SetGravity(bool isGravity)
    {
        this.isGravity = isGravity;
    }
    public float GetXSpeed()
    {
        return xSpeed;
    }


    //内部逻辑部分
    //下蹲相关
    private OnFloorDetector detector;
    //在这一帧中是否请求下蹲
    private bool isRequestCrouch = false;
    private CapsuleCollider2D playerCollider;
    //正常collider的大小和offset，以及下蹲后的collider参数
    private Vector2 colliderNormalSize;
    private Vector2 colliderNormalOffset;
    private Vector2 colliderCrouchSize;
    private Vector2 colliderCrouchOffset;

    //起跳相关
    //在这一帧中是否请求跳跃
    private bool isRequestJump = false;
    private int jumpNumberCur = 0;

    //在水中逻辑变量
    private bool isInWater = false;
    private float xDragInWater = 0.6f;
    private float waterJumpSpeed = 3f;
    private float waterDipGravity = 8f;
    private float waterGravity = 6f;
    private float waterMaxSpeed = 5f;
    private float waterRestoreForceRatio = 5f;

    //施法移动减速变量
    private float castingXSpeedRatio = 0.2f;

    //爬墙跳逻辑变量
    private bool isSideLeftWall = false;
    private bool isSideRightWall = false;
    public static readonly float climbingGravity = 5f;
    private Vector2 leftClimbingJumpVector = new Vector2(1.4f, 2f);
    private Vector2 rightClimbingJumpVector = new Vector2(-1.4f, 2f);
    private float climbingJumpTime = 0.25f;
    private float maxClimbingFallingSpeed = 4f;


    //与状态控制相关的整个类所用到的变量
    public enum PlayerControlStatus { Normal, Crouch, Casting, AbilityWithMovement, AbilityNeedControl, Interrupt, Stun}
    //当前主角处于何种状态：普通(包括行走、idle、jump、fall)，下蹲，技能释放，被控制
    private PlayerControlStatus controlStatus = PlayerControlStatus.Normal;
    //当前主角是否处于非正常状态，指主角是否处于释放技能状态或者被控制的状态，根据这个bool值判断某一帧是否需要计时器++
    private bool isInAbnormalStatus = false;
    //当前状态的计时器，Normal态和Crouch态不需要计时器
    private float controlStatusTotalTime = 0f;
    private float controlStatusCurTime = 0f;

    private bool isOnFloor = true;
    private bool isDownFloor = false;
    private bool isLeftDetect = false;
    private bool isRightDetect = false;
    private bool isDead = false;

    private bool isFacingLeft = false;
    private Vector3 leftLocalScale = new Vector3(-1, 1, 0);
    private Vector3 rightLocalScale = new Vector3(1, 1, 0);
    public bool IsFacingLeft() { return isFacingLeft; }


    //主角是否处于下坠状态，当主角在空中受到interrupt类型的攻击时，该状态位为true；当处于该状态且着地时，该状态为恢复为false
    private bool isAttackedFalling = false;

    //动画组件(改成public,用于其他类调用）
    public PlayerAnim playerAnim;
    private Rigidbody2D rigid;

    private Text debugInfo1;
    private Text debugInfo2;

    override public void Initialize()
    {
        playerAnim = GetComponent<PlayerAnim>();

        if(playerAnim == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到PlayerAnim组件！");
        }

        playerCollider = GetComponent<CapsuleCollider2D>();
        if(playerCollider == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到collider组件！");
        }
        colliderNormalSize = playerCollider.size;
        colliderNormalOffset = playerCollider.offset;

        CapsuleCollider2D crouchCollider = GameObject.Find("CrouchCollider").GetComponent<CapsuleCollider2D>();
        if (crouchCollider == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到子物体中crouchCollider组件！");
        }
        colliderCrouchOffset = crouchCollider.offset;
        colliderCrouchSize = crouchCollider.size;

        rigid = GetComponent<Rigidbody2D>();

        detector = GameObject.Find("FloorDetector").GetComponent<OnFloorDetector>();
        if(detector == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到子物体中OnFloorDetector组件！");
        }

        debugInfo1 = GameObject.Find("DebugInfo1").GetComponent<Text>();
        if (debugInfo1 == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到DebugInfo这个ui组件！");
        }
        debugInfo2 = GameObject.Find("DebugInfo2").GetComponent<Text>();
        if (debugInfo2 == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到DebugInfo这个ui组件！");
        }

        //初始化游戏菜单管理组件
        gameMenu = GameObject.Find("GameMenu").GetComponent<GameMenu>();
        if (gameMenu == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到GameMenu");
        }

        //初始化事件管理器
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        if (eventManager == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有找到EventManager");
        }
    }

    //在ui上输出movement状态信息
    StringBuilder debugInfoText = new StringBuilder(2048);
    private void SetDebugInfo()
    {
        //下面是debugInfo1的信息**************************

        debugInfoText.AppendLine("总体状态");
        debugInfoText.Append("ControlStatus: ");
        debugInfoText.AppendLine(controlStatus.ToString());
        debugInfoText.Append("isInAbnormalStatus: ");
        debugInfoText.AppendLine(isInAbnormalStatus.ToString());
        debugInfoText.Append("controlStatusTotalTime: ");
        debugInfoText.AppendLine(controlStatusTotalTime.ToString());
        debugInfoText.Append("controlStatusCurTime: ");
        debugInfoText.AppendLine(controlStatusCurTime.ToString());
        debugInfoText.Append("重力: ");
        debugInfoText.AppendLine(gravity.ToString());

        debugInfoText.AppendLine("探测器状态");
        debugInfoText.Append("isOnFloor: ");
        debugInfoText.AppendLine(isOnFloor.ToString());
        debugInfoText.Append("isDownFloor: ");
        debugInfoText.AppendLine(isDownFloor.ToString());
        debugInfoText.Append("isLeftDetect: ");
        debugInfoText.AppendLine(isLeftDetect.ToString());
        debugInfoText.Append("isRightDetect: ");
        debugInfoText.AppendLine(isRightDetect.ToString());
        debugInfoText.Append("isAttackedFalling: ");
        debugInfoText.AppendLine(isAttackedFalling.ToString());

        debugInfoText.AppendLine("跳跃状态");
        debugInfoText.Append("isRequestJump: ");
        debugInfoText.AppendLine(isRequestJump.ToString());
        debugInfoText.Append("jumpNumberCur: ");
        debugInfoText.AppendLine(jumpNumberCur.ToString());

        debugInfoText.AppendLine("下蹲状态");
        debugInfoText.Append("isRequestCrouch: ");
        debugInfoText.AppendLine(isRequestCrouch.ToString());


        debugInfoText.AppendLine("结算信息");
        debugInfoText.Append("xSpeed: ");
        debugInfoText.AppendLine(xSpeed.ToString());
        debugInfoText.Append("ySpeed: ");
        debugInfoText.AppendLine(ySpeed.ToString());
        debugInfoText.Append("xMovementPerFrame: ");
        debugInfoText.AppendLine(xMovementPerFrame.ToString());
        debugInfoText.Append("yMovementPerFrame: ");
        debugInfoText.AppendLine(yMovementPerFrame.ToString());
        debugInfoText.Append("newPosition: ");
        debugInfoText.AppendLine(newPosition.ToString());

        debugInfo1.text = debugInfoText.ToString();

        debugInfoText.Clear();

        //下面是debugInfo2的信息**************************

        debugInfoText.Append("TpSpace: ");
        debugInfoText.AppendLine(TpSpace.ToString());
        debugInfoText.AppendLine("被动传送");
        debugInfoText.Append("canPassiveTransport: ");
        debugInfoText.AppendLine(canPassiveTransport.ToString());
        debugInfoText.Append("isPassiveTransport: ");
        debugInfoText.AppendLine(isPassiveTransport.ToString());
        debugInfoText.Append("passiveTransportPosition: ");
        debugInfoText.AppendLine(passiveTransportPosition.ToString());

        debugInfoText.AppendLine("主动传送");
        debugInfoText.Append("canActiveTransport: ");
        debugInfoText.AppendLine(canActiveTransport.ToString());
        debugInfoText.Append("isActiveTransport: ");
        debugInfoText.AppendLine(isActiveTransport.ToString());
        debugInfoText.Append("activeTransportPosition: ");
        debugInfoText.AppendLine(activeTransportPosition.ToString());

        debugInfoText.AppendLine("技能位移");
        debugInfoText.Append("canAbilityMovement: ");
        debugInfoText.AppendLine(canAbilityMovement.ToString());
        debugInfoText.Append("isAbilityMovement: ");
        debugInfoText.AppendLine(isAbilityMovement.ToString());
        debugInfoText.Append("abilityMovement: ");
        debugInfoText.AppendLine(abilityMovement.ToString());
        debugInfoText.Append("abilityMovementSpeed: ");
        debugInfoText.AppendLine(abilityMovementSpeed.ToString());
        debugInfoText.Append("abilityMovementTotalTime: ");
        debugInfoText.AppendLine(abilityMovementTotalTime.ToString());
        debugInfoText.Append("abilityMovementCurTime: ");
        debugInfoText.AppendLine(abilityMovementCurTime.ToString());

        debugInfoText.AppendLine("被动位移");
        debugInfoText.Append("canPassiveMovement: ");
        debugInfoText.AppendLine(canPassiveMovement.ToString());
        debugInfoText.Append("isPassiveMovement: ");
        debugInfoText.AppendLine(isPassiveMovement.ToString());
        debugInfoText.Append("passiveMovement: ");
        debugInfoText.AppendLine(passiveMovement.ToString());
        debugInfoText.Append("passiveMovementSpeed: ");
        debugInfoText.AppendLine(passiveMovementSpeed.ToString());
        debugInfoText.Append("passiveMovementTotalTime: ");
        debugInfoText.AppendLine(passiveMovementTotalTime.ToString());
        debugInfoText.Append("passiveMovementCurTime: ");
        debugInfoText.AppendLine(passiveMovementCurTime.ToString());


        debugInfoText.AppendLine("控制移动");
        debugInfoText.Append("canControllorMovement: ");
        debugInfoText.AppendLine(canControllorMovement.ToString());
        debugInfoText.Append("isControllorMovement: ");
        debugInfoText.AppendLine(isControllorMovement.ToString());
        debugInfoText.Append("controllorMovement: ");
        debugInfoText.AppendLine(controllorMovement.ToString());


        debugInfo2.text = debugInfoText.ToString();

        debugInfoText.Clear();
    }


    private void SetAnimStatus()
    {
        playerAnim.SetXvelocity(xSpeed);
        playerAnim.SetYvelocity(ySpeed);
        playerAnim.SetIsOnGround(isOnFloor);
        //playerAnim.SetStatus(controlStatus);
    }

    //下蹲动作：改变状态、改变collider、改变探测器位置
    private void crouchBehavior()
    {
        ChangeControlStatus(0f, PlayerControlStatus.Crouch);

        ////改变大小实际上没用，因为碰撞检测交给探测器去实现了
        playerCollider.size = colliderCrouchSize;
        playerCollider.offset = colliderCrouchOffset;


        //这才是有用的
        detector.swtichToCrouchStatus();

    }
    //下蹲回复动作：改变状态、改变collider、改变探测器位置
    private void restoreFromCrouchBehavior()
    {
        ChangeControlStatus(0f, PlayerControlStatus.Normal);
        playerCollider.size = colliderNormalSize;
        playerCollider.offset = colliderNormalOffset;
      
        detector.swtichToNormalStatus();
    }
    //清除该帧统计信息
    private void Clear()
    {
        isRequestCrouch = false;
        isRequestJump = false;
        if(isOnFloor)
        {
            jumpNumberCur = 0;
        }
        else if(isInWater || isSideLeftWall || isSideRightWall)
        {
            jumpNumberCur = 0;
        }

        if(controlStatus == PlayerControlStatus.Casting)
        {
            ChangeControlStatus(0, PlayerControlStatus.Normal);
        }

        isPassiveTransport = false;
        isActiveTransport = false;
        isControllorMovement = false;

        xMovementPerFrame = 0f;
        yMovementPerFrame = 0f;
        xSpeed = 0f;

        needYFloorOffset = false;
        needXRightOffset = false;
        needXLeftOffset = false;

        if ((controlStatus != PlayerControlStatus.Normal || isPlayerControlMovement) && 
            controlStatus != PlayerControlStatus.Stun &&
            controlStatus != PlayerControlStatus.Interrupt)
        {
            ySpeed = 0;
            isGravity = false;
        }
        else
        {
            isGravity = true;
        }
    }


    //记录当前帧的运动记录参数

    //传送使用local坐标系、World坐标系混合
    private Space TpSpace;
    //防止陷入地板的y轴offset
    private float yFloorOffset = 0f;
    private float xFloorOffset = 0f;
    private bool needYFloorOffset = false;
    private bool needXRightOffset = false;
    private bool needXLeftOffset = false;
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

    //控制位移，以时间结算
    private Vector2 playerControlMovement;
    private Vector2 playerControlMovementSpeed = new Vector2(0, 0);
    private float playerControlMovementTotalTime = 0f;
    private float playerControlMovementCurTime = 0f;
    private bool isPlayerControlMovement = false;

    //变速状态管理(加速、减速的计时结构)
    private const int SPEED_RATIO_LIST_MAX_SIZE = 32;

    //减速列表
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

    //修改当前状态的私有方法；在修改当前状态的同时，把Can系列的状态为设置，保证在某些状态下屏蔽掉低优先级的请求
    //(比如在眩晕的时候请求释放技能)；同时，将低优先级的计时器状态设置为false(is系列状态位)，保证进行高优先级计时器是
    //低优先级计时器不在工作(比如0.5s眩晕状态会终止1s的技能释放状态，否则0.5s眩晕结束后，技能没有被打断，仍然处在技能移动状态下)
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
        //只有状态改变时才告诉动画机转换状态
        playerAnim.SetStatus(controlStatus);
        
        switch (status)
        {
            case PlayerControlStatus.Normal:
                canPassiveTransport = true;
                canActiveTransport = true;
                canAbilityMovement = true;
                canPassiveMovement = true;
                canControllorMovement = true;

                isAbilityMovement = false;
                isPassiveMovement = false;

                isInAbnormalStatus = false;
                break;
            case PlayerControlStatus.Crouch:
                canPassiveMovement = true;
                canActiveTransport = false;
                canAbilityMovement = false;
                canPassiveMovement = true;
                canControllorMovement = true;

                isAbilityMovement = false;
                isPassiveMovement = false;

                isInAbnormalStatus = false;

                break;

            case PlayerControlStatus.Casting:
                canPassiveMovement = true;
                canActiveTransport = true;
                canAbilityMovement = true;
                canPassiveMovement = true;
                canControllorMovement = true;

                isPassiveMovement = false;
                isControllorMovement = false;

                isInAbnormalStatus = true;
                break;
            case PlayerControlStatus.AbilityWithMovement:
                canPassiveMovement = true;
                canActiveTransport = true;
                canAbilityMovement = true;
                canPassiveMovement = true;
                canControllorMovement = false;

                isPassiveMovement = false;
                isControllorMovement = false;

                isInAbnormalStatus = true;
                break;
            case PlayerControlStatus.AbilityNeedControl:
                canPassiveMovement = true;
                canActiveTransport = true;
                canAbilityMovement = true;
                canPassiveMovement = true;
                canControllorMovement = true;

                isPassiveMovement = false;

                isInAbnormalStatus = true;
                break;
            case PlayerControlStatus.Interrupt:
            case PlayerControlStatus.Stun:
                canPassiveMovement = true;
                canActiveTransport = false;
                canAbilityMovement = false;
                canPassiveMovement = true;
                canControllorMovement = false;

                isAbilityMovement = false;
                isPassiveMovement = false;
                isControllorMovement = false;

                isInAbnormalStatus = true;
                break;
        }
    }


    //子物体需要对该物体进行的实时更新函数
    //比如检测器告诉player是否在地面上，头顶是否有东西等
    public void SetOnFloor(GameObject sender, bool isOnFloor)
    {
        if (sender.transform.IsChildOf(transform))
        {
            this.isOnFloor = isOnFloor;
        }
        else
        {
            Debug.LogError("在" + gameObject.name + "中，非探测器物体调用了setOnFloor函数！");
        }
    }
    public void SetDownFloor(GameObject sender, bool isDownFloor)
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
    public void SetLeftDetect(GameObject sender, bool flag)
    {
        if (sender.transform.IsChildOf(transform))
        {
            this.isLeftDetect = flag;
        }
        else
        {
            Debug.LogError("在" + gameObject.name + "中，非探测器物体调用了setDownFloor函数！");
        }
    }
    public void SetRightDetect(GameObject sender, bool flag)
    {
        if (sender.transform.IsChildOf(transform))
        {
            this.isRightDetect = flag;
        }
        else
        {
            Debug.LogError("在" + gameObject.name + "中，非探测器物体调用了setDownFloor函数！");
        }
    }
    public void SetFloorOffset(float offset)
    {
        this.yFloorOffset = offset;
        needYFloorOffset = true;
    }
    public void SetRightOffset(float offset)
    {
        xFloorOffset = offset;
        needXRightOffset = true;
    }
    public void SetLeftOffset(float offset)
    {
        xFloorOffset = offset;
        needXLeftOffset = true;
    }

    //MyUpdate相关的属性及方法

    //update函数，处理异常状态计时与恢复、下蹲逻辑、起跳逻辑、根据这一帧的运动记录结算运动情况、结算减速
    override public void MyUpdate()
{ 

        //Debug.Log("1Player当前的重力是" + gravity.ToString());
        if (isDead)
        {
            gameMenu.GameOver();
            return;
        }

        //如果当前状态不是Normal或Crouch，则时间++，
        if (isInAbnormalStatus && controlStatus != PlayerControlStatus.Casting)
        {
            ////如果在空中处于挨打，则处于受击下落状态，同时改变Interrupt时间为正无穷，直到落地才退出该状态
            //if (controlStatus == PlayerControlStatus.Interrupt && !isOnFloor)
            //{
            //    isAttackedFalling = true;
            //    controlStatusTotalTime = float.MaxValue;
            //}
            ////如果处于受击下落状态，且在这一帧着地，则进入普通状态(后面可以考虑改动为进入落地动画)
            //if (isAttackedFalling && isOnFloor)
            //{
            //    isAttackedFalling = false;
            //    ChangeControlStatus(0f, PlayerControlStatus.Normal);
            //}

            //状态时间++
            controlStatusCurTime += Time.deltaTime;
            //状态到期发生的事情
            if (controlStatusCurTime >= controlStatusTotalTime)
            {
                ChangeControlStatus(0f, PlayerControlStatus.Normal);
                
                //cameraMachine.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //Debug.Log("执行还原:"+cameraMachine.transform.rotation);
                //playerAnim.SetAbilityNum(999);
                //playerAnim.SetUseSkillType((int)AttackPlayer.SkillType.Null);
            }
        }
        //在处理正常状态
        else
        {
            if (isInWater)
            {
                //清空时恢复gravity
                if (isRequestCrouch)
                {
                    gravity = waterDipGravity;
                }
                else
                {
                    gravity = waterGravity;
                }
            }
            //如果在地面上
            else if (isOnFloor)
            {
                //处理下蹲逻辑
                //如果在地面上按下了下蹲键，或者，没有按下下蹲，但是蹲的时候头上有东西，则保持下蹲
                if (isRequestCrouch || !isRequestCrouch && controlStatus == PlayerControlStatus.Crouch && isDownFloor)
                {
                    crouchBehavior();
                }
                //否则恢复Normal
                else
                {
                    if (controlStatus == PlayerControlStatus.Crouch)
                    {
                        restoreFromCrouchBehavior();
                    }
                }
            }
        }

        //处理起跳逻辑
        //在某些技能状态下，允许玩家控制运动状态，我们也应该允许跳跃
        //跳跃计数在clear函数中恢复
        if (controlStatus == PlayerControlStatus.Normal || controlStatus == PlayerControlStatus.AbilityNeedControl)
        {
            if (isRequestJump && jumpNumberCur < jumpNumberMax)
            {
                if(isInWater)
                {
                    ySpeed = waterJumpSpeed + ySpeed * jumpOriRatio;
                }
                else if((isSideLeftWall || isSideRightWall) && controlStatus == PlayerControlStatus.Normal)
                {
                    Debug.Log("请求抓墙跳");
                    if(isSideLeftWall)
                    {
                        jumpNumberCur++;
                        RequestMoveByTime(leftClimbingJumpVector, climbingJumpTime, MovementMode.PlayerControl);
                    }
                    else if(isSideRightWall)
                    {
                        jumpNumberCur++;
                        RequestMoveByTime(rightClimbingJumpVector, climbingJumpTime, MovementMode.PlayerControl);
                    }
                }
                else
                {
                    ySpeed = jumpForce + ySpeed * jumpOriRatio;
                    jumpNumberCur++;

                    StatisticsCollector.jumpNumber++;
                    //触发事件
                    if (isOnFloor)
                    {
                        jumpArray[0] = 1;
                    }
                    else
                    {
                        jumpArray[0] = 2;
                    }
                    eventManager.TriggerEvent(EventManager.OnJumpNumChange, jumpArray);
                }
            }
        }

        //Debug.Log("2Player当前的重力是" + gravity.ToString());
        //Debug.Log("2Player当前的状态是" + controlStatus.ToString());

        //结算运动情况
        if (isPassiveTransport)
        {
            if(TpSpace == Space.Self)
            {
                xMovementPerFrame = passiveTransportPosition.x;
                yMovementPerFrame = passiveTransportPosition.y;
            }
            else if(TpSpace == Space.World)
            {
                Vector2 targetPosition = transform.InverseTransformPoint(passiveTransportPosition);
                xMovementPerFrame = isFacingLeft ? - targetPosition.x : targetPosition.x;
                yMovementPerFrame = targetPosition.y;
            }
        }
        else if (isActiveTransport)
        {
            if (TpSpace == Space.Self)
            {
                xMovementPerFrame = activeTransportPosition.x;
                yMovementPerFrame = activeTransportPosition.y;
            }
            else if (TpSpace == Space.World)
            {
                Vector2 targetPosition = transform.InverseTransformPoint(activeTransportPosition);
                xMovementPerFrame = isFacingLeft ? - targetPosition.x : targetPosition.x;
                yMovementPerFrame = targetPosition.y;
            }
        }
        else if (isAbilityMovement)
        {
            xSpeed += abilityMovementSpeed.x;
            ySpeed += abilityMovementSpeed.y;

            abilityMovementCurTime += Time.deltaTime;
            if (abilityMovementCurTime >= abilityMovementTotalTime)
            {
                isAbilityMovement = false;
                abilityMovementCurTime = 0f;
                abilityMovementTotalTime = 0f;
                isGravity = true;
            }
        }
        else if (isPassiveMovement)
        {
            xSpeed += passiveMovementSpeed.x;
            ySpeed += passiveMovementSpeed.y;

            passiveMovementCurTime += Time.deltaTime;
            if (passiveMovementCurTime >= passiveMovementTotalTime)
            {
                isPassiveMovement = false;
                passiveMovementCurTime = 0f;
                passiveMovementTotalTime = 0f;
                isGravity = true;
            }
        }
        else if (isControllorMovement)
        {
            //放大x轴向速度
            xSpeed += controllorMovement.x * xSpeedRatio;
            ySpeed += controllorMovement.y;

            if(isPlayerControlMovement)
            {
                xSpeed += playerControlMovementSpeed.x;
                ySpeed += playerControlMovementSpeed.y;
                playerControlMovementCurTime += Time.deltaTime;

                if(playerControlMovementCurTime >= playerControlMovementTotalTime)
                {
                    playerControlMovementCurTime = 0f;
                    isPlayerControlMovement = false;
                }
            }

            if (controlStatus == PlayerControlStatus.Casting)
            {
                xSpeed *= castingXSpeedRatio;
            }
        }

        //Debug.Log("3Player当前的重力是" + gravity.ToString());
        //结算减速列表

        if (controlStatus == PlayerControlStatus.Crouch)
        {
            xSpeed /= crouchDivision;
        } 
        else if(isInWater)
        {
            xSpeed *= xDragInWater;
        }


        //结算重力
        if (isGravity)
        {
            ySpeed -= gravity * Time.deltaTime;
            if(isInWater)
            {
                if (ySpeed > waterMaxSpeed) ySpeed -= ySpeed * waterRestoreForceRatio * Time.deltaTime;
                else if (ySpeed < -waterMaxSpeed) ySpeed -= ySpeed * waterRestoreForceRatio * Time.deltaTime;
            }
            else if(isSideLeftWall || isSideRightWall)
            {
                ySpeed = ySpeed < -maxClimbingFallingSpeed ? -maxClimbingFallingSpeed : ySpeed;
            }
            else
            {
                ySpeed = ySpeed < yFallingMaxSpeed ? yFallingMaxSpeed : ySpeed;
            }
        }

        //检测墙体碰撞，以便速度归0
        if ((isOnFloor && ySpeed < 0) || (isDownFloor && ySpeed > 0))
        {
            ySpeed = 0f;
        }
        if ((isLeftDetect && xSpeed < 0) || (isRightDetect && xSpeed > 0))
        {
            xSpeed = 0f;
        }

        //如果没有tp则计算速度
        if(!isActiveTransport && !isPassiveTransport)
        {
            xMovementPerFrame += xSpeed * Time.deltaTime;
            yMovementPerFrame += ySpeed * Time.deltaTime;
        }

        //转身: 只有在Normal or Crouch状态下才回转身，其他状态不会转身
        if(!isInAbnormalStatus || controlStatus == PlayerControlStatus.AbilityNeedControl
            || controlStatus == PlayerControlStatus.Casting)
        {
            if (xMovementPerFrame < 0)
            {
                isFacingLeft = true;
                transform.localScale = leftLocalScale;
            }
            else if (xMovementPerFrame > 0)
            {
                isFacingLeft = false;
                transform.localScale = rightLocalScale;
            }
        }

        ////控制y轴速度绝对值在最大值以内
        //ySpeed = ySpeed > yMaxSpeed ? yMaxSpeed : ySpeed;
        //ySpeed = ySpeed < -yMaxSpeed ? -yMaxSpeed : ySpeed;
        xMovementPerFrame += rigid.velocity.x * Time.deltaTime;
        yMovementPerFrame += rigid.velocity.y * Time.deltaTime;

        transform.Translate(xMovementPerFrame, yMovementPerFrame, 0, Space.Self);

        //如果探测器感知这一帧y轴速度为负，且探测到了地板，则进行y轴位置调整
        if(needYFloorOffset && controlStatus == PlayerControlStatus.Normal)
        {
            transform.Translate(0, yFloorOffset, 0, Space.Self);
        }
        if(needXRightOffset && xSpeed > 0)
        {
            transform.Translate(xFloorOffset, 0, 0, Space.Self);
        }
        if(needXLeftOffset && xSpeed < 0)
        {
            transform.Translate(xFloorOffset, 0, 0, Space.Self);
        }

        //Debug.Log("4Player当前的重力是" + gravity.ToString());
        SetAnimStatus();

        //Debug.Log("3Player当前的状态是" + controlStatus.ToString());
        SetDebugInfo();
        Clear();
    }
    //所在update队列为Player
    private UpdateType updateType = UpdateType.Player;
    //player中优先级等级为9
    private int priorityInType = 9;
    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
    public override int GetPriorityInType()
    {
        return priorityInType;
    }
    public RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;//人物位置

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        Debug.DrawRay(pos + offset, rayDiraction * length, color);//显示射线

        return hit;
    }

}
