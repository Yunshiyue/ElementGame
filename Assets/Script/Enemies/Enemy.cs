using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @Description: 敌人父类 
 * @Author:  夜里猛

 
*/
public class Enemy : LivingBody
{
    /// <summary>
    /// player的Layer
    /// </summary>
    public LayerMask playerLayer;

    public override void Initialize()
    {
        isDead = false;

    }

    public override void Dead()
    {
        throw new System.NotImplementedException();
    }

    public override void Movement()
    {
        throw new System.NotImplementedException();
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
    protected RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green;

        Debug.DrawRay(pos + offset, rayDiraction * length, color);

        return hit;

    }
}
