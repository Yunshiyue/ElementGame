using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : myUpdate
{
    protected LayerMask playerLayer;

    protected DefenceEnemies defenceComponent;//防御组件 包含hpmax hp isdead 且自带canbeFight
    //暂且只有防御组件一致？
    //protected AttackEnemies attackComponet;//攻击组件 包含canfight
    protected Animator enemyAnim;//动画

    protected UpdateType updateType = UpdateType.Enemy;
    protected int priorityInType = 0;

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public override void MyUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }

    
}
