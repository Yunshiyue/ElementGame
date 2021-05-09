/**
 * @Description: Machanism类为所有可以触发的机关的父类，包含触发机关的方法
 * @Author: CuteRed
1-2-1 20:30
 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Machanism : MonoBehaviour
{

    public Collider2D coll;

    // Start is called before the first frame update
    void Awake()
    {
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到collider");
        }
    }

    /// <summary>
    /// 触发机关
    /// </summary>
    public abstract void Trigger();
    
}
