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
    private CanFight canFight;
    private MovementPlayer movementComponent;

    //当前技能编号 用于人物技能动画帧事件判断处理哪一个动画
    private int currentSkill = 0;

    //被击中单位的位置
    private Vector2 attackedPos = new Vector2(0, 0);
    //普通攻击范围、伤害、字符串标识(名字)
    private Collider2D normalAttackArea;
    private int normalAttackDamage = 1;
    public const string NORMAL_ATTACK_NAME = "NormalAttack";
    //该攻击打断敌人参数
    private float normalinterruptTime = 0.3f;
    private Vector2 normalinterruptVector = new Vector2(1f, 0);

    //雷火之吼攻击范围、伤害、字符串标识(名字)
    private PolygonCollider2D thunderFireAttackArea;
    private int thunderFireAttackDamage = 1;
    public const string THUNDER_FIRE_NAME = "ThunderFire";
    //该技能攻击打断敌人参数
    private float thunderFireinterruptTime = 0.5f;
    private Vector2 thunderFireinterruptVector = new Vector2(2f, 0);
    //使用该技能上升距离
    private Vector2 ThunderFireVector = new Vector2(0, 0.15f);
    private GameObject thunderFire;

    //闪烁距离、字符串表示(名字)
    private Vector2 blinkVector = new Vector2(5, 0);
    public const string BLINK_NAME = "Blink";

    //冲刺距离、时间
    private Vector2 dashVector = new Vector2(2f, 0);
    private float dashTime = 0.3f;
    public const string DASH_NAME = "Dash";

    private string targetLayerName = "Enemy";

    //Dart技能
    private Vector2 dartTargetPosition = new Vector2(0, 0);
    private int dartDistance = 10;

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

        //之后根据属性搭配来进行判断 是否设为true
        thunderFire = gameObject.transform.Find("ThunderFire").gameObject;
        thunderFire.SetActive(true);
        thunderFireAttackArea = GameObject.Find("ThunderFireCollider").GetComponent<PolygonCollider2D>();
        if (thunderFireAttackArea == null)
        {
            Debug.LogError("在" + gameObject.name + "中，没有把雷火攻击组件拖动到AttackPlayer脚本中！");
        }
    }


    //debug出攻击到的单位
    StringBuilder targetsName = new StringBuilder();
    CanBeFighted[] targets;
    public void NormalAttack()
    {
        //动画处理
        if(movementComponent.RequestChangeControlStatus(0.1f, MovementPlayer.PlayerControlStatus.AbilityNeedControl)){
            movementComponent.playerAnim.SetAbilityNum(0);//状态改变，动画机处理攻击动画
        }
        else
        {
            Debug.Log("普通攻击请求失败");
        }
        
    }
    //帧事件调用攻击
    public void AttackEvent()
    {
        Debug.Log("AttackEvent");
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(normalAttackArea, normalAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
                BeatBack(a, normalinterruptTime, normalinterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());
            targetsName.Clear();
        }
    }

    //攻击击退效果
    public void BeatBack(CanBeFighted a,float interruptTime,Vector2 interruptVector)
    {
        MovementEnemies movement = a.gameObject.GetComponent<MovementEnemies>();
        attackedPos.x = a.transform.position.x;
        attackedPos.y = a.transform.position.y;

        if (movement.RequestChangeControlStatus(interruptTime, MovementEnemies.EnemyStatus.Stun))
        {
            Debug.Log("击退");
            if (a.transform.position.x > transform.position.x)
            {
                movement.RequestMoveByTime(attackedPos + interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
            }
            else
            {
                movement.RequestMoveByTime(attackedPos - interruptVector, interruptTime, MovementEnemies.MovementMode.Attacked);
            }
        }
    }

    //技能帧事件
    
    public void SkillEvent()
    {
        switch (currentSkill)
        {
            case 1:
                ThunderFireEvent();
                break;
        }
    }
    public void Skill()
    {
        Debug.Log("事件事件！");
    }

    //雷火 长按主 + 雷
    //瞬间在主角周围造成小范围爆炸效果，伤害范围主要向上
    public void ThunderAndFire()
    {
        //动画处理
        if (movementComponent.RequestChangeControlStatus(0.3f, MovementPlayer.PlayerControlStatus.AbilityWithMovement))
        {
            //释放该技能上升一小段距离
            if (movementComponent.RequestMoveByTime(ThunderFireVector, 0.3f, MovementPlayer.MovementMode.Ability))
            {
                currentSkill = 1;
                movementComponent.playerAnim.SetAbilityNum(currentSkill);//状态改变，动画机处理攻击动画   
                //不知道为啥帧事件不起作用了，暂放在这
                //SkillEvent();
            }
        }
        else
        {
            Debug.Log("雷火攻击请求失败");
        }
    }

    //ThunderAndFire()调用人物释放技能动画，其释放技能动画帧事件再根据SkillEvent()调用ThunderFireEvent()
    public void ThunderFireEvent()
    {
        thunderFire.GetComponent<Animator>().SetBool("effect", true);
        Debug.Log("状态：" );
        //找到攻击命中的单位 canFight.AttackArea实现了攻击
        targets = canFight.AttackArea(thunderFireAttackArea, thunderFireAttackDamage);
        if (targets != null)
        {
            foreach (CanBeFighted a in targets)
            {
                targetsName.Append(" ");
                targetsName.Append(a.gameObject.name);
               //击退效果
                BeatBack(a, thunderFireinterruptTime, thunderFireinterruptVector);
            }
            Debug.Log("打到了： " + targetsName.ToString());

            targetsName.Clear();
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

    public void Dart()
    {
        if (movementComponent.RequestChangeControlStatus(0.05f, MovementPlayer.PlayerControlStatus.AbilityNeedControl))
        {
            GameObject dart = GameObject.Find("PoolManager").GetComponent<PoolManager>().GetGameObject(PoolManager.poolType.Dart, gameObject.transform.position);

            //修改位置
            dartTargetPosition.x = gameObject.transform.position.x + dartDistance * gameObject.transform.localScale.x;
            dartTargetPosition.y = gameObject.transform.position.y;
            
            dart.GetComponent<Dart>().Initialize(gameObject, dartTargetPosition);
        }
    }
}
