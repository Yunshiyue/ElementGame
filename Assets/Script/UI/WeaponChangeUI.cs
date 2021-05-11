using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChangeUI : MonoBehaviour
{
    private Transform player;//对应的物体
    private Vector3 screenPos;
    private Image arrow;
    private Image sword;
    private Image hammer;
    private Image sheild;
    private Color originColor;
    private Color changedColor = new Color(236f / 255, 198f / 255, 22f / 255, 188f / 255);

    private Camera mainCamera;
    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    private void Start()
    {
        player = GameObject.Find("Player").transform;
        arrow = transform.GetChild(0).GetComponent<Image>();
        sword = transform.GetChild(1).GetComponent<Image>();
        hammer = transform.GetChild(2).GetComponent<Image>();
        sheild = transform.GetChild(3).GetComponent<Image>();
        originColor = arrow.color;
        //剑为默认武器 改变其颜色
        sword.color = changedColor;

    }
    //private void Update()
    //{
    //    //screenPos = mainCamera.WorldToScreenPoint();
    //    //transform.position = player.position;
    //}

    public void ChooseWeapon(IceAbility.IceWeapon type)
    {
        //颜色还原
        arrow.color = originColor;
        sword.color = originColor;
        hammer.color = originColor;
        sheild.color = originColor;

        switch (type)
        {
            case IceAbility.IceWeapon.Arrow:
                arrow.color = changedColor;
                break;
            case IceAbility.IceWeapon.Sword:
                sword.color = changedColor;
                break;
            case IceAbility.IceWeapon.Hammer:
                hammer.color = changedColor;
                break;
            case IceAbility.IceWeapon.Shield:
                sheild.color = changedColor;
                break;
        }
    }


}
