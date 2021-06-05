using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IceEnergy : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform energyTransform;
    private bool iceUse;
    private bool iceRecover;

    //Input System
    private Keyboard keyboard;
    void Awake()
    {
        energyTransform = GetComponent<RectTransform>();
        keyboard = InputSystem.GetDevice<Keyboard>();
        if (keyboard == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取keyboard失败");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //旧输入系统
        iceUse = Input.GetKeyDown("u");
        iceRecover = Input.GetKeyDown("i");
        //iceUse = keyboard.uKey.isPressed;
        //iceRecover = keyboard.iKey.isPressed;

        if (iceUse)
        {
            energyTransform.anchoredPosition = new Vector2(energyTransform.anchoredPosition.x - 10f, 0);
        }
        if(iceRecover)
        {
            energyTransform.anchoredPosition = new Vector2(energyTransform.anchoredPosition.x + 50f, 0);
        }
    }
}
