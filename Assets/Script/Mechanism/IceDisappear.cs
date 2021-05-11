using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDisappear : MonoBehaviour
{
    private float curTime = 0f;
    private PoolManager poolManager;
    private void Start()
    {
        curTime = 0f;
    }
    public void SetPoolManger(PoolManager p)
    {
        poolManager = p;
    }
    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if(curTime >= IceAbility.MAX_ICE_EXIST_TIME)
        {
            if(poolManager != null)
            {
                poolManager.RemoveGameObject(IceAbility.IceFreezingZoneName, gameObject);
            }
            else
            {
                Debug.LogError("冻结的冰没有找到poolmanager，为空值");
            }
            curTime = 0f;
        }
    }
}
