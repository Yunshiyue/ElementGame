using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFloorDetector : myUpdate
{
    public int priorityInType = 2;

    private MovementPlayer playerMovementComponent;
    private CapsuleCollider2D coll;

    //碰撞体尺寸
    private Vector2 collStandSize;
    private Vector2 collStandOffset;//坐标
    private Vector2 collCrouchSize;
    private Vector2 collCrouchOffset;

    [Header("环境检测")]
    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundStance = 0.1f;

    private LayerMask groundLayer;


    private void Awake()
    {
        GameObject.Find("UpdateManager").GetComponent<UpdateManager>().Register(this);
    }

    public UpdateType updateType = UpdateType.Player;
    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
    public override int GetPriorityInType()
    {
        return priorityInType;
    }
    override public void Initialize()
    {

        playerMovementComponent = transform.parent.GetComponent<MovementPlayer>();
        if(playerMovementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "的父亲中，找不到MovementPlayer组件！");
        }

        coll = transform.parent.GetComponent<CapsuleCollider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "的父亲中，找不到coll2D组件！");
        }

        collStandSize = coll.size;//人物站立时大小
        collStandOffset = coll.offset;//人物站立时坐标
        collCrouchSize = new Vector2(coll.size.x, coll.size.y / 2);//人物蹲下时大小
        collCrouchOffset = new Vector2(coll.offset.x, -0.2418612f);//人物蹲下时坐标

        groundLayer = LayerMask.GetMask("Platform");
    }


    /// <summary>
    /// 物理检测，检测人物是否在地面;
    /// leftCheck：左脚  ；rightCheck：右脚;
    /// headCheck：头部检测
    /// </summary>
    override public void MyUpdate()
    {
        RaycastHit2D leftCheck = Raycast(new Vector2(-coll.size.x / 2, -collStandSize.y / 2 + 0.029f), Vector2.down, groundStance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(coll.size.x / 2, -collStandSize.y / 2 + 0.029f), Vector2.down, groundStance, groundLayer);

        if (leftCheck || rightCheck)
        {
            playerMovementComponent.setOnFloor(gameObject, true);
        }
        else
        {
            playerMovementComponent.setOnFloor(gameObject, false);
        }

        RaycastHit2D headCheck = Raycast(new Vector2(0, coll.size.y - collStandSize.y / 2 + 0.029f), Vector2.up, headClearance, groundLayer);

        playerMovementComponent.setDownFloor(gameObject, (bool)headCheck);
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
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;//人物位置

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        Debug.DrawRay(pos + offset, rayDiraction * length, color);//显示射线

        return hit;
    }

    public int GetPriority()
    {
        return 1;
    }
}
