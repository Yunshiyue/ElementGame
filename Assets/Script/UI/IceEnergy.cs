using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEnergy : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform energyTransform;
    private bool iceUse;
    private bool iceRecover;
    void Start()
    {
        energyTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        iceUse = Input.GetKeyDown("u");
        iceRecover = Input.GetKeyDown("i");
        if (iceUse)
        {
            energyTransform.anchoredPosition = new Vector2(energyTransform.anchoredPosition.x - 10f, 0);
        }
        if(iceRecover)
        {
            energyTransform.anchoredPosition = new Vector2(energyTransform.anchoredPosition.x +50f, 0);
        }
    }
}
