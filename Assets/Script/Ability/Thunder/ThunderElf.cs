using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderElf : MonoBehaviour
{
    private float ElfChasinSpeed = 15f;

    private GameObject nextTarget;
    private GameObject player;
    private GameObject temp;
    private CanFight playerHand;

    private ThunderAbility thunderAbility;

    private bool isChasingTarget;

    private float attackTimeDelta = 0.7f;
    private float attackTimeCur = 0f;
    private int attackTotalNumber = 2;
    private int attackCurNumber = 0;

    private int maxAttackAmout = 3;

    //private bool isReturnPlayer = false;
    //private float returnPlayerAttachingTime = 0.5f;
    //private float returnPlayerCurTime = 0f;
    public float GetTotalTime()
    {
        return maxAttackAmout * attackTimeDelta * (attackTotalNumber + 1);
    }

    private void Awake()
    {
        player = GameObject.Find("Player");
        thunderAbility = player.GetComponent<ThunderAbility>();
        playerHand = player.GetComponent<CanFight>();
        nextTarget = player;
    }
    public void AutoAttack()
    {
        if(isChasingTarget)
        {
            if(nextTarget.GetInstanceID() == player.GetInstanceID())
            {
                //追踪敌人
                temp = thunderAbility.GetClosestTargetInList(thunderAbility.GetTargetInThunderCircle());
                if(temp != null)
                {
                    nextTarget = temp;
                }
            }
            transform.position =
                Vector2.MoveTowards(transform.position, nextTarget.transform.position, ElfChasinSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position, nextTarget.transform.position) < 0.02f)
            {
                isChasingTarget = false;
            }
        }
        else
        {
            transform.position = nextTarget.transform.position;

            if (nextTarget.GetInstanceID() == player.GetInstanceID())
            {
                //追踪敌人
                temp = thunderAbility.GetClosestTargetInList(thunderAbility.GetTargetInThunderCircle());
                if (temp != null)
                {
                    nextTarget = temp;
                    isChasingTarget = true;
                }
            }
            //如果当前依附的单位已经死亡，则寻找下一个单位，如果没有，则追踪主角
            else if (nextTarget == null || nextTarget.activeSelf == false || attackCurNumber >= attackTotalNumber)
            {
                temp = thunderAbility.GetClosestTargetInList(thunderAbility.GetTargetInThunderCircle());
                if (temp != null)
                {
                    nextTarget = temp;
                }
                else
                {
                    nextTarget = player;
                }
                //清空攻击数据并进入追踪态
                isChasingTarget = true;
                attackCurNumber = 0;
                attackTimeCur = 0f;
            }
            else
            {
                attackTimeCur += Time.deltaTime;
                if (attackTimeCur >= attackTimeDelta)
                {
                    playerHand.Attack(nextTarget.GetComponent<CanBeFighted>(), 1, AttackInterruptType.WEAK);
                    attackTimeCur = 0f;
                    attackCurNumber++;
                }
            }

        }
    }
}
