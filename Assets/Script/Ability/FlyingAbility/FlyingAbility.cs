/**
 * @Description: FlyingAbility为所有技能中飞行道具的父类
 * 
 * 初始化说明：
 * 1.此类中的部分参数必须初始化，这些参数包括：
 * startPosition、thrower
 * 2.部分参数可以选择初始化
 * damage（默认为1）
 * 3.在使用对象池生成对象后，必须调用相应参数的Set函数初始化
 * 4.部分子类中仍有参数需要初始化，详情见子类
 * 
 * @Author: CuteRed

 *
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingAbility : myUpdate
{
    [Header("时间参数")]
    public float maxExistTime = 5.0f;
    public float existTime = 0;

    [Header("移动参数")]
    protected Vector2 startPosition;
    protected float speed = 10.0f;

    [Header("伤害参数")]
    protected int damage = 1;
    protected GameObject thrower;
    protected CanFight canFight;
    protected string targetLayerName = "Enemy";
    protected LayerMask targetLayer;
    protected List<CanBeFighted> fought = new List<CanBeFighted>();

    [Header("其他参数")]
    protected LayerMask ground;
    protected LayerMask mechanism;
    protected Collider2D coll;
    protected PoolManager poolManager;

    [Header("优先级")]
    protected UpdateType updateType = UpdateType.PoolThing;
    protected int priorityInType = 0;

    [Header("消失检测参数")]
    protected bool isTimeOut = false;

    public override void Initialize()
    {
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        if (poolManager == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取PoolManager失败");
        }

        coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取Collider2D失败");
        }

        ground = LayerMask.GetMask("Platform");
        mechanism = LayerMask.NameToLayer("Mechanism");

        //设置目标层
        targetLayer = LayerMask.NameToLayer(targetLayerName);
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    /// <summary>
    /// 设置道具最大的存在时间
    /// </summary>
    /// <param name="time"></param>
    public void SetMaxExistTime(float time)
    {
        maxExistTime = time;
    }

    /// <summary>
    /// 设置目标层次名字
    /// </summary>
    /// <param name="name"></param>
    public void SetTargetLayerName(string name)
    {
        targetLayerName = name;

        //设置目标层
        targetLayer = LayerMask.NameToLayer(targetLayerName);
    }

    /// <summary>
    /// 设置初始位置
    /// </summary>
    /// <param name="position"></param>
    public void SetStartPosition(Vector2 position)
    {
        startPosition = position;
        transform.position = position;
    }

    /// <summary>
    /// 设置技能释放者
    /// </summary>
    /// <param name="thrower"></param>
    public void SetThrower(GameObject thrower)
    {
        this.thrower = thrower;
        canFight = thrower.GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取CanFight出错");
        }

        //transform.SetParent(thrower.transform, true);
    }

    /// <summary>
    /// 设置速度
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    /// <summary>
    /// 更新存在时间
    /// </summary>
    public void TimeUpdate()
    {
        existTime += Time.deltaTime;
        if (existTime >= maxExistTime)
        {
            isTimeOut = true;
        }
    }

    protected abstract void Movement();

    protected abstract void Disappear();

    protected abstract bool DisappearDetect();

    /// <summary>
    /// 参数还原
    /// </summary>
    protected virtual void Clear()
    {
        fought.Clear();
        isTimeOut = false;
        existTime = 0.0f;
    }
}
