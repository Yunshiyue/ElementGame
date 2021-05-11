/**
 * @Description: TriggerablePlatform为可触发的平台类，由Switch机关触发。默认脚本失效，在被触发后生效
 * @Author: CuteRed

 *     
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerablePlatform : Mechanism, Movable
{
    [Header("移动参数")]
    public float speed = 1.0f;
    public Vector3 direction = new Vector3(0, 1, 0);
    public float moveTime = 2.5f;
    private float passTime = 0.0f;


    protected override void Start()
    {
        base.Start();

        enabled = false;
    }

    private void Update()
    {
        //更新时间
        UpdateTime();

        //移动
        Movement();
    }

    /// <summary>
    /// 向上移动
    /// </summary>
    public void Movement()
    {
        //向指定方向移动
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// 触发
    /// </summary>
    /// <param name="triggerType"></param>
    public override void Trigger(TiggerType triggerType)
    {
        enabled = true;
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    private void UpdateTime()
    {
        passTime += Time.deltaTime;
        if (passTime > moveTime)
        {
            enabled = false;
        }
    }

    /// <summary>
    /// 触发检测（在此类中用不到）
    /// </summary>
    /// <returns></returns>
    protected override bool TriggerDetect()
    {
        return true;
    }
}
