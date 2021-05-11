using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAutoFetch : Fetch
{
    protected GameObject player;
    private bool isFetched = false;
    public float speed;

    private float justBirthTime = 0f;
    private float cannotBeFetchUntil = 0.5f;
    private void Reset()
    {
        speed = 10f;
    }
    protected virtual void Awake()
    {
        player = GameObject.Find("Player");
    }
    public override void FetchObject(GameObject who)
    {
        player = who;
        isFetched = true;
    }

    private bool isJustBroned = true;
    private Rigidbody2D rb;
    // Update is called once per frame
    void Update()
    {
        if(justBirthTime <= cannotBeFetchUntil)
        {
            justBirthTime += Time.deltaTime;
            return;
        }

        if(isFetched)
        {
            if(isJustBroned)
            {
                if (TryGetComponent<Rigidbody2D>(out rb))
                {
                    rb.isKinematic = true;
                    rb.velocity = Vector2.zero;
                    isJustBroned = false;
                }
            }
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            if(Vector2.Distance(transform.position, player.transform.position) < 0.7f)
            {
                //通知主角捡到了东西！
                GetFetched();
                Destroy(gameObject);
            }
        }
    }
    protected virtual void GetFetched()
    {

    }
}
