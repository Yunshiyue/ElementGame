using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPItem :  MonoBehaviour
{
    private Image img;
    public Sprite lostingImg;
    public Sprite havingImg;

    //状态
    public bool isGetting =false;//生命上限是否获得
    public bool isHaving =true;//当前的心是否存在
    public bool isChanging =false;


    //private int priorityInType = 0;
    //private UpdateType updateType = UpdateType.GUI;

    //public override void Initialize()
    //{
    //    if (!isGetting)
    //    {
    //        transform.gameObject.SetActive(false);
    //    }
    //    img = GetComponent<Image>();
    //}
    private void Awake()
    {
        if (!isGetting)
        {
            transform.gameObject.SetActive(false);
        }

        img = GetComponent<Image>();
    }

    //public  void Update()
    //{
    //    if (isChanging)
    //    {
           
    //        if (isHaving == false)
    //        {
    //            Debug.Log("改变");
    //            //img.overrideSprite = lostingImg;
    //        }
                
    //    }
    //}

    //public override int GetPriorityInType()
    //{
    //    return priorityInType;
    //}

    //public override UpdateType GetUpdateType()
    //{
    //    return updateType;
    //}
    /// <summary>
    /// 获得生命上限
    /// </summary>
    public void Getting()
    {
        isGetting = true;
        transform.gameObject.SetActive(true);
    }
    /// <summary>
    /// 丢失生命值
    /// </summary>
    public void Lost()
    {
        Debug.Log("心心丢了");
        isChanging = true;
        isHaving = false;
        img.overrideSprite = lostingImg;
    }

    /// <summary>
    /// 回复生命值
    /// </summary>
    public void Recover()
    {
        isHaving = true;
        img.overrideSprite = havingImg;
    }

    //public override void MyUpdate()
    //{
    //    throw new System.NotImplementedException();
    //}
}
