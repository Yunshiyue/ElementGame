/**
 * @Description: 探测器组件，player的子物体，在update中探测主角上下左右是否有碰撞体，并通知主角的movement组件。
 *               通过raycast实现，调用顺序应该在movement前。
 * @Author: ridger

 * 

 * @Editor: ridger
 * @Edit: 1. 修改了下方探测器的位置设定逻辑，现在下方探测器长度由两部分组成，一部分是player内部探测器长度，用于
 *           固定探测器的初始位置，以及计算当触碰到地板的时候y轴距离补偿。另一部分是是player外部探测器长度，用于检测
 *           isOnfloor。
 *        2. 当这一帧主角y轴速度为负值(下落)且下方探测器探测到地板时，进行y轴距离补偿，调整y轴位置以使得主角
 *           不再陷入地板
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnFloorDetector : myUpdate
{
    //updateManager.player调用顺序
    //OnFloorDetector: 2
    private int priorityInType = 2;

    private MovementPlayer playerMovementComponent;
    //保存player中的collider组件
    private CapsuleCollider2D coll;

    //水探测器长度
    private float waterDetectorLength = 0.1f;
    //头顶探测器长度
    private float headDetectorLength = 0.3f;
    //脚下探测器长度参数
    private static float floorOutOfBodyDistance = 0.02f;
    private static float floorDetectorAsecendDistance = 0.25f;
    private static float floorDetectorLength = floorOutOfBodyDistance + floorDetectorAsecendDistance;
    //其他探测器长度
    private static float LRDetectorInsideLength = 0.02f;
    private static float LRDetectorOutsideLength = 0.05f;
    private static float LRDetectorTotalLength = LRDetectorInsideLength + LRDetectorOutsideLength;
    //脚下探测器x轴向缩放比例
    private float detectorInsideOffsetRatio = 0.6f;
    //获得地面层
    private LayerMask groundLayer;
    private LayerMask waterLayer;

    private bool isTouchingLeftGround;
    private bool isTouchingRightGround;

    private GameObject player;

    //Input System
    private Keyboard keyboard;

    //updateManager中代替start函数的初始化方法
    override public void Initialize()
    {
        player = GameObject.Find("Player");
        playerMovementComponent = player.GetComponent<MovementPlayer>();
        if(playerMovementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "的父亲中，找不到MovementPlayer组件！");
        }

        coll = GameObject.Find("Player").GetComponent<CapsuleCollider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "的父亲中，找不到coll2D组件！");
        }

        groundLayer = LayerMask.GetMask("Platform");
        waterLayer = LayerMask.GetMask("Water");

        keyboard = InputSystem.GetDevice<Keyboard>();
        if (keyboard == null)
        {
            Debug.LogError("在" + gameObject.name + "中，初始化keyboard失败");
        }

        //给探测器固定位置
        ldnx = -coll.size.x / 2 + LRDetectorInsideLength;
        ldny = -coll.size.y / 2 * detectorInsideOffsetRatio;
        ldcx = ldnx;
        ldcy = ldny;
        ltnx = -coll.size.x / 2 + LRDetectorInsideLength;
        ltny = coll.size.y / 2;
        ltcx = ltnx;
        ltcy = ltny - descentDistance;
        rdnx = coll.size.x / 2 - LRDetectorInsideLength;
        rdny = -coll.size.y / 2 * detectorInsideOffsetRatio;
        rdcx = rdnx;
        rdcy = rdny;
        rtnx = coll.size.x / 2 - LRDetectorInsideLength;
        rtny = coll.size.y / 2;
        rtcx = rtnx;
        rtcy = rtny - descentDistance;
        flnx = -coll.size.x / 2 * detectorInsideOffsetRatio;
        flny = -coll.size.y / 2 + floorDetectorAsecendDistance;
        flcx = flnx;
        flcy = flny;
        frnx = coll.size.x / 2 * detectorInsideOffsetRatio;
        frny = -coll.size.y / 2 + floorDetectorAsecendDistance;
        frcx = frnx;
        frcy = frny;
        hlnx = -coll.size.x / 2 * detectorInsideOffsetRatio;
        hlny = coll.size.y / 2 - 0.029f;
        hlcx = hlnx;
        hlcy = hlny - descentDistance;
        hrnx = coll.size.x / 2 * detectorInsideOffsetRatio;
        hrny = coll.size.y / 2 - 0.029f;
        hrcx = hrnx;
        hrcy = hrny - descentDistance;

        swtichToNormalStatus();
    }
    //当下蹲状态结束后，切换探测器位置到一般位置
    public void swtichToNormalStatus()
    {
        leftDownRay.x = ldnx;
        leftDownRay.y = ldny;
        leftTopRay.x = ltnx;
        leftTopRay.y = ltny;

        rightDownRay.x = rdnx;
        rightDownRay.y = rdny;
        rightTopRay.x = rtnx;
        rightTopRay.y = rtny;

        floorLeftRay.x = flnx;
        floorLeftRay.y = flny;
        floorRightRay.x = frnx;
        floorRightRay.y = frny;

        headLeftRay.x = hlnx;
        headLeftRay.y = hlny;
        headRightRay.x = hrnx;
        headRightRay.y = hrny;
    }
    //当下蹲状态开始时，切换探测器位置到下蹲位置
    public void swtichToCrouchStatus()
    {

        leftDownRay.x = ldcx;
        leftDownRay.y = ldcy;
        leftTopRay.x = ltcx;
        leftTopRay.y = ltcy;

        rightDownRay.x = rdcx;
        rightDownRay.y = rdcy;
        rightTopRay.x = rtcx;
        rightTopRay.y = rtcy;


        floorLeftRay.x = flcx;
        floorLeftRay.y = flcy;
        floorRightRay.x = frcx;
        floorRightRay.y = frcy;


        headLeftRay.x = hlcx;
        headLeftRay.y = hlcy;
        headRightRay.x = hrcx;
        headRightRay.y = hrcy;
    }

    public bool isLeftTouchingGround()
    {
        return isTouchingLeftGround;
    }
    public bool isRightTouchingGround()
    {
        return isTouchingRightGround;
    }
    //通过封装的raycast方法，检测射线是否碰撞到地面。并通过set方法通知movementplayer。
    override public void MyUpdate()
    {
        //下方探测器
        RaycastHit2D floorLeftCheck = Raycast(floorLeftRay, Vector2.down, floorDetectorLength, groundLayer);
        RaycastHit2D floorRightCheck = Raycast(floorRightRay, Vector2.down, floorDetectorLength, groundLayer);
        if (floorLeftCheck || floorRightCheck)
        {
            playerMovementComponent.SetOnFloor(gameObject, true);

            //设置父物体
            if (floorLeftCheck && floorLeftCheck.collider.gameObject.tag == "MoveablePlatform")
            {
                player.transform.SetParent(floorLeftCheck.collider.gameObject.transform);
            }
            else if(floorRightCheck && floorRightCheck.collider.gameObject.tag == "MoveablePlatform")
            {
                player.transform.SetParent(floorRightCheck.collider.gameObject.transform);
            }
            else if (player.gameObject.transform.parent != null)
            {
                player.gameObject.transform.parent = null;
            }

            //如果碰到地板而且在下落状态
            //if (playerMovementComponent.GetYSpeed() < 0)
            //{
                //取hit中最大的那个(如果只有一个探针碰到则另一个distance = 0)进行移动
                float maxDistance = floorLeftCheck.distance > floorRightCheck.distance ? floorLeftCheck.distance : floorRightCheck.distance;
                playerMovementComponent.SetFloorOffset(floorDetectorAsecendDistance - maxDistance);
            //}
        }
        else
        {
            playerMovementComponent.SetOnFloor(gameObject, false);
        }

        //左方探测器
        RaycastHit2D leftTopCheck = Raycast(leftTopRay, Vector2.left, LRDetectorTotalLength, groundLayer);
        RaycastHit2D leftDownCheck = Raycast(leftDownRay, Vector2.left, LRDetectorTotalLength, groundLayer);
        if (leftDownCheck || leftTopCheck)
        {
            isTouchingLeftGround = true;
            playerMovementComponent.SetLeftDetect(gameObject, true);
            if(playerMovementComponent.GetXSpeed() < 0)
            {
                Debug.Log("左侧x轴距离补偿");
                //取hit中最大的那个(如果只有一个探针碰到则另一个distance = 0)进行移动
                float maxDistance = leftTopCheck.distance > leftDownCheck.distance ? leftTopCheck.distance : leftDownCheck.distance;
                playerMovementComponent.SetRightOffset(LRDetectorInsideLength - maxDistance);
            }
        }
        else
        {
            playerMovementComponent.SetLeftDetect(gameObject, false);
            isTouchingLeftGround = false;
        }

        //右方探测器
        RaycastHit2D rightTopCheck = Raycast(rightTopRay, Vector2.right, LRDetectorTotalLength, groundLayer);
        RaycastHit2D rightDownCheck = Raycast(rightDownRay, Vector2.right, LRDetectorTotalLength, groundLayer);
        if (rightDownCheck || rightTopCheck)
        {
            isTouchingRightGround = true;
            playerMovementComponent.SetRightDetect(gameObject, true);
            //if (playerMovementComponent.GetXSpeed() > 0)
            //{
                //Debug.Log("右侧侧x轴距离补偿");
                //取hit中最大的那个(如果只有一个探针碰到则另一个distance = 0)进行移动
                float maxDistance = rightTopCheck.distance > rightDownCheck.distance ? rightTopCheck.distance : rightDownCheck.distance;
                playerMovementComponent.SetRightOffset(maxDistance - LRDetectorInsideLength);
            //}
        }
        else
        {
            isTouchingRightGround = false;
            playerMovementComponent.SetRightDetect(gameObject, false);
        }

        //旧输入系统
        if (rightTopCheck && Input.GetAxis("Horizontal") > 0f)
        {
            playerMovementComponent.SetSideWall(2);
        }
        else if (leftTopCheck && Input.GetAxis("Horizontal") < 0f)
        {
            playerMovementComponent.SetSideWall(1);
        }
        //if (rightTopCheck && keyboard.dKey.isPressed)
        //{
        //    playerMovementComponent.SetSideWall(2);
        //}
        //else if (leftTopCheck && keyboard.aKey.isPressed)
        //{
        //    playerMovementComponent.SetSideWall(1);
        //}
        else
        {
            playerMovementComponent.SetSideWall(0);
        }
        //上方探测器
        RaycastHit2D headLeftCheck = Raycast(headLeftRay, Vector2.up, headDetectorLength, groundLayer);
        RaycastHit2D headRightCheck = Raycast(headRightRay, Vector2.up, headDetectorLength, groundLayer);
        if (headLeftCheck || headRightCheck)
        {
            playerMovementComponent.SetDownFloor(gameObject, true);
        }
        else
        {
            playerMovementComponent.SetDownFloor(gameObject, false);
        }

        RaycastHit2D halfInWater = Raycast(halfInWaterRay, Vector2.down, waterDetectorLength, waterLayer);
        if(halfInWater)
        {
            playerMovementComponent.SetInWater(true);
            //Debug.Log("进入水中");
        }
        else
        {
            playerMovementComponent.SetInWater(false);
        }
    }

    /// <summary>
    /// 射线检测方法，对原本的Physics2D.Raycast(Vector2 origin, Vector2 direction, float length, int layerMask)方法进行封装
    /// 射线的起始点origin由当前人物位置pos + 偏移量 offset确定
    /// </summary>
    /// <param name="offset">偏移量</param>
    /// <param name="rayDiraction">射线方向</param>
    /// <param name="length">射线长度</param>
    /// <param name="layer">投射射线，选择投射的层蒙版</param>
    /// <returns>返回bool，判断射线是否触碰到layer</returns>
    public RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;//人物位置

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        Debug.DrawRay(pos + offset, rayDiraction * length, color);//显示射线

        return hit;
    }

    override public int GetPriorityInType()
    {
        return priorityInType;
    }
    public UpdateType updateType = UpdateType.Player;
    public override UpdateType GetUpdateType()
    {
        return updateType;
    }

    private Vector2 floorLeftRay = new Vector2(0, 0);
    private Vector2 floorRightRay = new Vector2(0, 0);
    private Vector2 leftTopRay = new Vector2(0, 0);
    private Vector2 leftDownRay = new Vector2(0, 0);
    private Vector2 rightTopRay = new Vector2(0, 0);
    private Vector2 rightDownRay = new Vector2(0, 0);
    private Vector2 headRightRay = new Vector2(0, 0);
    private Vector2 headLeftRay = new Vector2(0, 0);
    private Vector2 halfInWaterRay = new Vector2(0, 0);
    private float descentDistance = 0.2f;
    private float ldnx;
    private float ldny;
    private float ldcx;
    private float ldcy;
    private float ltnx;
    private float ltny;
    private float ltcx;
    private float ltcy;
    private float rdnx;
    private float rdny;
    private float rdcx;
    private float rdcy;
    private float rtnx;
    private float rtny;
    private float rtcx;
    private float rtcy;
    private float flnx;
    private float flny;
    private float flcx;
    private float flcy;
    private float frnx;
    private float frny;
    private float frcx;
    private float frcy;
    private float hlnx;
    private float hlny;
    private float hlcx;
    private float hlcy;
    private float hrnx;
    private float hrny;
    private float hrcx;
    private float hrcy;
}
