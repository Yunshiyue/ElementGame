using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEvent : MonoBehaviour
{
    private AttackPlayer attackComponent;

    private void Awake()
    {
        attackComponent = GameObject.Find("Player").GetComponent<AttackPlayer>();
        if (attackComponent == null)
        {
            Debug.LogError("在"+gameObject.name+"中，没有找到AttackPlayer脚本！");
        }
    }
    public void EventFinsh()
    {
        gameObject.GetComponent<Animator>().SetBool("effect", false);
    }
    public void ReadyFinsh()
    {
        gameObject.GetComponent<Animator>().SetBool("ready", false);
        gameObject.GetComponent<Animator>().SetBool("effect", true);
    }

    public void EventStart()
    {
        switch (gameObject.name)
        {
            case "ThunderFire":
                Debug.Log("ThunderFireAttack！");
                attackComponent.ThunderFireEvent();
                break;
            case "WindShortMain":
                Debug.Log("WindShortMainAttack！");
                attackComponent.WindShortMainEvent();
                break;
            case "ThunderLongMain":
                Debug.Log("ThunderLongMainAttack！");
                attackComponent.ThunderLongMainAttackEvent();
                break;
        }
    }
}
