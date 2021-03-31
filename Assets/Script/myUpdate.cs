using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class myUpdate : MonoBehaviour
{
    //private void Awake()
    //{
    //    UpdateManager.GetInstance().Register(this);
    //}
    abstract public void Initialize();

    abstract public void MyUpdate();

    public enum UpdateType { GUI, Map, Player, Enemy }

    abstract public UpdateType GetUpdateType();

    abstract public int GetPriorityInType();

}
