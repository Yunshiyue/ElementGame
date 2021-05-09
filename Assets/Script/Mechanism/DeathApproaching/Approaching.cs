using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Approaching : MonoBehaviour
{
    private float originSpeed = 2f;
    public float approachingSpeed;
    public float maxDistanceFromPlayer = 5;
    private Transform player;
    // Update is called once per frame
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        approachingSpeed = originSpeed;
    }
    void Update()
    {
        if(player.position.x - transform.position.x > maxDistanceFromPlayer)
        {
            approachingSpeed += 0.004f;
        }
        else
        {
            approachingSpeed = originSpeed;
        }
        transform.Translate(approachingSpeed * Time.deltaTime, 0, 0, Space.Self);

    }
}
