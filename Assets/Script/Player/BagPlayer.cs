using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagPlayer : MonoBehaviour
{
    private Text goldTextUI;
    private int glod = 0;
    private void Awake()
    {
        goldTextUI = GameObject.Find("Money Num").GetComponent<Text>();
    }
    private void Start()
    {
        SetGlod(0);
    }
    public int GetGlod()
    {
        return glod;
    }
    public void SetGlod(int newGold)
    {
        glod = newGold;
        goldTextUI.text = newGold.ToString();
    }
}
