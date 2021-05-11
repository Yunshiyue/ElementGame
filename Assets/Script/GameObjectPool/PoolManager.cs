/**
 * @Description: PoolManager类是对象池管理类，所有的对象池要在该类中注册，生成特定对象时要调用此类中的接口
 * @Author: CuteRed

 * 
 * 
 * @Description: 在对象池中添加了poolType-FireBall 火球 typeCount = 2
 * @Author:夜里猛

 * 

 * @Editor: CuteRed
 * @Edit: 新增了指定对象池的清空函数ClearPool
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    /// <summary>
    /// 存放所有对象池
    /// </summary>
    private Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();

    private Transform managerTransform;

    private void Awake()
    {
        managerTransform = GetComponent<Transform>();
        if (managerTransform == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到transform组件");
        }

        //根据预设创建所有对象池
        foreach (Transform poolType in managerTransform)
        {
            GameObjectPool gameObjectPool = new GameObjectPool(poolType.gameObject, poolType.gameObject.name, managerTransform);
            pools.Add(poolType.gameObject.name, gameObjectPool);
        }

        //初始化所有对象池
        foreach (GameObjectPool pool in pools.Values)
        {
            pool.Init();
        }

        pools[MeteorShower.METEORITE].MaxCount = MeteorShowerSpell.fireNum;
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="poolType">对象类型</param>
    /// <param name="position">对象生成位置</param>
    /// <returns>获取到的对象</returns>
    public GameObject GetGameObject(string poolType, Vector3 position)
    {
        //检查是否有该对象池
        if (pools.ContainsKey(poolType))
        {
            return pools[poolType].Get(position);
        }

        Debug.LogError("获取对象" + poolType + "错误");
        return null;
    }

    public GameObject GetGameObject(string poolType)
    {
        //检查是否有该对象池
        if (pools.ContainsKey(poolType))
        {
            return pools[poolType].Get();
        }

        Debug.LogError("获取对象" + poolType + "错误");
        return null;
    }


    /// <summary>
    /// 删除对象，将其放入对象池中
    /// </summary>
    /// <param name="poolType">对象池类型</param>
    /// <param name="obj">要删除的对象</param>
    public void RemoveGameObject(string poolType, GameObject obj)
    {
        //检查是否有该对象池
        if (pools.ContainsKey(poolType))
        {
            pools[poolType].Recycle(obj);
        }
    }

    /// <summary>
    /// 清空指定的对象池
    /// </summary>
    /// <param name="poolType"></param>
    public void ClearPool(string poolType)
    {
        //检查是否有该对象池
        if (pools.ContainsKey(poolType))
        {
            pools[poolType].ClearPool();
        }
    }

    /// <summary>
    /// 销毁所有对象池
    /// </summary>
    public void Destroy()
    {
        pools.Clear();
    }
}
