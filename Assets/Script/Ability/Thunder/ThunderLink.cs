/**
 * @Description: 闪电链物体在主角技能组件SetTarget后就会寻找敌人，在飞到敌人周围后对敌人造成伤害，
 *               并待机等待下一次Set敌人
 * @Author: ridger

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderLink : myUpdate
{
    private int priorityInType = 0;
    private UpdateType type = UpdateType.PoolThing;
    private GameObject nextTarget;
    private bool isReachedTarget = true;
    private float speed = 10f;

    private CanBeFighted targetFought;

    private CanFight canFight;

    public void SetTarget(GameObject target)
    {
        nextTarget = target;
        isReachedTarget = false;
    }
    public void SetStartPosition(Vector2 position)
    {
        this.transform.position = position;
    }
    public void SetCanFight(CanFight canFight)
    {
        this.canFight = canFight;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        nextTarget = null;
    }
    private void MoveToTarget()
    {
        transform.Translate(Vector2.ClampMagnitude(nextTarget.transform.position - transform.position, speed * Time.deltaTime), Space.Self);
        if(Vector2.Distance(nextTarget.transform.position, transform.position) < 0.05f)
        {
            //transform.SetParent(nextTarget.transform);
            if(nextTarget.TryGetComponent(out targetFought))
            {
                canFight.Attack(targetFought, 2);
            }
            else
            {
                Debug.LogError("闪电链攻击到的" + nextTarget.name + "中，没有CanBeFighted组件!");
            }
            isReachedTarget = true;
        }
    }

    public override void MyUpdate()
    {
        if(!isReachedTarget)
        {
            MoveToTarget();
        }
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return type;
    }
    public override void Initialize()
    {
    }
}
