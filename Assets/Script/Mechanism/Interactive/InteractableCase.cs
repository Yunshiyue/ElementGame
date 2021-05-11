using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCase : Interactable
{
    private void Awake()
    {
        objectNameOnUI = "铁箱子";
    }
    public override void Interactive()
    {
        //播放箱子打开动画
        //TODO
        Destroy(gameObject);
    }
}
