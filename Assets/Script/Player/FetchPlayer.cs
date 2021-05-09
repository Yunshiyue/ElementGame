using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchPlayer : MonoBehaviour
{
    protected GameObject player;
    protected void Awake()
    {
        player = GameObject.Find("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fetch fetchAble;
        if(collision.TryGetComponent<Fetch>(out fetchAble))
        {
            Debug.Log(fetchAble.name + "被捡到了！");
            fetchAble.FetchObject(player);
        }
    }
}
