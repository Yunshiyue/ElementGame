using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGourndPic : MonoBehaviour
{
    private Transform player;
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }
    private void Update()
    {
        this.transform.position = player.transform.position;
    }
}
