/**
 * @Description: PressureSwitch是压力机关类，当物体触碰压力机关，机关会被触发。压力机关可与其他种类机关组合使用，可以触发多种机关
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitch : Mechanism
{
    /// <summary>
    /// 存放所有压在机关上的物体
    /// </summary>
    protected List<GameObject> pressingObjects = new List<GameObject>();

    /// <summary>
    /// 该开关要触发的机关
    /// </summary>
    public List<GameObject> mechanisms = new List<GameObject>();

    /// <summary>
    /// 相互配合的机关，全部打开后才可以触发对应的机关
    /// </summary>
    public List<GameObject> combinationMechanisms = new List<GameObject>();

    [Header("触发参数")]
    protected bool isPressed = false;
    protected int triggerNum = 0;

    [Header("时间参数")]
    public float triggerIntervalTime = 2.0f;
    protected float passTime = 0.0f;

    protected override void Start()
    {
        base.Start();

        if (mechanisms.Count == 0)
        {
            Debug.LogError("在" + gameObject.name + "中，未初始化mechanisms");
        }

        passTime = triggerIntervalTime;

        enabled = false;
    }

    protected void Update()
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
        //检测所有触发条件是否满足
        if (TriggerDetect())
        {
            isPressed = true;

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
                isTriggered = true;
            }
        }
    }

    /// <summary>
    /// 检测该机关是否还未触发且所有条件已满足
    /// </summary>
    /// <returns></returns>
    protected override bool TriggerDetect()
    {
        return !isTriggered && isPressed;
    }

    public override bool IsTriggered()
    {
        return isPressed;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        Rigidbody2D rigidbody;
        if (collision.TryGetComponent<Rigidbody2D>(out rigidbody))
        {
            Debug.Log("机关触发");
            //将物体添加至被压列表
            pressingObjects.Add(collision.gameObject);
            isPressed = true;

            //触发机关
            Trigger(Mechanism.TiggerType.Other);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //检测该物体是否压在上面
        if (pressingObjects.Contains(collision.gameObject))
        {
            Debug.Log("机关退出");
            //从列表中移除
            pressingObjects.Remove(collision.gameObject);

            //如果没有被任何物体压着，则改变被压状态
            if (pressingObjects.Count == 0)
            {
                isPressed = false;
            }
        }
    }
}