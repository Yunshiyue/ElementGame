/**
 * @Description: 主角的风属性瞄准镜脚本，当主角施法时，瞄准镜物体被active，该脚本接管移动控制。
 *               主角施法完成时，通过GetPosition获得瞄准镜物体的位置，以控制释放技能的目的地点。同时瞄准镜被disactive
 * @Author: ridger

 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SightHead : myUpdate
{
    //控制镜头移动速度
    private float speed = 8f;

    private int priorityInType = 1;
    private UpdateType type = UpdateType.Player;
    private Text debugInfoUI;
    private StringBuilder debugInfo = new StringBuilder(512);

    public Vector2 GetPosition()
    {
        return transform.position;
    }
    //现在先直接set主角，以后考虑设计自动寻敌
    public void SetPosition(in Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    public override UpdateType GetUpdateType()
    {
        return type;
    }
    public override void Initialize()
    {
        debugInfoUI= GameObject.Find("SightHeadDebugInfo").GetComponent<Text>();
        //return;
    }

    public override void MyUpdate()
    {
        Movement();
        SetDebugInfo();
    }
    private void SetDebugInfo()
    {
        debugInfo.AppendLine("准星位置");
        debugInfo.Append(transform.position);

        debugInfoUI.text = debugInfo.ToString();
        debugInfo.Clear();
    }
    private Vector2 movementVector = new Vector2();
    private void Movement()
    {
        movementVector.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        movementVector.y = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.Translate(movementVector);
    }
    public override int GetPriorityInType()
    {
        return priorityInType;
    }
}
