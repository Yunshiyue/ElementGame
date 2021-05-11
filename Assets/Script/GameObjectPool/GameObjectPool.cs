/**
 * @Description: GameObjectPool类是对象池的基类，该类要被具体的类继承，负责生成需要频繁生成和消失的对象
 * @Author: CuteRed

 *      
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    protected const int DEFAULT_MAX_COUNT = 10;

    /// <summary>
    /// 对象池
    /// </summary>
    protected Queue pool;

    /// <summary>
    /// 池中存放最大对象数量
    /// </summary>
    protected int maxCount;

    /// <summary>
    /// 对象的预设
    /// </summary>
    protected GameObject prefab;

    /// <summary>
    /// 对象池类型
    /// </summary>
    protected string poolType;

    /// <summary>
    /// 对象池的位置
    /// </summary>
    protected Transform parentTransform;
    protected Transform currentTransfrom;

    public GameObjectPool()
    {
        this.maxCount = DEFAULT_MAX_COUNT;
        pool = new Queue();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefab">预设</param>
    /// <param name="poolType">对象池类型</param>
    /// <param name="parentTransform">父transform</param>
    public GameObjectPool(GameObject prefab, string poolType, Transform parentTransform)
    {
        this.maxCount = DEFAULT_MAX_COUNT;
        pool = new Queue();

        this.poolType = poolType;
        this.parentTransform = parentTransform;

        this.prefab = prefab;
        //Debug.Log("对象池" + poolType + "成功初始化。" + " 最大存放：" + maxCount + " 当前存放" + pool.Count);
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="poolType">对象池类型</param>
    /// <param name="parentTransform">对象池位置</param>
    public virtual void Init()
    {
        //新建GameObject并修改transform关系
        GameObject poolObject = new GameObject(poolType + "Pool");
        poolObject.transform.SetParent(parentTransform);
        currentTransfrom = poolObject.transform;

        //设置预设的父transform
        prefab.transform.SetParent(currentTransfrom);
    }


    public GameObject Prefab
    {
        set
        {
            prefab = value;
        }
    }

    public int MaxCount
    {
        set
        {
            maxCount = value;
        }
    }

    /// <summary>
    /// 通过对象池，生成对象
    /// </summary>
    /// <param name="position">生成对象的位置</param>
    /// <returns>生成的对象</returns>
    public virtual GameObject Get(Vector3 position)
    {
        Debug.Log("对象池" + poolType + "Get。" + " 最大存放：" + maxCount + " 当前存放" + pool.Count);
        GameObject returnObj;

        //如果对象池中有对象，直接取出
        if (pool.Count > 0)
        {
            returnObj = (GameObject) pool.Dequeue();
        }
        //如果没有对象，生成一个
        else
        {
            //根据预设实例化对象
            returnObj = GameObject.Instantiate(prefab) as GameObject;
            returnObj.transform.SetParent(currentTransfrom);
            returnObj.SetActive(false);
        }

        //修改位置
        returnObj.transform.position = position;
        returnObj.SetActive(true);

        return returnObj;
    }

    /// <summary>
    /// 通过对象池，生成对象
    /// </summary>
    /// <param name="position">生成对象的位置</param>
    /// <returns>生成的对象</returns>
    public virtual GameObject Get()
    {
        //Debug.Log("对象池" + poolType + "Get。" + " 最大存放：" + maxCount + " 当前存放" + pool.Count);
        GameObject returnObj;

        //如果对象池中有对象，直接取出
        if (pool.Count > 0)
        {
            returnObj = (GameObject)pool.Dequeue();
        }
        //如果没有对象，生成一个
        else
        {
            //根据预设实例化对象
            returnObj = GameObject.Instantiate(prefab) as GameObject;
            returnObj.transform.SetParent(currentTransfrom);
        }
        returnObj.SetActive(true);

        return returnObj;
    }

    /// <summary>
    /// 删除对象，将其放入对象池
    /// </summary>
    /// <param name="obj">要删除的对象</param>
    public virtual void Recycle(GameObject obj)
    {
        //Debug.Log("对象池" + poolType + "Recycle。" + " 最大存放：" + maxCount + " 当前存放" + pool.Count);
        //如果对象已经在对象池中，则不能重复放入
        if (pool.Contains(obj))
        {
            Debug.LogError(obj.name + "已经在对象池中，不能重复remove");
            return;
        }

        //如果对象池中存放数量超过最大数量，直接销毁
        if (pool.Count > maxCount)
        {
            GameObject.Destroy(obj);
        }
        //否则，放入对象池
        else
        {
            pool.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// 销毁对象池
    /// </summary>
    public virtual void ClearPool()
    {
        foreach (GameObject gameObject in pool)
        {
            GameObject.Destroy(gameObject);
        }
        pool.Clear();
    }
}