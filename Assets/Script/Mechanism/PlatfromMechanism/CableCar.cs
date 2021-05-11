/**
 * @Description: CableCar是缆车类，用于检测风短技能是否吹到了缆车
 * @Author: CuteRed

 *      
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableCar : MonoBehaviour
{
    private Rigidbody2D rbody;
    private GameObject player;
    private Vector3 direction = Vector3.zero;
    private CableCarCollider cableCarCollider;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        if (rbody == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取rigidbody失败");
        }

        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取玩家失败");
        }

        cableCarCollider = GetComponent<CableCarCollider>();
        if (cableCarCollider == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取CAbleCarCollider失败");
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WindShortMainCollider")
        {
            direction.x = player.transform.localScale.x * (-1);
            cableCarCollider.Init(direction);
        }
    }
}
