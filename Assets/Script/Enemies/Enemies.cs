using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  Enemies : myUpdate
{
    protected LayerMask playerLayer;

    protected DefenceEnemies defenceComponent;//防御组件 包含hpmax hp isdead 且自带canbeFight
                                              

    protected bool isSeePlayer = false;

    
    //update顺序
    protected UpdateType updateType = UpdateType.Enemy;
    //子类修改优先级
    protected int priorityInType = 0;

    /// <summary>
    /// 射线检测方法，对原本的Physics2D.Raycast(Vector2 origin, Vector2 direction, float length, int layerMask)方法进行封装
    /// 射线的起始点origin由当前人物位置pos + 偏移量 offset确定
    /// </summary>
    /// <param name="offset">偏移量</param>
    /// <param name="rayDiraction">射线方向</param>
    /// <param name="length">射线长度</param>
    /// <param name="layer">投射射线，选择投射的层蒙版</param>
    /// <returns>返回bool，判断射线是否触碰到layer</returns>
    protected RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;//人物位置

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green; //射线颜色 触碰layerr变red;不触碰则为green

        Debug.DrawRay(pos + offset, rayDiraction * length, color);//显示射线

        return hit;
    }
    public override void Initialize()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }

    
}
