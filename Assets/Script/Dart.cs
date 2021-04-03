/**
 * @Description: Dart类是飞镖类，模拟飞行道具，目前实现的功能有旋转、移动、撞到地图返回、撞到敌人造成伤害
 * 之后可能要修改的地方：OnTriggerEnter2D中的标签，由于飞行道具的伤害、目标位置、投掷者不确定，所以这些变量没有初始化，之后应该要修改
 * @Author: CuteRed

 *           
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    [Header("旋转参数")]
    float rotateSpeed = 1000f;

    [Header("移动参数")]
    float moveSpeed = 10f;

    /// <summary>
    /// 目标位置
    /// </summary>
    Vector2 targetPosition;

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
    int damage;
    /// <summary>
    /// 可以攻击
    /// </summary>
    CanFight fight;


    // Start is called before the first frame update
    void Start()
    {
        fight = GetComponent<CanFight>();
    }

    // Update is called once per frame
    void Update()
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
        }
    }

    /// <summary>
    /// 自旋
    /// </summary>
    void SelfRotate()
    {
        transform.Rotate(-Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 移动，移动到指定位置
    /// </summary>
    void Movement()
    { 

        //武器到达目的点时
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            isBack = true;
            isGo = false;
        }

        //武器回到投掷者手中
        if (Vector2.Distance(transform.position, thrower.transform.position) < 0.01f)
        {
            isBack = false;
            //飞镖消失
            Destroy(this);
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
            fight.Attack(collision.GetComponent<CanBeFighted>(), damage);
        }

        //撞到地图，直接返回
        if (collision.tag == "Map")
        {
            isGo = false;
            isBack = true;
        }
    }
}
