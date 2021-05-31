using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Camera follower;
    private GameObject player;
    private Vector3 cameraPosition;

    private void Start()
    {
        player = GameObject.Find("Player");
        cameraPosition.z = -20;
    }

    // Update is called once per frame
    void Update()
    {
        cameraPosition.x = player.transform.position.x;
        cameraPosition.y = player.transform.position.y;
        gameObject.transform.position = cameraPosition;
    }
}
