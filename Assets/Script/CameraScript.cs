using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraScript : MonoBehaviour
{
    public Transform follower;
    public float smoothing;
    public BoxCollider2D cameraBounds;

    public float offsetX = 0f;
    public float offsetY = 0f;
    
    private Vector3 targetPos;
    //相机移动范围
    private Vector2 minPos;
    private Vector2 maxPos;

    void Start()
    {
        getMoveArea();
    }

    void LateUpdate()
    {
        if (follower != null)
        {
            //Debug.Log("follower不为空!");
            if(transform.position != follower.position)
            {
                targetPos.x = follower.position.x+offsetX;
                targetPos.y = follower.position.y+offsetY;
                targetPos.z = transform.position.z;
                //移动范围
                targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);
                //transform朝targetPos移动smooth距离
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
            }
        }
       
    }

    private void getMoveArea()
    {
        minPos.x = cameraBounds.transform.position.x - (cameraBounds.size.x / 2);
        minPos.y = cameraBounds.transform.position.y - (cameraBounds.size.y / 2);
        maxPos.x = cameraBounds.transform.position.x + (cameraBounds.size.x / 2);
        maxPos.y = cameraBounds.transform.position.y + (cameraBounds.size.y / 2);
    }

}
