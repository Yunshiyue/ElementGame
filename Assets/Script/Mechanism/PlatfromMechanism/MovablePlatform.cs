/**
 * @Description: MovablePlatform是可移动平台类，可以任意方向循环移动
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour, Movable
{
    public float speed = 1.0f;
    public float oneWayTime = 3.0f;
    private float passTime = 0.0f;

    [Header("移动参数")]
    public Vector3 direction = new Vector3(1, 0, 0);

    private void Update()
    {
        //移动
        Movement();

        //更新时间
        UpdateTime();
    }

    public void Movement()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// 更新计时器
    /// </summary>
    private void UpdateTime()
    {
        passTime += Time.deltaTime;
        if (passTime > oneWayTime)
        {
            passTime = 0.0f;
            direction.x = -1 * direction.x;
            direction.y = -1 * direction.y;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    collision.gameObject.transform.SetParent(transform);
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    collision.gameObject.transform.SetParent(null);
    //}
}
