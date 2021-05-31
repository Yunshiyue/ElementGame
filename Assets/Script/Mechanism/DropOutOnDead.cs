using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOutOnDead : MonoBehaviour
{
    private PoolManager poolManager;
    public string Thing1;
    public int Thing1Number;
    public string Thing2;
    public int Thing2Number;
    public string Thing3;
    public int Thing3Number;
    public string Thing4;
    public int Thing4Number;
    public string Thing5;
    public int Thing5Number;

    private void Awake()
    {
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
    }
    private void OnDisable()
    {
        DropOut(Thing1, Thing1Number);
        DropOut(Thing2, Thing2Number);
        DropOut(Thing3, Thing3Number);
        DropOut(Thing4, Thing4Number);
        DropOut(Thing5, Thing5Number);
    }

    private void DropOut(string thing, int number)
    {
        Debug.Log(this.name);
        if (thing != "" && number != 0)
        {
            Vector2 direction = new Vector2(0, 1);
            GameObject thing1;

            for (int i = 0; i < number; i++)
            {
                thing1 = poolManager.GetGameObject(thing);
                direction.x = Random.Range(-1, 1);
                thing1.SetActive(true);
                thing1.transform.position = this.transform.position;
                thing1.GetComponent<Rigidbody2D>().AddForce(direction.normalized * 5, ForceMode2D.Impulse);
            }
        }
    }

}
