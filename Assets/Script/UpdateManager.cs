using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private void Start()
    {
        foreach (KeyValuePair<int, myUpdate> a in GUIUpdates)
        {
            a.Value.Initialize();
        }
        foreach (KeyValuePair<int, myUpdate> a in MapUpdates)
        {
            a.Value.Initialize();
        }
        foreach (KeyValuePair<int, myUpdate> a in PlayerUpdates)
        {
            a.Value.Initialize();
        }
        foreach (KeyValuePair<int, myUpdate> a in EnemyUpdates)
        {
            a.Value.Initialize();
        }
        print();
    }

    private void Update()
    {
        foreach (KeyValuePair<int, myUpdate> a in GUIUpdates)
        {
            a.Value.MyUpdate();
        }
        foreach (KeyValuePair<int, myUpdate> a in MapUpdates)
        {
            a.Value.MyUpdate();
        }
        foreach (KeyValuePair<int, myUpdate> a in PlayerUpdates)
        {
            a.Value.MyUpdate();
        }
        foreach (KeyValuePair<int, myUpdate> a in EnemyUpdates)
        {
            a.Value.MyUpdate();
        }
    }

    public bool Register(myUpdate update)
    {
        switch(update.GetUpdateType())
        {
            case myUpdate.UpdateType.GUI:
                GUIUpdates.Add(update.GetPriorityInType(), update);
                return true;
            case myUpdate.UpdateType.Map:
                MapUpdates.Add(update.GetPriorityInType(), update);
                return true;
            case myUpdate.UpdateType.Player:
                PlayerUpdates.Add(update.GetPriorityInType(), update);
                return true;
            case myUpdate.UpdateType.Enemy:
                EnemyUpdates.Add(update.GetPriorityInType(), update);
                return true;
        }
        return false;
    }

    private static UpdateManager me = new UpdateManager();
    private SortedList<int, myUpdate> GUIUpdates = new SortedList<int, myUpdate>();
    private SortedList<int, myUpdate> MapUpdates = new SortedList<int, myUpdate>();
    private SortedList<int, myUpdate> PlayerUpdates = new SortedList<int, myUpdate>();
    private SortedList<int, myUpdate> EnemyUpdates = new SortedList<int, myUpdate>();

    public void print()
    {
        foreach (KeyValuePair<int, myUpdate> a in GUIUpdates)
        {
            Debug.Log("在updatemanager的GUI队列里，存在" + a.Value.name);
        }
        foreach (KeyValuePair<int, myUpdate> a in MapUpdates)
        {
            Debug.Log("在updatemanager的Map队列里，存在" + a.Value.name);
        }
        foreach (KeyValuePair<int, myUpdate> a in PlayerUpdates)
        {
            Debug.Log("在updatemanager的Player队列里，存在" + a.Value.name);
        }
        foreach (KeyValuePair<int, myUpdate> a in EnemyUpdates)
        {
            Debug.Log("在updatemanager的Enemy队列里，存在" + a.Value.name);
        }
    }
}
