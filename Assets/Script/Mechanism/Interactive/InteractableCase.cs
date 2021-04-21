using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCase : Interactable
{
    private PoolManager poolManager;
    private int goldNumber = 3;

    private void Awake()
    {
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        objectNameOnUI = "铁箱子";
    }
    public override void Interactive()
    {
        for(int i = 0; i < goldNumber; i ++)
        {
            GameObject gold = poolManager.GetGameObject("Gold");
            Vector2 direction = new Vector2(0, 1);
            direction.x = Random.Range(-1, 1);
            gold.SetActive(true);
            gold.transform.position = this.transform.position;
            gold.GetComponent<Rigidbody2D>().AddForce(direction.normalized * 5, ForceMode2D.Impulse);
        }
        //播放箱子打开动画
        //TODO
        Destroy(gameObject);
    }
}
