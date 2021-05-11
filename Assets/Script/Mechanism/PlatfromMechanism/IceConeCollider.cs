/**
 * @Description: IceConeCollider为冰锥碰撞体类，当玩家进入检测范围，会触发冰锥掉落
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceConeCollider : MonoBehaviour
{
    private Mechanism parentMechanism;

    void Start()
    {
        parentMechanism = transform.GetComponentInParent<Mechanism>();
        if (parentMechanism == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到parentMechanism");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("人物进入范围");
            //玩家触发机关
            parentMechanism.Trigger(Mechanism.TiggerType.Other);
        }
    }
}
