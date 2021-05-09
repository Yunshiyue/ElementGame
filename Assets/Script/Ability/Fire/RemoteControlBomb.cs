using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControlBomb : MonoBehaviour
{
    private GameObject thrower;
    private CanFight throwerHand;
    private Collider2D coll;
    private PoolManager poolManager;
    public static string REMOTE_CONTROL_BOMB_NAME = "RemoteControlBomb";
    private string[] targetsName = { "Player", "Enemy" };
    private string[] originName = { "Enemy" };
    private void Awake()
    {
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        //获得第二个collider，为爆炸范围
        coll = GetComponents<Collider2D>()[1];
        if(!coll.isTrigger)
        {
            Debug.LogError("炸弹coll没有rtigger");
        }
    }
    public void Bomb()
    {
        throwerHand.Initiailize(targetsName);
        throwerHand.AttackArea(coll, 2, AttackInterruptType.WEAK);
        throwerHand.Initiailize(originName);
        //播放爆炸动画
        poolManager.RemoveGameObject(REMOTE_CONTROL_BOMB_NAME, gameObject);
    }
    public void SetThrower(GameObject who)
    {
        thrower = who;
        throwerHand = who.GetComponent<CanFight>();
    }
}
