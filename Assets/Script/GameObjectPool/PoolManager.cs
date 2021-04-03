/**
 * @Description: PoolManager类是对象池管理类，所有的对象池要在该类中注册，生成特定对象时要调用此类中的接口
 * @Author: CuteRed

 *      
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public enum poolType { Dart };
    private int typeCount = 1;

    /// <summary>
    /// 存放所有对象池
    /// </summary>
    Dictionary<PoolManager.poolType, GameObjectPool> pools = new Dictionary<PoolManager.poolType, GameObjectPool>();

    Transform parentTransform;

    public static PoolManager instance = null;

    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = GameObject.Find("PoolManager");

                if (gameObject == null)
                {
                    gameObject = new GameObject("PoolManager");
                    DontDestroyOnLoad(gameObject);
                }

                instance = gameObject.GetComponent<PoolManager>();

                if (instance == null)
                {
                    instance = gameObject.AddComponent<PoolManager>();
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        parentTransform = GetComponent<Transform>();

        //初始化对象池
        DartPool dartPool = new DartPool();
        dartPool.Init(PoolManager.poolType.Dart, transform);
        pools.Add(PoolManager.poolType.Dart, dartPool);
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="poolType">对象类型</param>
    /// <param name="position">对象生成位置</param>
    /// <returns>获取到的对象</returns>
    public GameObject GetGameObject(PoolManager.poolType poolType, Vector3 position)
    {
        //检查是否有该对象池
        if (pools.ContainsKey(poolType))
        {
            return pools[poolType].Get(position);
        }

        Debug.LogError("获取对象" + poolType + "错误");
        return null;
    }


    /// <summary>
    /// 删除对象，将其放入对象池中
    /// </summary>
    /// <param name="poolType">对象池类型</param>
    /// <param name="obj">要删除的对象</param>
    public void RemoveGameObject(PoolManager.poolType poolType, GameObject obj)
    {
        //检查是否有该对象池
        if (pools.ContainsKey(poolType))
        {
            pools[poolType].Remove(obj);
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
