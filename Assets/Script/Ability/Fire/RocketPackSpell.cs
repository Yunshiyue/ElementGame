using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPackSpell : Spell
{
    private float rocketMaxSpeed = 5f;
    private float slowdownRocketForce = 5f;
    private float speedUpRocketForce = 12f;
    //private Vector2 forceVector = new Vector2(0, 0);
    private float borkeDownStunTime = 1f;
    private Vector2 velocityVector = new Vector2(0, 0);

    private Rigidbody2D rigid;
    private MovementPlayer movementComponent;

    private GameObject rocketPackObject;
    private OnFloorDetector playerDetector;

    private bool isOn;
    private float curTime = 0f;
    public float rocketTotalTime = 10f;
    public override void Initialize()
    {
        base.Initialize();
        rigid = player.GetComponent<Rigidbody2D>();
        movementComponent = player.GetComponent<MovementPlayer>();

        playerAnim.SetSpell(this, SkillType.RocketPack);
        rocketPackObject = GameObject.Find("RocketPack");
        rocketPackObject.SetActive(false);

        playerDetector = GameObject.Find("FloorDetector").GetComponent<OnFloorDetector>();
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.RocketPack);
    }
    public override void ReleaseSpell()
    {
        //if (movementComponent.RequestChangeControlStatus(rocketTotalTime, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        //{
            rocketPackObject.SetActive(true);
            isOn = true;
        //}
    }
    public void RocketPackClock()
    {
        if(isOn)
        {
            velocityVector = rigid.velocity;
            if (movementComponent.IsFacingLeft())
            {
                velocityVector.x -= (velocityVector.x < 0 ? speedUpRocketForce : slowdownRocketForce) * Time.deltaTime;
                //forceVector.x = -rocketForce;
                velocityVector.x = velocityVector.x < -rocketMaxSpeed ? -rocketMaxSpeed : velocityVector.x;
            }
            else
            {
                velocityVector.x += (velocityVector.x > 0 ? speedUpRocketForce : slowdownRocketForce) * Time.deltaTime;
                velocityVector.x = velocityVector.x > rocketMaxSpeed ? rocketMaxSpeed : velocityVector.x;
            }
            rigid.velocity = velocityVector;

            if(velocityVector.x > 3 && playerDetector.isRightTouchingGround() ||
               velocityVector.x < -3 && playerDetector.isLeftTouchingGround())
            {
                movementComponent.RequestChangeControlStatus(borkeDownStunTime, MovementPlayer.PlayerControlStatus.Interrupt);
                isOn = false;
                curTime = 0f;
                rigid.velocity = Vector2.zero;
                rocketPackObject.SetActive(false);
            }

            curTime += Time.deltaTime;
            if (curTime >= rocketTotalTime)
            {
                isOn = false;
                curTime = 0f;
                rigid.velocity = Vector2.zero;
                rocketPackObject.SetActive(false);
            }
        }
    }

    public override void Disable()
    {
    }

    public override void Enable()
    {
    }

}
