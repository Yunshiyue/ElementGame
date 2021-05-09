using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemies : MonoBehaviour
{
    // Start is called before the first frame update
    public bool canActiveAttack = false;
    public bool isHitPlayer = false;
    public float attackLength = 0f;
    public bool isInAttackLength = false;

    //protected Rigidbody2D rb;
    protected CanFight canFight;
    protected CanBeFighted[] targets;
    //protected StringBuilder targetsName = new StringBuilder();


    //技能CD
    public float skillCD1 = 0f;
    public float skillCD2 = 0f;
    public float skillCD3 = 0f;
    public float skillCD4 = 0f;
    
    public float curSkillCD1 = 0f;
    public float curSkillCD2 = 0f;
    public float curSkillCD3 = 0f;
    public float curSkillCD4 = 0f;

    public bool isInAction = false;

    protected GameObject player;
    void Start()
    {
        player = GameObject.Find("Player");
        //rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        AttackLengthCheck();
    }
    /// <summary>
    /// 攻击范围检测，判断主角是否在攻击范围内
    /// </summary>
    public virtual void AttackLengthCheck()
    {
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < attackLength)
        {
            isInAttackLength = true;
        }
        else
        {
            isInAttackLength = false;
        }
    }
    public virtual void AttackControl()
    {

    }

    public virtual void Skill1()
    {

    }
    public virtual void Skill2()
    {

    } 
    public virtual void Skill3()
    {

    }
    public virtual void Skill4()
    {

    }
    public void noInAction()
    {
        isInAction = false;
    }
}
