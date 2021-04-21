using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderLinkSpell : Spell
{
    private ThunderAbility thunderAbility;

    private ThunderLink thunderLinkScript;
    private List<GameObject> thunderLinkHasAttackedList = new List<GameObject>(16);
    private float thunderLinkIntervalTime = 1f;
    private float thunderLinkCurTime = 0f;
    private int thunderLinkAttackTotalTimes = 6;
    private int thunderLinkAttackedTimes = 0;
    private bool isThunderLink = false;
    private List<GameObject> tempTargetsInThunderCircle = new List<GameObject>(32);
    Vector2 thunderLinkStartPosition = new Vector2(0, 0);

    public void ThunderLinkClock()
    {
        if(isThunderLink)
        {
            if (thunderLinkAttackedTimes >= thunderLinkAttackTotalTimes)
            {
                isThunderLink = false;
                //关闭spell
                thunderLinkCurTime = 0f;
                thunderLinkAttackedTimes = 0;
                thunderLinkHasAttackedList.Clear();
                thunderLinkScript.gameObject.SetActive(false);
                Debug.Log("闪电链攻击达到上限次数，结束");
            }

            thunderLinkCurTime += Time.deltaTime;
            if (thunderLinkCurTime >= thunderLinkIntervalTime)
            {
                //执行spell
                thunderLinkCurTime = 0f;
                thunderLinkAttackedTimes++;

                Except(thunderAbility.GetTargetInThunderCircle(), thunderLinkHasAttackedList, out tempTargetsInThunderCircle);
                GameObject target = thunderAbility.GetClosestTargetInList(tempTargetsInThunderCircle);
                if (target != null)
                {
                    thunderLinkHasAttackedList.Add(target);
                }
                tempTargetsInThunderCircle = null;


                //如果当前找不到敌人，则终止闪电链技能
                if (target == null)
                {
                    isThunderLink = false;
                    thunderLinkCurTime = 0f;
                    thunderLinkAttackedTimes = 0;
                    thunderLinkHasAttackedList.Clear();
                    thunderLinkScript.gameObject.SetActive(false);
                    Debug.Log("闪电链攻击找不到敌人，结束");
                }
                else
                {
                    //通知函数
                    Debug.Log("闪电链攻击敌人：" + target.gameObject.name);
                    thunderLinkScript.SetTarget(target);
                }
            }
        }
    }
    private void Except(List<GameObject> allList, List<GameObject> excpetList, out List<GameObject> resultList)
    {
        resultList = new List<GameObject>(allList.Count);
        GameObject temp;
        for (int i = 0; i < allList.Count; i++)
        {
            temp = allList[i];
            if (!excpetList.Contains(temp))
            {
                resultList.Add(temp);
            }
        }
    }


    public override void Cast()
    {
        playerAnim.SetUseSkillType(SkillType.ThunderFire);
    }

    public override void ReleaseSpell()
    {
        if (isThunderLink)
        {
            Debug.Log("闪电链已经执行！");
            return;
        }

        isThunderLink = true;
        //Spell开始
        thunderLinkScript.gameObject.SetActive(true);

        thunderLinkStartPosition.x = player.transform.localScale.x < 0 ? player.transform.position.x - 1 : player.transform.position.x + 1;
        thunderLinkStartPosition.y = player.transform.position.y;

        thunderLinkScript.SetStartPosition(thunderLinkStartPosition);
    }

    public override void Disable()
    {
    }

    public override void Enable()
    {
    }

    public override void Initialize()
    {
        base.Initialize();
        thunderLinkScript = GameObject.Find("ThunderLink").GetComponent<ThunderLink>();
        thunderLinkScript.SetCanFight(player.GetComponent<CanFight>());
        thunderLinkScript.gameObject.SetActive(false);
        if(thunderLinkScript == null)
        {
            Debug.LogError("在ThunderLinkSpell中，没找到闪电链物体脚本！");
        }

        thunderAbility = player.GetComponent<ThunderAbility>();
        if(thunderAbility == null)
        {
            Debug.LogError("在ThunderLinkSpell中，没找到雷属性技能脚本！");
        }
        playerAnim.SetSpell(this,SkillType.ThunderFire);

    }

}
