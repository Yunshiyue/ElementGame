using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(MovementPlayer))]
public class AttackPlayer : MonoBehaviour
{
    CanFight canFight;
    MovementPlayer movementComponent;

    //普通攻击范围、伤害、字符串标识(名字)
    public Collider2D normalAttackArea;
    public int normalAttackDamage = 1;
    private const string NORMAL_ATTACK_NAME = "NormalAttack";

    //闪烁距离、字符串表示(名字)
    public Vector2 blinkVector = new Vector2(5, 0);
    private const string BLINK_NAME = "BLINK";

    //冲刺距离、时间
    public Vector2 dashVector = new Vector2(8, 0);
    public float dashTime = 0.5f;
    private const string DASH_NAME = "DASH";

    private void Start()
    {
        canFight = GetComponent<CanFight>();
        if(canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
        }

        movementComponent = GetComponent<MovementPlayer>();
        if(movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }

        if (normalAttackArea == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有把普通攻击组件拖动到AttackPlayer脚本中！");
        }
    }

    //返回先后的问题，在一帧中只能释放一个技能
    public string AttackControl()
    {
        if (Input.GetButton(BLINK_NAME))
        {
            blink();
            return BLINK_NAME;
        }

        if (Input.GetButton(DASH_NAME))
        {
            dash();
            return DASH_NAME;
        }

        if (Input.GetButton(NORMAL_ATTACK_NAME))
        {
            NormalAttack();
            return NORMAL_ATTACK_NAME;
        }

        return null;
    }

    private void NormalAttack()
    {
        canFight.AttackArea(normalAttackArea, normalAttackDamage);
    }

    private void blink()
    {
        movementComponent.RequestMoveByFrame(blinkVector, MovementPlayer.MovementMode.Ability);
    }

    private void dash()
    {
        movementComponent.RequestMoveByTime(dashVector, dashTime, MovementPlayer.MovementMode.Ability);
    }
}
