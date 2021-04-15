//using System.Collections;
///**
// * @Description: DartPool是GameObjectPool类的子类，是用于生成Dart的对象池，Init函数中加载了Dart预设
// * @Author: CuteRed

// *      
//*/

//using System.Collections.Generic;
//using UnityEngine;

//public class DartPool : GameObjectPool
//{
//    /// <summary>
//    /// 初始化Dart对象池，加载预设
//    /// </summary>
//    /// <param name="poolType">对象池类型</param>
//    /// <param name="parentTransfrom">父物体transform</param>
//    public override void Init(PoolManager.poolType poolType, Transform parentTransfrom)
//    {
//        base.Init(poolType, parentTransfrom);

//        //新建GameObject并修改transform关系
//        GameObject dartPool = new GameObject("DartPool");
//        dartPool.transform.SetParent(parentTransfrom);
//        currentTransfrom = dartPool.transform;

//        prefab = (GameObject)Resources.Load("Prefabs/Dart");
//    }
//}
