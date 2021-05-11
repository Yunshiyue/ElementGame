using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAutoFetchedMainElement : RemoteAutoFetchElement
{
    protected override void Awake()
    {
        base.Awake();
        element = player.GetComponent<ElementAbilityManager>().GetMainElement();
    }
    private void Start()
    {
        base.Awake();
        ColorMyself();
        //Debug.Log("main初始化结束！" + element);
    }
}
