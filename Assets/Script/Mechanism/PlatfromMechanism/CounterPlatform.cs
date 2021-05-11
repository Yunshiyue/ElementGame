/**
 * @Description: CounterPlatform类为跳跃计数地板机关类，根据人物的跳跃次数决定是否出现
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterPlatform : Mechanism
{
    [Header("跳跃参数")]
    /// <summary>
    /// 出现时，跳跃次数的奇偶
    /// </summary>
    public int appearNum = 1;
    private Vector3 scale = new Vector3(1, 1, 1);

    private EventManager eventManager;

    protected override void Start()
    {
        base.Start();

        //初始化scale
        scale = transform.localScale;

        //初始化事件管理器
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        if (eventManager == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到EventManager");
        }

        //添加事件监听
        eventManager.AddEvent(EventManager.OnJumpNumChange, (object[] o)=>{
            ChangeState((int)o[0]);
            return null;
        });

        //初始化状态（奇数显示，偶数消失）
        ChangeState(1);
    }

    /// <summary>
    /// 此机关中不用
    /// </summary>
    /// <returns></returns>
    protected override bool TriggerDetect()
    {
        return true;
    }

    /// <summary>
    /// 此机关中不用
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        
    }

    /// <summary>
    /// 根据跳跃次数改变状态（出现或消失）
    /// </summary>
    /// <param name="jumpNum"></param>
    public void ChangeState(int jumpNum)
    {
        if (jumpNum == appearNum)
        {
            transform.localScale = scale;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}
