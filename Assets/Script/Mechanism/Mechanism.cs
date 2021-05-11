/**
 * @Description: Mechanism类为所有可以触发的机关的父类，包含触发机关的方法
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mechanism : MonoBehaviour
{
    public enum TiggerType { Fire, Ice, Wind, Thunder, Switch, Other};

    private Collider2D coll;
    protected bool isTriggered = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到collider");
        }
    }

    /// <summary>
    /// 触发机关
    /// </summary>
    public abstract void Trigger(TiggerType triggerType);

    /// <summary>
    /// 检测机关是否可以触发
    /// </summary>
    /// <returns>机关是否可以触发</returns>
    protected abstract bool TriggerDetect();

    /// <summary>
    /// 机关是否被触发
    /// </summary>
    /// <returns></returns>
    public virtual bool IsTriggered()
    {
        return isTriggered;
    }
    
}
