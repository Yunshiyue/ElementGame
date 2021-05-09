using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAutoFetchedAElement : RemoteAutoFetchElement
{
    protected override void Awake()
    {
        base.Awake();
        element = player.GetComponent<ElementAbilityManager>().GetAElement();
    }
    private void Start()
    {
        base.Awake();
        ColorMyself();
        Debug.Log("A初始化结束！" + element);
    }
}
