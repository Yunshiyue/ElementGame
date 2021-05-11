/**
 * @Description: TimeLimitedSwitch为显示开关类，当开关打开后，经过一定时间如果没有触发相应的开关，则关闭开关
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLimitedSwitch : CombinationSwitch
{
    /// <summary>
    /// 机关失效时间
    /// </summary>
    public float stopTime = 0.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 打开该开关
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        //检测是否有该触发条件
        if (triggerConditionsDic.ContainsKey(triggerType))
        {
            triggerConditionsDic[triggerType] = true;

            //检测所有触发条件是否满足
            if (TriggerDetect())
            {
                isTriggered = true;

                //检测所有组合机关是否已经触发
                bool isAllTriggered = true;
                foreach (GameObject comMechanism in combinationMechanisms)
                {
                    //如果没触发
                    if (!comMechanism.GetComponent<Mechanism>().IsTriggered())
                    {
                        isAllTriggered = false;
                        break;
                    }
                }

                //如果其他组合机关全部打开，则触发
                if (isAllTriggered)
                {
                    enabled = true;
                }
                else
                {
                    //一定时间后关闭开关
                    StartCoroutine(nameof(SwitchOff));
                }
            }
        }
    }

    /// <summary>
    /// 一定时间后关闭开关
    /// </summary>
    /// <returns></returns>
    private IEnumerator SwitchOff()
    {
        Debug.Log("协程启动");
        yield return new WaitForSeconds(stopTime);

        //检测所有组合机关是否已经触发
        bool isAllTriggered = true;
        foreach (GameObject comMechanism in combinationMechanisms)
        {
            //如果没触发
            if (!comMechanism.GetComponent<Mechanism>().IsTriggered())
            {
                isAllTriggered = false;
                break;
            }
        }

        //如果其他组合机关没有全部打开，则关闭机关
        if (!isAllTriggered)
        {
            isTriggered = false;
            ShutDownConditions();
        }
    }

    //所有触发条件失效
    private void ShutDownConditions()
    {
        foreach (Mechanism.TiggerType tiggerType in triggerConditionsDic.Keys)
        {
            triggerConditionsDic[tiggerType] = false;
        }
    }
}
