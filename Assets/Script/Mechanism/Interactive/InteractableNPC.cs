using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : Interactable
{
    public override void Interactive()
    {
        //加载对话系统，并显示对话框
    }

    void Awake()
    {
        objectNameOnUI = gameObject.name;
    }
}
