/**
 * @Description: 该类实现主角挨打后闪烁动画效果，该组件功能由三个变量控制：
 *               maxPlayerAlpha：闪烁期间最大不透明度
 *               minPlayerAlpha：闪烁期间最小不透明度
 *               alphaChangeOnceTime：一次闪烁的时间间隔
 * @Author: ridger

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnime : MonoBehaviour
{
    private float maxPlayerAlpha = 1;
    private float minPlayerAlpha = 0.4f;
    private float alphaChangeOnceTime = 0.3f;
    private float alphaSpeed;
    private float alphaChangeCurTime = 0;
    private bool isAttacked = false;

    private Color spriteColor;
    private Color originColor;

    private SpriteRenderer sprite;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        alphaSpeed = (maxPlayerAlpha - minPlayerAlpha) / alphaChangeOnceTime;
    }
    private void Update()
    {
        if(isAttacked)
        {
            alphaChangeCurTime += Time.deltaTime;
            if(alphaChangeCurTime >= alphaChangeOnceTime)
            {
                alphaChangeCurTime = 0f;
                spriteColor.a = maxPlayerAlpha;
            }
            else
            {
                //截断，不会超出，但是会无法回到初值
                spriteColor.a -= alphaSpeed * Time.deltaTime;
            }
            //Debug.Log(sprite.color.a);
            sprite.color = spriteColor;
        }
    }
    public void StartAnime()
    {
        isAttacked = true;
        spriteColor = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a);
        originColor = spriteColor;
    }
    public void EndAnime()
    {
        isAttacked = false;
        alphaChangeCurTime = 0;
        sprite.color = originColor;
    }
}
