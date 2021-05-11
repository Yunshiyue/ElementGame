using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Approaching : MonoBehaviour
{

    public float originSpeed = 2f;
    public float maxDistanceFromPlayer = 5;
    public Transform StartPosition;
    public Transform EndPosition;
    public float approachingSpeed;

    private Transform player;
    private Vector3 direction;

    private bool isEnded = false;

    // Update is called once per frame
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        approachingSpeed = originSpeed;
    }
    private void Start()
    {
        direction = EndPosition.position - StartPosition.position;
        direction.z = player.position.z;
        direction.Normalize();

        transform.position = StartPosition.position;
    }
    void Update()
    {
        if(Vector2.Distance(EndPosition.position, transform.position) < 0.1f)
        {
            enabled = false;
            return;
        }
        
        if(Vector3.Project(player.position - transform.position, direction).magnitude > maxDistanceFromPlayer)
        {
            approachingSpeed += 0.01f;
        }
        else
        {
            approachingSpeed = originSpeed;
        }
        transform.Translate(approachingSpeed * Time.deltaTime * direction,  Space.Self);

    }
}
