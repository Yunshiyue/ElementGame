/**
 * @Description: ElasticPlatform是弹性地形类，有刚体的物体在上面会被弹起
 * @Author: CuteRed

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticPlatform : MonoBehaviour
{
    /// <summary>
    /// 玩家移动组件
    /// </summary>
    MovementPlayer movementPlayer;

    [Header("位移参数")]
    public Vector2 force = new Vector2(0, 1);

    private void Start()
    {
        movementPlayer = GameObject.Find("Player").GetComponent<MovementPlayer>();
        if (movementPlayer == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rigidbody;

        if (collision.gameObject.TryGetComponent<Rigidbody2D>(out rigidbody))
        {
            //如果是玩家，则调用玩家的移动组件
            if (collision.gameObject.name == "Player")
            {
                movementPlayer.RequestMoveByTime(force, 1.5f, MovementPlayer.MovementMode.PlayerControl);
            }
            //如果是其他物体，则调用刚体的操作
            else
            {
                rigidbody.velocity = force * 2;
                
            }
            Debug.Log(collision.gameObject.name + "被弹起");
        }
    }
}
