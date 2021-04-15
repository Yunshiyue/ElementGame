//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FireBallPool : GameObjectPool
//{
//    /// <summary>
//    /// 初始化FireBallPool对象池，加载预设
//    /// </summary>
//    /// <param name="poolType">对象池类型</param>
//    /// <param name="parentTransfrom">父物体transform</param>
//    public override void Init(PoolManager.poolType poolType, Transform parentTransfrom)
//    {
//        base.Init(poolType, parentTransfrom);

//        //新建GameObject并修改transform关系
//        GameObject FireBallPool = new GameObject("FireBallPool");
//        FireBallPool.transform.SetParent(parentTransfrom);
//        currentTransfrom = FireBallPool.transform;

//        prefab = (GameObject)Resources.Load("Prefabs/FireBall");
//    }

//}
