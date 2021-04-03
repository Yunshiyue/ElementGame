using System.Collections;
/**
 * @Description: DartPool是GameObjectPool类的子类，是用于生成Dart的对象池，Init函数中加载了Dart预设
 * @Author: CuteRed

 *      
*/

using System.Collections.Generic;
using UnityEngine;

public class DartPool : GameObjectPool
{
    /// <summary>
    /// 初始化Dart对象池，加载预设
    /// </summary>
    /// <param name="poolType">对象池类型</param>
    /// <param name="transform">父物体transform</param>
    public override void Init(PoolManager.poolType poolType, Transform transform)
    {
        base.Init(poolType, transform);
        prefab = (GameObject)Resources.Load("Prefabs/Dart");
    }
}
