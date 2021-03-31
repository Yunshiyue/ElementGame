using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPItem : MonoBehaviour
{
    private Image img;
    public Sprite lostingImg;
    public Sprite havingImg;

    //状态
    public bool isGetting =false;
    public bool isHaving =true;

    // Start is called before the first frame update
    void Start()
    {
        if (!isGetting)
        {
            transform.gameObject.SetActive(false);
        }
        img =GetComponent<Image>();   
    }

    public void Getting()
    {
        isGetting = true;
        transform.gameObject.SetActive(true);
    }

    public void Lost()
    {
        isHaving = false;
        img.overrideSprite = lostingImg;
    }

    public void Recover()
    {
        isHaving = true;
        img.overrideSprite = havingImg;
    }

}
