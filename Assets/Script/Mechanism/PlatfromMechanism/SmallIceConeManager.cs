/**
 * @Description: SmallIceConeManager为小冰锥管理器，在一定范围内随机生成小冰锥
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallIceConeManager : Mechanism
{
    [Header("生成参数")]
    public int generateNumber = 3;
    private int flowNum = 0;
    private Vector2 generatePosition = new Vector2(0, 0);

    [Header("时间参数")]
    public float flowIntervalTime = 1.0f;
    private float passTime = 0.0f;

    private Collider2D coll;
    private SmallIceCone[] cones;

    private PoolManager poolManager;

    protected override void Start()
    {
        base.Start();

        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        if (poolManager == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到PoolManager");
        }

        coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取collider失败");
        }

        cones = new SmallIceCone[generateNumber];

        //生成冰锥
        GenerateIceCone();

        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //计时
        passTime += Time.deltaTime;
        if (passTime > flowIntervalTime)
        {
            cones[flowNum].Trigger(TiggerType.Other);
            flowNum++;
            passTime = 0.0f;
        }

        //检测是否冰锥都掉落
        if (flowNum == generateNumber)
        {
            //掉落数置为0
            flowNum = 0;
            //cones.Clear();

            //生成新冰锥
            GenerateIceCone();

            enabled = false;
            isTriggered = false;
        }
    }

    public override void Trigger(TiggerType triggerType)
    {
        if (TriggerDetect())
        {
            isTriggered = true;
            enabled = true;
        }
    }

    protected override bool TriggerDetect()
    {
        return !isTriggered;
    }

    /// <summary>
    /// 在指定范围内随机生成冰锥
    /// </summary>
    private void GenerateIceCone()
    {
        for (int i = 0; i < generateNumber; i++)
        {
            Debug.Log(i);
            GameObject cone = poolManager.GetGameObject(SmallIceCone.SMALL_ICE_CONE);
            SmallIceCone a = cone.GetComponent<SmallIceCone>();

            //随机生成位置
            float x = Random.Range(coll.bounds.min.x, coll.bounds.max.x);
            float y = Random.Range(coll.bounds.min.y, coll.bounds.max.y);
            generatePosition.x = x;
            generatePosition.y = y;
            a.SetPosition(generatePosition);

            //加入列表
            cones[i] = a;
        }
    }
}
