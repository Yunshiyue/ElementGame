using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableElementChanger : Interactable
{
    private GameObject elementMenuObject;
    private ElementMenu elementMenuScript;
    private void Awake()
    {
        elementMenuObject = GameObject.Find("SwitchElementMenu");
        elementMenuScript = elementMenuObject.GetComponent<ElementMenu>();
        objectNameOnUI = "元素石";
    }
    public override void Interactive()
    {
        elementMenuObject.SetActive(true);
        elementMenuScript.RefreshItem();
    }
}
