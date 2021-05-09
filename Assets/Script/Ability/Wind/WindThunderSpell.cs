using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindThunderSpell : Spell
{
    private MovementPlayer movement;
    private float windThunderStatusTotalTime = 0.2f;
    //private Collider2D windThunderDamageArea;
    private Vector2 targetPosition;
    public override void Initialize()
    {
        base.Initialize();
        movement = player.GetComponent<MovementPlayer>();
        //windThunderDamageArea = player.GetComponent<Collider2D>();
    }
    public override void Cast()
    {
        if(movement.RequestMoveByTime(targetPosition, windThunderStatusTotalTime, MovementPlayer.MovementMode.Ability))
        {
            playerAnim.SetUseSkillType(SkillType.WindThunder);
        }  
    }

    private float windThunderDistance = 7f;
    private bool isUnstoppable = false;//冲刺时是否有伤害
    public void SetTargetPosition(Vector2 position)
    {
        Vector2 temp = player.transform.position;
        targetPosition = position - temp;
        if (targetPosition.magnitude > windThunderDistance)//冲刺最长距离为7
        {
            targetPosition.Normalize();
            targetPosition *= windThunderDistance;
        }

        movement.SetGravity(false);

        if (movement.IsFacingLeft())
        {
            targetPosition.x = -targetPosition.x;
        }
        //isUnstoppable = true;
    }


    public override void Disable()
    {

    }

    public override void Enable()
    {

    }

    public override void ReleaseSpell()
    {
        
    }
}
