using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanFight))]
[RequireComponent(typeof(WitcherMovement))]
public class WitcherAttack : MonoBehaviour
{
    CanFight canFight;
    WitcherMovement movementComponent;
    private string targetLayerName = "Player";
    GameObject target;
    //火球
    private Vector2 fireBallTargetPosition = new Vector2(0, 0);
    private int fireBallDistance = 10;

    //当前可使用技能情况
    private int blinkNum = 0;
    private bool canBlink = false;
    private bool canFire = true;
    private float fireCD;

    void Start()
    {
        canFight = GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
        }

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);

        movementComponent = GetComponent<WitcherMovement>();
        if (movementComponent == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到MovementPlayer组件！");
        }

        //得到player
        target = GameObject.Find("Player");
    }

    public void PushFire()
    {
        if (movementComponent.RequestChangeControlStatus(0.05f, WitcherMovement.EnemyStatus.AbilityNeedControl))
        {
            movementComponent.witcherAnim.SetBool("firing", true);
            //帧事件 FireBall
            Debug.Log("fire");
        }
    }
    public void FireBall()
    {
        GameObject fireBall = GameObject.Find("PoolManager").GetComponent<PoolManager>().GetGameObject("FireBall", gameObject.transform.position);

        //修改位置
        //fireBallTargetPosition.x = gameObject.transform.position.x + fireBallDistance * gameObject.transform.localScale.x;
        //fireBallTargetPosition.y = gameObject.transform.position.y;

        //fireBallTargetPosition = GameObject.Find("Player").transform.position;
        int scale = 1;
        if (target.transform.position.x < gameObject.transform.position.x)
        {
            scale = -1;
        }
        //发射点到目标的单位向量
        var dir = (target.transform.position - gameObject.transform.position).normalized;
        Debug.Log("向量：" + dir.ToString() +"角度：" + Vector2.Angle(target.transform.position, gameObject.transform.position));
        fireBallTargetPosition = dir * fireBallDistance;
        fireBall.GetComponent<FireBall>().Init(gameObject, fireBallTargetPosition);
        fireBall.GetComponent<FireBall>().SetAngle(Vector2.Angle(target.transform.position, gameObject.transform.position), scale);

        movementComponent.witcherAnim.SetBool("firing", false);
       
    }


}
