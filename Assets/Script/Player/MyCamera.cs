using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : myUpdate
{
    private int priority = 0;
    private UpdateType type = UpdateType.GUI;

    private Camera mainCamera;
    public Transform FollowedTarget;

    public float CameraChaseSpeed;

    private Collider2D dontChangeArea;
    public BoxCollider2D followArea;
    private float leftEdge;
    private float rightEdge;
    private float topEdge;
    private float bottomEdge;

    private bool isFollowed = true;
    private bool isBeyondDontChangeArea = false;

    private void Reset()
    {
        CameraChaseSpeed = 0f;
    }

    public override void Initialize()
    {
        mainCamera = GetComponent<Camera>();
        dontChangeArea = GetComponent<Collider2D>();
        leftEdge = followArea.transform.position.x - followArea.size.x / 2;
        rightEdge = followArea.transform.position.x + followArea.size.x / 2;
        topEdge = followArea.transform.position.y + followArea.size.y / 2;
        bottomEdge = followArea.transform.position.y - followArea.size.y / 2;
    }

    public override void MyUpdate()
    {
        if(isFollowed)
        {
            if(isBeyondDontChangeArea)
            {
                transform.Translate(Vector2.MoveTowards(transform.position, FollowedTarget.position, CameraChaseSpeed * Time.deltaTime), Space.Self);
            }
            else
            {

            }

        }
    }
    public override int GetPriorityInType()
    {
        return priority;
    }

    public override UpdateType GetUpdateType()
    {
        return type;
    }
}
