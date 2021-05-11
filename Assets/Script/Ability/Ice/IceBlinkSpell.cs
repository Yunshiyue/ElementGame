using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlinkSpell : Spell
{
    private ThunderAbility thunderAbility;
    private MovementPlayer movementPlayer;
    private CanFight canFight;
    public override void Initialize()
    {
        base.Initialize();
        playerAnim.SetSpell(this, SkillType.IceBlink);
        thunderAbility = player.GetComponent<ThunderAbility>();
        movementPlayer = player.GetComponent<MovementPlayer>();
        canFight = player.GetComponent<CanFight>();
    }
    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.IceBlink);
    }
    public override void ReleaseSpell()
    {
        GameObject target = thunderAbility.GetClosestTargetInList(thunderAbility.GetTargetInThunderCircle());
        if(target != null)
        {
            Vector2 targetPosition = target.transform.position;
            targetPosition.x += target.transform.localScale.x < 0 ? 2 : -2;
            movementPlayer.RequestMoveByFrame(targetPosition, MovementPlayer.MovementMode.Ability, Space.World);
            canFight.Attack(target.GetComponent<CanBeFighted>(), 1, AttackInterruptType.WEAK, ElementAbilityManager.Element.Ice);
        }
        //播放攻击动画
    }
    public override void Disable()
    {
    }

    public override void Enable()
    {
    }

}
