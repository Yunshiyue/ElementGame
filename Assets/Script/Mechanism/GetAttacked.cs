using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanBeFighted))]
public class GetAttacked : myUpdate
{
    private CanBeFighted canBeFighted;
    private PoolManager poolManager;

    private int priority = 0;
    private UpdateType type = UpdateType.Map;

    public bool MustDropOut;
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

    public bool RandomlyDropOut;
    public string Thing6;
    public string Thing7;
    public string Thing8;
    public string Thing9;
    public string Thing10;

    private string[] randomThingName = new string[5];
    private int randomThingNumber = 0;

    public override void Initialize()
    {
        canBeFighted = GetComponent<CanBeFighted>();
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();

        if (Thing6 != "")
        {
            randomThingName[randomThingNumber] = Thing6;
            randomThingNumber++;
        }
        if (Thing7 != "")
        {
            randomThingName[randomThingNumber] = Thing7;
            randomThingNumber++;
        }
        if (Thing8 != "")
        {
            randomThingName[randomThingNumber] = Thing8;
            randomThingNumber++;
        }
        if (Thing9 != "")
        {
            randomThingName[randomThingNumber] = Thing9;
            randomThingNumber++;
        }
        if (Thing10 != "")
        {
            randomThingName[randomThingNumber] = Thing10;
            randomThingNumber++;
        }
    }

    public override void MyUpdate()
    {
        if(canBeFighted.getAttackNum() > 0)
        {
            if (MustDropOut)
            {
                Debug.Log("丢出东西, canBeFighted.AttNum = " + canBeFighted.getAttackNum());

                DropOut(Thing1, Thing1Number);
                DropOut(Thing2, Thing2Number);
                DropOut(Thing3, Thing3Number);
                DropOut(Thing4, Thing4Number);
                DropOut(Thing5, Thing5Number);
            }
            if(RandomlyDropOut)
            {
                int randomChoice;
                for (int i = 0; i < canBeFighted.getAttackNum(); i++)
                {
                    randomChoice = Random.Range(0, randomThingNumber);
                    DropOut(randomThingName[randomChoice], 1);
                }
            }
        }
        canBeFighted.Clear();
    }
    private void DropOut(string thing, int number)
    {
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
                thing1.GetComponent<Rigidbody2D>().velocity = direction.normalized * 5;
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
}
