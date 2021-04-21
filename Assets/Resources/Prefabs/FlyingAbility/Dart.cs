/**
 * @Description: Dart类是飞镖类，模拟飞行道具，目前实现的功能有旋转、移动、撞到地图返回、撞到敌人造成伤害
 * 之后可能要修改的地方：OnTriggerEnter2D中的标签，由于飞行道具的伤害、目标位置、投掷者不确定，所以这些变量没有初始化，之后应该要修改
 * @Author: CuteRed

 *      

 * @Editor: CuteRed
 * @Edit: 1.增加了道具存在时间超时的判断
 *        2.增加了被攻击单位的列表
 *        

 * @Editor: CuteRed's daddy
 * @Edit: 1.增加了接口，给回旋镖设置目标投掷位置
 *        2.增加了接口，让player扔它
 *        3.改变了回旋镖死亡动作：setactive(false)而不是destroy
 *        4.增加了接口，设置抛出者，并设置判断位判定是否设置了抛出者
 *        

 * @Editor: CuteRed
 * @Edit: 1.将直接销毁改为由对象池回收
 *        2.增加print和OnEnable函数
 *        

 * @Editor: ridger
 * @Edit: 1.增加了碰到墙面返回的特性，通过每update一帧调用射线检测实现(尝试过用collider的isTouching但是失败了，原因未知)
 *          当向前飞的时候，碰到墙则返回；当飞回来的时候，碰到墙则消失
 *        2.将一些GetComponent的方法变为了成员变量，比如poolManager、throwerHand
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : FlyingProp
{
    public const string DART = "Dart";

    [Header("旋转参数")]
    float rotateSpeed = 1000f;

    [Header("移动参数")]
    private float moveSpeed = 10f;

    /// <summary>
    /// 目标位置
    /// </summary>
    private Vector2 targetPosition = new Vector2();

    /// <summary>
    /// 投掷者
    /// </summary>
    /// TODO 这里还没有初始化
    private GameObject thrower;

    [Header("bool参数")]
    /// <summary>
    /// 旋转状态
    /// </summary>
    private bool isRotating = false;

    /// <summary>
    /// 前进状态
    /// </summary>
    private bool isGo = false;

    /// <summary>
    /// 返回状态
    /// </summary>
    private bool isBack = false;

    [Header("伤害参数")]
    private int damage = 1;
    
    /// <summary>
    /// 被该飞镖攻击过的物体
    /// </summary>
    private List<CanBeFighted> fought = new List<CanBeFighted>();

    private LayerMask ground;
    private PoolManager poolManager;
    private CanFight throwerHand;

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        ground = LayerMask.GetMask("Platform");

        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        if(poolManager == null)
        {
            Debug.LogError("飞镖中没有找到pollManager！");
        }

        if (thrower == null)
        {
            Debug.LogError("飞镖类没有设置thrower！");
        }

        if (thrower == null)
        {
            Debug.LogError("在" + gameObject.name + "中，初始化thrower时出错");
        }
    }

    //private void OnEnable()
    //{
    //    ground = LayerMask.GetMask("Platform");

    //    isRotating = false;
    //    isGo = false;
    //    isBack = false;
    //    existTime = 0f;
    //    fought.Clear();
    //}

    /// <summary>
    /// 初始化Dart，包括投掷者、目标位置，在每次生成飞镖时需要调用
    /// </summary>
    /// <param name="thrower">投掷者</param>
    public void SetThrower(GameObject thrower)
    {
        this.thrower = thrower;
        throwerHand = thrower.GetComponent<CanFight>();
        if(throwerHand == null)
        {
            Debug.LogError("在飞镖中，投掷者没有CanFight类！");
        }
    }

    public void SetTargetPosition(Vector2 tarPosition)
    {
        targetPosition.x = tarPosition.x;
        targetPosition.y = tarPosition.y;
    }

    /// <summary>
    /// 初始化飞镖，设置投掷者和目标位置
    /// </summary>
    /// <param name="thrower">投掷者</param>
    /// <param name="tarPosition">目标位置</param>
    public void Init(GameObject thrower, Vector2 tarPosition)
    {
        Debug.Log("thrower:" + thrower.name);
        SetThrower(thrower);
        SetTargetPosition(tarPosition);

        ground = LayerMask.GetMask("Platform");

        //将各状态变量设为初值
        isRotating = false;
        isGo = false;
        isBack = false;
        existTime = 0f;
        fought.Clear();

    }

    // Update is called once per frame
    public override void MyUpdate()
    {
        //刚投掷时，把对应bool变量置为true
        if (!isGo && !isBack)
        {
            isGo = true;
            isRotating = true;
        }

        //运动中
        if (isRotating)
        {
            SelfRotate();
            Movement();
            //Debug.Log("Dart运动中" + gameObject.transform);
        }

        //超时检测
        TimeOutDetect();
    }

    /// <summary>
    /// 自旋
    /// </summary>
    private void SelfRotate()
    {
        transform.Rotate(-Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 移动，移动到指定位置
    /// </summary>
    protected override void Movement()
    {
        //武器到达目的点时 或者 碰到墙
        if(isGo && !isBack)
        {
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f || Physics2D.Raycast(transform.position, Vector2.right, 0.5f, ground))
            {
                isBack = true;
                isGo = false;
            }
        }
        else
        {
            if (Physics2D.Raycast(transform.position, Vector2.right, 0.5f, ground))
            {
                Disappear();
            }
        }

        //武器回到投掷者手中
        if (Vector2.Distance(transform.position, thrower.transform.position) < 0.2f && isBack)
        {
            isBack = false;
            //回到对象池
            Disappear();
        }

        //在前进状态下，前往目标地点
        if (isGo && !isBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        //在返回状态下,返回到投掷者位置
        if (isBack && !isGo)
        {
            transform.position = Vector2.MoveTowards(transform.position, thrower.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //撞到人，造成伤害
        CanBeFighted beFought;
        if (collision.TryGetComponent<CanBeFighted>(out beFought))
        {
            //不可对同一目标造成2次伤害
            if (!fought.Contains(beFought))
            {
                throwerHand.Attack(beFought, damage);
                fought.Add(beFought);
                Debug.Log("Dart对" + beFought.name + "造成伤害");
            }
        }        
    }

    /// <summary>
    /// 超时检测，超过最大时间后，销毁
    /// </summary>
    protected override void TimeOutDetect()
    {
        existTime += Time.deltaTime;
        if (existTime > MAX_EXIST_TIME)
        {
            Disappear();
        }
    }
    protected override void Disappear()
    {
        Debug.Log("Dart:消失");
        poolManager.RemoveGameObject("Dart", gameObject);
    }

    /// <summary>
    /// debug使用
    /// </summary>
    private void print()
    {
        Debug.Log("isGo:" + isGo + "\nisBack:" + isBack + "\nisRotating" + isRotating + "\nposition" + gameObject.transform.position);
    }

}
