using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIndShortSpell : Spell
{
    //使用该技能自身后退距离
    private Vector2 windVector = new Vector2(-0.15f, 0);
    private float windShortMainStatusTime = 0.2f;
    private MovementPlayer movementComponent;

    public override void Initialize()
    {
        base.Initialize();
        movementComponent = player.GetComponent<MovementPlayer>();
    }
    
    public override void Cast()
    {
        if (movementComponent.RequestMoveByTime(windVector, windShortMainStatusTime, MovementPlayer.MovementMode.Ability))
        {
            playerAnim.SetUseSkillType(SkillType.WindShort);
        }
    }
    //public void EffectActive()
    //{
    //    leftIceThunder.SetActive(true);
    //    rightIceThunder.SetActive(true);
    //}
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
