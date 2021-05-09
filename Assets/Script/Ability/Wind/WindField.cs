using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindField : myUpdate
{
    private int priority = 0;
    private UpdateType type = UpdateType.PoolThing;

    private float force = 5f;
    private bool isDrag = true;
    private List<Rigidbody2D> thingList = new List<Rigidbody2D>(32);
    private Rigidbody2D target;
    private float strechCurTime = 0f;
    private float strechOnceTime = 0.7f;

    public void SetDragOrNot(bool mode)
    {
        isDrag = mode;
    }
    public bool GetIsDragging()
    {
        return isDrag;
    }
    public override void Initialize()
    {
    }

    public override void MyUpdate()
    {
        strechCurTime += Time.deltaTime;
        if(strechCurTime >= strechOnceTime)
        {
            strechCurTime = 0;
            foreach(Rigidbody2D r in thingList)
            {
                Debug.Log(r.name + "正在被吸引！");
                r.AddForce(isDrag ? (transform.position - r.transform.position).normalized * force : (transform.position - r.transform.position).normalized * - force, ForceMode2D.Impulse);
            }
        }
    }

    public override int GetPriorityInType()
    {
        return priority;
    }

    public override UpdateType GetUpdateType()
    {
        return type;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name + "进入范围！");
        if (collision.TryGetComponent<Rigidbody2D>(out target) && collision.name != "Player")
        {
            Debug.Log(collision.name + "被加入吸引队列！");
            thingList.Add(target);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Rigidbody2D>(out target) && collision.name != "Player")
        {
            Debug.Log(collision.name + "逃出去了！");
            thingList.Remove(target);
        }
    }

}
