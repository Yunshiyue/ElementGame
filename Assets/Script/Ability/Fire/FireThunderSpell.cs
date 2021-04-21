using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireThunderSpell : Spell
{
   
    //使用该技能上升距离
    private Vector2 fireThunderVector = new Vector2(0, 0.5f);
    private float fireThunderMovementTime = 0.2f;

    private MovementPlayer movementComponent;

    public override void Initialize()
    {
        base.Initialize();
        movementComponent = player.GetComponent<MovementPlayer>();
    }
    public override void Cast()
    {  
        if (movementComponent.RequestMoveByTime(fireThunderVector, fireThunderMovementTime, MovementPlayer.MovementMode.Ability))
        {           
            movementComponent.playerAnim.SetUseSkillType(SkillType.FireThunder);//决定释放的技能动画是哪个 
        }
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
