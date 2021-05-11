/**
 * @Description: SmallIceCone为小冰锥，由冰锥管理器管理，掉落地面后消失
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallIceCone : Mechanism
{
    public const string SMALL_ICE_CONE = "SmallIceCone";

    private Collider2D coll;
    private CanFight canFight;
    private SpriteRenderer spriteRenderer;

    [Header("颜色参数")]
    private Color originalColor;
    private Color transparentColor;

    [Header("运动参数")]
    public float originalFlowSpeed = 3.0f;
    private float speed;
    private Vector3 direction = Vector3.down;
    private Vector3 startPosition;

    [Header("伤害参数")]
    public int damage = 2;
    private List<LayerMask> layers = new List<LayerMask>();

    [Header("时间参数")]
    public float generateTime = 2.0f;

    private LayerMask ground;


    protected override void Start()
    {
        base.Start();

        

        speed = originalFlowSpeed;

        startPosition = transform.position;

        ground = LayerMask.GetMask("Platform");

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到spriteRenderer");
        }

        //颜色初始化
        originalColor = spriteRenderer.color;
        transparentColor = spriteRenderer.color;
        transparentColor.a = 0.0f;

        //碰撞体初始化
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
        transform.position += direction * speed * Time.deltaTime;
        speed += 1.0f;

        //检测碰到地图
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, 0.2f, ground);
        if (hit && speed > 0.0f)
        {
            //消失
            spriteRenderer.color = transparentColor;

            //速度置为0
            speed = 0.0f;
            StartCoroutine(nameof(FlowToFloor));
        }
    }

    /// <summary>
    /// 设置生成位置
    /// </summary>
    /// <param name="startPosition">生成位置</param>
    public void SetPosition(Vector2 startPosition)
    {
        transform.position = startPosition;
    }

    public override void Trigger(TiggerType triggerType)
    {
        if (TriggerDetect())
        {
            //触发
            isTriggered = true;

            //恢复速度
            speed = originalFlowSpeed;

            //开启脚本
            enabled = true;
        }
    }

    protected override bool TriggerDetect()
    {
        return !isTriggered;
    }

    /// <summary>
    /// 落到地板上，被对象池收回
    /// </summary>
    private IEnumerator FlowToFloor()
    {

        yield return new WaitForSeconds(generateTime);

        //消失
        //gameObject.SetActive(false);

        //等待重新生成
        Generate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //层级检测
        if (speed > 0 && layers.Contains(collision.gameObject.layer))
        {
            //检测是否可以被攻击
            CanBeFighted beFought;
            if (collision.TryGetComponent<CanBeFighted>(out beFought))
            {
                canFight.Attack(beFought, damage);
                Debug.Log(gameObject.name + "攻击到了" + beFought.gameObject.name);
            }
        }
    }

    /// <summary>
    /// 重新生成
    /// </summary>
    /// <returns></returns>
    private void Generate()
    {
        enabled = false;

        transform.position = startPosition;

        //恢复颜色
        spriteRenderer.color = originalColor;
        //gameObject.SetActive(true);
        isTriggered = false;
    }
}
