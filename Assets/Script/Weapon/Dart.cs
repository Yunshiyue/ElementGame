/**
 * @Description: Dart类是飞镖类，模拟飞行道具，目前实现的功能有旋转、移动、撞到地图返回、撞到敌人造成伤害
 * 之后可能要修改的地方：OnTriggerEnter2D中的标签，由于飞行道具的伤害、目标位置、投掷者不确定，所以这些变量没有初始化，之后应该要修改
 * @Author: CuteRed

 *      

 * @Editor: CuteRed
 * @Edit: 1.增加了道具存在时间超时的判断
 *        2.增加了被攻击单位的列表
 *        
<<<<<<< HEAD

 * @Editor: CuteRed's daddy
 * @Edit: 1.增加了接口，给回旋镖设置目标投掷位置
 *        2.增加了接口，让player扔它
 *        3.改变了回旋镖死亡动作：setactive(false)而不是destroy
 *        4.增加了接口，设置抛出者，并设置判断位判定是否设置了抛出者
=======

 * @Editor: CuteRed
 * @Edit: 1.将直接销毁改为由对象池回收
 *        2.增加print和OnEnable函数
>>>>>>> a31ff31d88086891f0374cb8992a2db0ddfbe766
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : FlyingProp
{
    [Header("旋转参数")]
    float rotateSpeed = 1000f;

    [Header("移动参数")]
    float moveSpeed = 10f;

    /// <summary>
    /// 目标位置
    /// </summary>
    Vector2 targetPosition = new Vector2();

    /// <summary>
    /// 投掷者
    /// </summary>
    /// TODO 这里还没有初始化
    GameObject thrower;

    [Header("bool参数")]
    /// <summary>
    /// 旋转状态
    /// </summary>
    bool isRotating = false;

    /// <summary>
    /// 前进状态
    /// </summary>
    bool isGo = false;

    /// <summary>
    /// 返回状态
    /// </summary>
    bool isBack = false;

    [Header("伤害参数")]
    int damage = 1;
    /// <summary>
    /// 可以攻击
    /// </summary>
    CanFight fight;
    /// <summary>
    /// 被该飞镖攻击过的物体
    /// </summary>
    List<CanBeFighted> fought = new List<CanBeFighted>();

   
    

    // Start is called before the first frame update
    private void Start()
    {
        if(thrower == null)
        {
            Debug.LogError("飞镖类没有设置thrower！");
        }

        startTime = Time.time;

        fight = GetComponent<CanFight>();
        if (fight == null)
        {
            Debug.LogError("在" +  gameObject.name +"中，获取fight组件时出错");
        }

        if (thrower == null)
        {
            Debug.LogError("在" + gameObject.name + "中，初始化thrower时出错");
        }
    }

    private void OnEnable()
    {
        isRotating = false;
        isGo = false;
        isBack = false;
        startTime = Time.time;
        fought.Clear();
    }

    /// <summary>
    /// 初始化Dart，包括投掷者、目标位置，在每次生成飞镖时需要调用
    /// </summary>
    /// <param name="thrower">投掷者</param>
    public void Init(GameObject thrower)
    {
        this.thrower = thrower;
        targetPosition.y = thrower.transform.position.y;
        targetPosition.x = thrower.transform.position.x + 10 * thrower.transform.localScale.x;
    }

    // Update is called once per frame
    private void Update()
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
            Debug.Log("运动中" + gameObject.transform);
        }

        //print();
        //超时检测
        //TimeOutDetect();
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
        //武器到达目的点时
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            isBack = true;
            isGo = false;
        }

        //武器回到投掷者手中
        if (Vector2.Distance(transform.position, thrower.transform.position) < 0.01f && isBack)
        {
            isBack = false;
            //飞镖消失

            //Destroy(this);
            PoolManager.instance.RemoveGameObject(PoolManager.poolType.Dart, gameObject);
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
        if (collision.tag == "Enemy")
        {
            CanBeFighted beFought;
            if (collision.TryGetComponent<CanBeFighted>(out beFought))
            {
                fight.Attack(beFought, damage);
                fought.Add(beFought);
            }
        }

        //撞到地图，直接返回
        if (collision.tag == "Map")
        {
            isGo = false;
            isBack = true;
        }
    }


    /// <summary>
    /// 超时检测，超过最大时间后，销毁
    /// </summary>
    protected override void TimeOutDetect()
    {
        float currentTime = Time.time;
        if (startTime + MAX_EXIST_TIME > currentTime)
        {
            PoolManager.instance.RemoveGameObject(PoolManager.poolType.Dart, gameObject);
        }
    }

    /// <summary>
    /// debug使用
    /// </summary>
    private void print()
    {
        Debug.Log("isGo:" + isGo + "\nisBack:" + isBack + "\nisRotating" + isRotating + "\nposition" + gameObject.transform.position);
    }

}
