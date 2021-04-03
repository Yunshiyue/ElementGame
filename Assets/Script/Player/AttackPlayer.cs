/**
 * @Description: 主角的技能释放组件，依赖CanFight组件，接收control脚本的控制信息，负责技能释放的具体实现。
 *               通过向移动组件requestChangeControlStatus的返回值来判读自己能够进行想要是放的技能。
 *               如果能够，可以向移动组件请求改变移动状态，同时应该向动画组间申请播放技能动画。
 * @Author: ridger

 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(MovementPlayer))]
public class AttackPlayer : MonoBehaviour
{
    CanFight canFight;
    MovementPlayer movementComponent;

    //普通攻击范围、伤害、字符串标识(名字)
    private Collider2D normalAttackArea;
    private int normalAttackDamage = 1;
    public const string NORMAL_ATTACK_NAME = "NormalAttack";

    //闪烁距离、字符串表示(名字)
    private Vector2 blinkVector = new Vector2(5, 0);
    public const string BLINK_NAME = "Blink";


    //冲刺距离、时间
    private Vector2 dashVector = new Vector2(2f, 0);
    private float dashTime = 0.3f;
    public const string DASH_NAME = "Dash";

    private string targetLayerName = "Enemy";

    private void Start()
    {
        canFight = GetComponent<CanFight>();
        if(canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
        }

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);

        movementComponent = GetComponent<MovementPlayer>();
        if(movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }

        normalAttackArea = GameObject.Find("NormalAttack").GetComponent<Collider2D>();
        if (normalAttackArea == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有把普通攻击组件拖动到AttackPlayer脚本中！");
        }
    }


    //debug出攻击到的单位
    StringBuilder targetsName = new StringBuilder();
    CanBeFighted[] targets;
    public void NormalAttack()
    {
        Debug.Log("普通攻击！");

        //动画处理
        if(movementComponent.RequestChangeControlStatus(0.1f, MovementPlayer.PlayerControlStatus.AbilityNeedControl)){
            movementComponent.playerAnim.SetAbilityNum(0);
            //找到攻击命中的单位 canFight.AttackArea实现了攻击
            targets = canFight.AttackArea(normalAttackArea, normalAttackDamage);
            if (targets != null)
            {
                foreach (CanBeFighted a in targets)
                {
                    targetsName.Append(" ");
                    targetsName.Append(a.gameObject.name);
                }
                Debug.Log("打到了： " + targetsName.ToString());
                targetsName.Clear();
            }
        }
        else
        {
            Debug.Log("普通攻击请求失败");
        }
        
    }

    public void blink()
    {
        if(movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.AbilityNeedControl) )
        {
            movementComponent.RequestMoveByFrame(blinkVector, MovementPlayer.MovementMode.Ability);
        }
    }

    public void dash()
    {
        if(movementComponent.RequestChangeControlStatus(dashTime, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            movementComponent.RequestMoveByTime(dashVector, dashTime, MovementPlayer.MovementMode.Ability);
        }
    }
}
