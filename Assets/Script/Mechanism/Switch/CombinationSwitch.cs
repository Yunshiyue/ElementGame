/**
 * @Description: CombinationSwitch是组合机关类，组合机关可以与其他多个机关共同使用，当他们全部打开后，可触发(多对多)
 * 其他多个机关
 * @Author: CuteRed
1-3-6 10:40
 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationSwitch : Mechanism
{
    /// <summary>
    /// 机关的触发条件，一个机关可能有多个条件触发
    /// </summary>
    public List<Mechanism.TiggerType> triggerConditions = new List<TiggerType>();

    /// <summary>
    /// 该开关要触发的机关
    /// </summary>
    public List<GameObject> mechanisms = new List<GameObject>();

    /// <summary>
    /// 相互配合的机关，全部打开后才可以触发对应的机关
    /// </summary>
    public List<GameObject> combinationMechanisms = new List<GameObject>();

    [Header("触发参数")]
    private CanBeFoughtMachanism canBeFought;
    private int triggerNum = 0;

    [Header("时间参数")]
    public float triggerIntervalTime = 2.0f;
    private float passTime = 0.0f;

    protected override void Start()
    {
        base.Start();

        if (triggerConditions.Count == 0)
        {
            Debug.LogError("在" + gameObject.name + "中，未初始化triggerConditions");
        }

        if (mechanisms.Count == 0)
        {
            Debug.LogError("在" + gameObject.name + "中，未初始化mechanisms");
        }

        passTime = triggerIntervalTime;

        enabled = false;
    }

    private void Update()
    {
        //判断时间间隔
        if (passTime > triggerIntervalTime)
        {
            mechanisms[triggerNum].GetComponent<Mechanism>().Trigger(Mechanism.TiggerType.Switch);
            triggerNum++;
            passTime = 0.0f;
        }

        //机关触发完成
        else if (triggerNum == mechanisms.Count)
        {
            //机关失效
            enabled = false;
        }

        //更新时间
        passTime += Time.deltaTime;
    }


    /// <summary>
    /// 打开该开关
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        //检测是否有该触发条件
        if (triggerConditions.Contains(triggerType))
        {
            triggerConditions.Remove(triggerType);

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
            }
        }
    }

    /// <summary>
    /// 检测该机关是否还未触发且所有条件已满足
    /// </summary>
    /// <returns></returns>
    protected override bool TriggerDetect()
    {
        return !isTriggered && triggerConditions.Count == 0;
    }
}
