/**
 * @Description: CableCarCollider是缆车碰撞体类，用于控制缆车的移动
 * @Author: CuteRed

 *      
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableCarCollider : MonoBehaviour
{
    /// <summary>
    /// 阻力
    /// </summary>
    private Vector3 direction = Vector3.zero;
    /// <summary>
    /// 上一帧位置
    /// </summary>
    private Vector3 location = Vector3.zero;
    private float speed = 5.0f;
    public float DEFAULT_SPEED = 5.0f;
    private LayerMask ground;

    private Rigidbody2D rbody;

    private GameObject left;
    private GameObject right;

    private SliderJoint2D sliderJoint;

    void Start()
    {
        //获取刚体
        rbody = GetComponent<Rigidbody2D>();
        if (rbody == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取rigidbody失败");
        }

        sliderJoint = GetComponent<SliderJoint2D>();

        Transform[] childTransform = GetComponentsInChildren<Transform>();
        for (int i = 0; i < childTransform.Length; i++)
        {
            if (childTransform[i].gameObject.name == "CableCarLeftCollider")
            {
                left = childTransform[i].gameObject;
            }
            else if (childTransform[i].gameObject.name == "CableCarRightCollider")
            {
                right = childTransform[i].gameObject;
            }
        }

        ////左
        //left = GameObject.Find("CableCarLeftCollider");
        //if (left == null)
        //{
        //    Debug.LogError("在" + gameObject.name + "中，获取CableCarLeftCollider失败");
        //}

        ////右
        //right = GameObject.Find("CableCarRightCollider");
        //if (right == null)
        //{
        //    Debug.LogError("在" + gameObject.name + "中，获取CableCarRightCollider失败");
        //}

        speed = DEFAULT_SPEED;

        ground = LayerMask.GetMask("Platform");

        //失效
        enabled = false;
    }                     

    void Update()
    {

        RaycastHit2D hit;
        //左检测
        if (direction.x < 0)
        {
            Debug.Log("左碰墙");
            hit = Physics2D.Raycast(left.transform.position, direction, 1.0f, ground);
        }
        //右检测
        else
        {
            Debug.Log("右碰墙");
            hit = Physics2D.Raycast(left.transform.position, direction, 1.0f, ground);
        }

        if (hit)
        {
            enabled = false;
            speed = DEFAULT_SPEED;

            //停止运动
            rbody.velocity = Vector3.zero;
        }
        else
        {
            rbody.velocity = direction * speed;
            //transform.position += direction * speed * Time.deltaTime;
            speed -= 0.01f;
            if (speed < 0.1f)
            {
                enabled = false;
                speed = DEFAULT_SPEED;
            }
        }
        
    }

    /// <summary>
    /// 设置方向，并打开机关，初始化速度
    /// </summary>
    /// <param name="force">力</param>
    public void Init(Vector3 force)
    {
        direction.x = force.x;

        if (!enabled)
        {
            enabled = true;
        }
        else
        {
            speed += DEFAULT_SPEED;
        }
    }
}
