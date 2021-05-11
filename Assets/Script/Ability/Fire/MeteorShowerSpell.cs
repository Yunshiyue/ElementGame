using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShowerSpell : Spell
{
    private float yScreenRatio = 0.9f;
    private Vector3 ballWorldPosition;
    private Vector3 ballScreenPosition;
    private int firstFlastFireNumber = 10;
    private int firstFlastIceNumber = 2;
    private float secondPerFire = 0.1f;
    private float curTime = 0f;
    static public int fireNum = 80;
    private int curFireNum = 0;
    private float fireNumberPerIce = 13;
    private Vector2 fireDirection = new Vector2(0, -1);

    private Camera mainCamera;

    private Vector2 temp = new Vector2(0, 0);
    private GameObject ball;

    private bool isOn = false;

    private PoolManager poolManager;

    public override void Initialize()
    {
        base.Initialize();
        playerAnim.SetSpell(this, SkillType.MeteorShower);
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        ballScreenPosition = new Vector3(0, yScreenRatio, 1);
        fireNum = 80;
    }

    public override void Cast()
    {
        Debug.Log("火冰风Cast");
        //播放聚焦动画
        playerAnim.SetUseSkillType(SkillType.MeteorShower);
    }

    public override void ReleaseSpell()
    {
        isOn = true;
        for(int i = 0; i < firstFlastFireNumber; i++)
        {
            ball = poolManager.GetGameObject(MeteorShower.METEORITE);
            BallInitialize(ball);
        }
        ball = poolManager.GetGameObject(HealMeteorShower.HEALMETEORITE);
        BallInitialize(ball);
    }

    public void MeteorShowerClock()
    {
        if(isOn)
        {
            curTime += Time.deltaTime;
            if(curTime >= secondPerFire)
            {
                curTime = 0;
                for(int i = 0; i < 2; i ++)
                {
                    ball = poolManager.GetGameObject(MeteorShower.METEORITE);
                    BallInitialize(ball);
                    curFireNum++;
                    Debug.Log("生成火流星:" + curFireNum);
                    if (curFireNum % fireNumberPerIce == 0)
                    {
                        ball = poolManager.GetGameObject(HealMeteorShower.HEALMETEORITE);
                        BallInitialize(ball);
                        Debug.Log("生成冰流星, curFireNum = " + curFireNum);
                    }
                    if (curFireNum >= fireNum)
                    {
                        isOn = false;
                        curTime = 0;
                        curFireNum = 0;
                    }
                }
            }
        }
    }
    private void BallInitialize(GameObject ball)
    {
        //计算得到屏幕上的一点的世界坐标，并赋给新生成的物体
        ballScreenPosition.x = Random.value;
        ballWorldPosition = mainCamera.ViewportToWorldPoint(ballScreenPosition);
        temp.x = ballWorldPosition.x;
        temp.y = ballWorldPosition.y;

        MeteorShower meteorShower;
        if (ball.TryGetComponent<MeteorShower>(out meteorShower))
        {
            meteorShower.SetStartPosition(temp);
            meteorShower.SetThrower(player);
            meteorShower.SetDamage(1);
            fireDirection.x = Random.value * 2 - 1;
            Debug.Log("随机方向" + fireDirection);
            meteorShower.SetDirection(fireDirection);

            return;
        }

        HealMeteorShower healMeteorShower;
        if (ball.TryGetComponent<HealMeteorShower>(out healMeteorShower))
        {
            healMeteorShower.SetStartPosition(temp);
            healMeteorShower.SetThrower(player);

            return;
        }
    }

    public override void Enable()
    {
    }

    public override void Disable()
    {
        poolManager.ClearPool(MeteorShower.METEORITE);
        poolManager.ClearPool(HealMeteorShower.HEALMETEORITE);
    }
}
