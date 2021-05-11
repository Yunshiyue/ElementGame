using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealMeteorShower : FlyingAbility
{
    public const string HEALMETEORITE = "HealMeteorShower";

    [Header("运动参数")]
    private Vector3 direction;

    [Header("消失检测参数")]
    private bool isTouchPlayer = false;

    private LayerMask playerLayer;
    private DefencePlayer defencePlayer;

    private bool isFlyingLeft = true;
    private float onceFlyingTime = 0f;
    private float curFlyingTime = -1f;

    public override void Initialize()
    {
        base.Initialize();

        maxExistTime = 5f;
        speed = 5f;
        playerLayer = LayerMask.NameToLayer("Player");
        defencePlayer = GameObject.Find("Player").GetComponent<DefencePlayer>();
    }

    public override void MyUpdate()
    {
        //更新时间
        TimeUpdate();

        //消失检测
        if (DisappearDetect())
        {
            Disappear();
        }
        //运动
        else
        {
            Movement();
        }
    }

    /// <summary>
    /// 爆炸并消失
    /// </summary>
    protected override void Disappear()
    {
        //消失
        poolManager.RemoveGameObject(HEALMETEORITE, gameObject);
        //参数还原
        Clear();
    }

    /// <summary>
    /// 消失检测，超时、触碰敌人、触碰地图
    /// </summary>
    /// <returns></returns>
    protected override bool DisappearDetect()
    {
        return isTimeOut || isTouchPlayer;
    }
     
    /// <summary>
    /// 移动，指定方向
    /// </summary>
    protected override void Movement()
    {
        curFlyingTime += Time.deltaTime;
        if(curFlyingTime >= onceFlyingTime)
        {
            if(Random.value < 0.5)
            {
                isFlyingLeft = true;
                direction = new Vector2(- Random.value, -1);
            }
            else
            {
                isFlyingLeft = false;
                direction = new Vector2(Random.value, -1);
            }
            curFlyingTime = 0;
            onceFlyingTime = Random.value * 3 + 1;
        }
        //向指定方向移动
        gameObject.transform.position += direction.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            defencePlayer.Heal(1);
            isTouchPlayer = true;
        }
    }

    protected override void Clear()
    {
        base.Clear();

        isTimeOut = false;
        isTouchPlayer = false;
    }
}