/**
 * @Description: LargeIceCone为大冰锥类，掉落会变成地形，碰到敌人或玩家会造成伤害
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeIceCone : Mechanism
{
    private Collider2D coll;
    private CanFight canFight;

    [Header("运动参数")]
    public float flowSpeed = 3.0f;
    private Vector3 direction = Vector3.down;

    [Header("伤害参数")]
    public int damage = 2;
    private List<LayerMask> layers = new List<LayerMask>();

    private LayerMask ground;

    protected override void Start()
    {
        base.Start();

        ground = LayerMask.GetMask("Platform");

        coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到collider");
        }

        //初始化目标层级
        layers.Add(LayerMask.NameToLayer("Player"));
        layers.Add(LayerMask.NameToLayer("Enemy"));

        //初始化canFight
        canFight = GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到canFight");
        }
        string[] targetLayer = new string[2];
        targetLayer[0] = "Player";
        targetLayer[1] = "Enemy";
        canFight.Initiailize(targetLayer);

        enabled = false;
    }

    private void Update()
    {
        //下落
        transform.position += direction * flowSpeed * Time.deltaTime;
        flowSpeed += 1.0f;

        //检测碰到地图
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, 1.0f, ground);
        if (hit)
        {
            StartCoroutine(nameof(FlowToFloor));
            flowSpeed = 0.0f;
        }
    }

    public override void Trigger(TiggerType triggerType)
    {
        if (TriggerDetect())
        {
            //触发
            isTriggered = true;

            //开启脚本
            enabled = true;
        }
    }

    protected override bool TriggerDetect()
    {
        return !isTriggered;
    }

    /// <summary>
    /// 落到地板上
    /// </summary>
    private IEnumerator FlowToFloor()
    {
        yield return new WaitForSeconds(2.0f);

        gameObject.layer = LayerMask.NameToLayer("Platform");
        coll.isTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //层级检测
        if (flowSpeed > 0 && layers.Contains(collision.gameObject.layer))
        {
            //检测是否可以被攻击
            CanBeFighted beFought;
            if (collision.TryGetComponent<CanBeFighted>(out beFought))
            {
                canFight.Attack(beFought, damage);
                beFought.BeatBack(transform, 1.0f, 10.0f);
                Debug.Log(gameObject.name + "攻击到了" + beFought.gameObject.name);
            }
        }
    }
}
