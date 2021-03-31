using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementPlayer))]
public class Player : myUpdate
{
    private MovementPlayer movementComponent;

    private int priorityInType = 3;
    private UpdateType updateType = UpdateType.Player;

    private Vector2 tempMovement = new Vector2(0, 0);

    private void Awake()
    {
        GameObject.Find("UpdateManager").GetComponent<UpdateManager>().Register(this);
    }

    public override void Initialize()
    {
        movementComponent = GetComponent<MovementPlayer>();
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
    public override void MyUpdate()
    {
        MoveControl();
    }

    private void MoveControl()
    {
        tempMovement.x = Input.GetAxis("Horizontal");
        movementComponent.RequestMoveByFrame(tempMovement, MovementPlayer.MovementMode.PlayerControl);

        if(Input.GetButtonDown("Jump"))
        {
            movementComponent.RequestJump();
        }
        if(Input.GetButtonDown("Crouch"))
        {
            movementComponent.RequestCrouch();
        }
    }
}
