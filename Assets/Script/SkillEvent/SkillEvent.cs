using UnityEngine;

public class SkillEvent : MonoBehaviour
{
    private Animator playerAnim;
    private void Awake()
    {
        playerAnim = GameObject.Find("Player").GetComponent<Animator>();
    }
    public void EventFinsh()
    {
        //gameObject.GetComponent<Animator>().SetBool("effect", false);
        gameObject.SetActive(false);
        playerAnim.SetInteger("useSkillType", 0);
    }
    public void EventFinshAndDestory()
    {
        gameObject.GetComponent<Animator>().SetBool("effect", false);
        Destroy(gameObject);
        playerAnim.SetInteger("useSkillType", 0);
    }

    public void ReadyFinsh()
    {
        //gameObject.GetComponent<Animator>().SetBool("ready", false);
        gameObject.GetComponent<Animator>().SetBool("effect", true);
    }
    //public void ReadyFinshAndEventStart()
    //{
    //    gameObject.GetComponent<Animator>().SetBool("effect", true);
    //    //暂只有冰盾使用该方法
    //    attackComponent.IceShieldAttackEvent();
    //}

    //public void EventStart()
    //{
    //    switch (gameObject.name)
    //    {
    //        case "FireThunder":
    //            Debug.Log("FireThunderAttack！");
    //            attackComponent.FireThunderEvent();
    //            break;
    //        case "WindShortMain":
    //            Debug.Log("WindShortMainAttack！");
    //            attackComponent.WindShortMainEvent();
    //            break;
    //        case "ThunderLongMain":
    //            Debug.Log("ThunderLongMainAttack！");
    //            attackComponent.ThunderLongMainAttackEvent();
    //            break;
    //        case "RightIceThunder":
    //            Debug.Log("IceThunderAttack！");
    //            attackComponent.IceThunderEvent();
    //            break;
    //        case "IceShield":
    //            Debug.Log("IceShieldAttack！");
    //            attackComponent.IceShieldAttackEvent();
    //            break;
    //    }
    //}
}

