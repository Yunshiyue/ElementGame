/**
 * @Description: Treasure为宝箱类，为可触发的机关的一种，是机关类Machanism的子类
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Machanism
{
    bool isOpen = false;
    bool isPlayerEnter = false;

    void Update()
    {
        if (!isOpen && CanOpen())
        {
            Trigger();
        }
    }

    /// <summary>
    /// 打开宝箱的条件
    /// </summary>
    /// TODO 该如何判断是否可以打开宝箱？是否有前置条件？
    private bool CanOpen()
    {
        return isPlayerEnter;
    }

    /// <summary>
    /// 触发机关（开启宝箱）
    /// </summary>
    /// TODO 开启宝箱该做什么？
    public override void Trigger()
    {
        isOpen = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerEnter = false;
        }
    }
}
