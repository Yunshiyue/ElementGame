using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  Enemies : myUpdate
{

    protected DefenceEnemies defenceComponent;//防御组件 包含hpmax hp isdead 且自带canbeFight
    
                                             
    //update顺序
    protected UpdateType updateType = UpdateType.Enemy;
    //子类需修改优先级
    protected int priorityInType = 0;

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }

   
}
