using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotAutoFetchBElement : RemoteAutoFetchElement
{
    protected override void Awake()
    {
        base.Awake();
        element = player.GetComponent<ElementAbilityManager>().GetBElement();
    }
    private void Start()
    {
        ColorMyself();
        Debug.Log("B初始化结束！" + element);
    }
}
