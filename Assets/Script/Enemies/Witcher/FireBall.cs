using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : FlyingProp
{
    private float moveSpeed = 10f;

    private Vector2 targetPosition = new Vector2();

    private GameObject thrower;

    /// <summary>
    /// 被该火球攻击过的物体
    /// </summary>
    private List<CanBeFighted> fought = new List<CanBeFighted>();

    [Header("伤害参数")]
    private string targetLayerName;
    private int damage = 2;

    //protected override void Start()
    //{
    //    base.Start();

    //    if (thrower == null)
    //    {
    //        Debug.LogError("火球没有设置thrower！");
    //    }

    //    if (thrower == null)
    //    {
    //        Debug.LogError("在" + gameObject.name + "中，初始化thrower时出错");
    //    }
    //}

    public override void Initialize()
    {
        base.Initialize();

        if (thrower == null)
        {
            Debug.LogError("火球没有设置thrower！");
        }

        if (thrower == null)
        {
            Debug.LogError("在" + gameObject.name + "中，初始化thrower时出错");
        }
    }

    //private void OnEnable()
    //{
    //    existTime = 0f;
    //    fought.Clear();
    //}

    //调整角度
    public void SetAngle(float angle , int scale)
    {
        transform.localEulerAngles = new Vector3(0,0,angle);
        transform.localScale = new Vector3(scale, 1, 1);
    }
    /// <summary>
    /// 初始化火球，包括投掷者、目标位置，在每次生成火球时需要调用
    /// </summary>
    /// <param name="thrower">投掷者</param>
    public void SetThrower(GameObject thrower)
    {
        this.thrower = thrower;
    }

    public void SetTargetPosition(Vector2 tarPosition)
    {
        targetPosition.x = tarPosition.x;
        targetPosition.y = tarPosition.y;
    }
    /// <summary>
    /// 初始化火球，设置投掷者和目标位置
    /// </summary>
    /// <param name="thrower">投掷者</param>
    /// <param name="tarPosition">目标位置</param>
    public void Init(GameObject thrower, Vector2 tarPosition)
    {
        SetThrower(thrower);
        SetTargetPosition(tarPosition);

        existTime = 0f;
        fought.Clear();
    }


    public override void MyUpdate()
    {
        //超时检测
        Movement();
        //existTime += Time.deltaTime;
        TimeOutDetect();
    }
    protected override void Movement()
    {
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //撞到地图，直接消失
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            GameObject.Find("PoolManager").GetComponent<PoolManager>().RemoveGameObject("FireBall", gameObject);
        }

        //撞到人，造成伤害       
        CanBeFighted beFought;
        if (collision.TryGetComponent<CanBeFighted>(out beFought))
        {
            //不可对同一目标造成2次伤害
            if (!fought.Contains(beFought))
            {
                thrower.GetComponent<WitcherAttack>().GetComponent<CanFight>().Attack(beFought, damage);
                fought.Add(beFought);
                Debug.Log("Fireball对" + beFought.name + "造成伤害");
            }
        }
    }

    /// <summary>
    /// 超时检测，超过最大时间后，销毁
    /// </summary>
    protected override void TimeOutDetect()
    {
        if (transform.position.Equals(targetPosition))
        {
            GameObject.Find("PoolManager").GetComponent<PoolManager>().RemoveGameObject("FireBall", gameObject);
        }
    }

    protected override void Disappear()
    {
        GameObject.Find("PoolManager").GetComponent<PoolManager>().RemoveGameObject("FireBall", gameObject);
    }

    
}
