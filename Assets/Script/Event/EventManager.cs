/**
 * @Description: EventManager类是事件管理器类，用于事件的注册与触发管理
 * @Author: CuteRed

 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    /// <summary>
    /// 存储事件
    /// </summary>
    private Dictionary<string, Func<object[], object>> events 
        = new Dictionary<string, Func<object[], object>>();

    /// <summary>
    /// 跳跃状态改变
    /// </summary>
    public const string OnJumpNumChange = "OnJumpNumChange";


    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventName">时间名</param>
    /// <param name="action">方法</param>
    public void AddEvent(string eventName, Func<object[], object> action)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] += action;
        }
        else
        {
            events.Add(eventName, action);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="o"></param>
    /// <returns></returns>
    public object TriggerEvent(string eventName, object[] o)
    {
        if (events.ContainsKey(eventName))
        {
            return events[eventName](o);
        }
        else
        {
            return null;
        }
    }
}
