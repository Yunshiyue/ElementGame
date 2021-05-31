using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;


public class ColliderBoundsCalculator
{
    public static Rect GetColliderRect(BoxCollider2D boxCollider)
    {
        return new Rect(boxCollider.bounds.min, boxCollider.bounds.size);
    }
    //public static Rectangle GetColliderRectangleInSys(BoxCollider2D boxCollider)
    //{
    //    Debug.Log(string.Format("左下坐标:({0}, {1}), w\\h:({2}, {3})", (int)boxCollider.bounds.min.x, (int)boxCollider.bounds.min.y, (int)boxCollider.bounds.size.x, (int)boxCollider.bounds.size.y));
    //    return new Rectangle((int)(boxCollider.bounds.min.x + 0.5f), (int)(boxCollider.bounds.min.y + 0.5f), (int)(boxCollider.bounds.size.x + 0.5f), (int)(boxCollider.bounds.size.y + 0.5f));
        
    //}
    public static Rect GetColliderIntersection(BoxCollider2D boxCollider1, BoxCollider2D boxCollider2)
    {
        //Rectangle rect1 = GetColliderRectangleInSys(boxCollider1);
        //Rectangle rect2 = GetColliderRectangleInSys(boxCollider2);
        //Rectangle result = Rectangle.Intersect(rect1, rect2);
        //Debug.Log(string.Format("求交结果：左下坐标:({0}, {1}), w\\h:({2}, {3})", result.Location.X, result.Location.Y, result.Width, result.Height));
        //return new Rect(result.Location.X, result.Location.Y, result.Width, result.Height * 0.87f);
        return new Rect(0, 0, 0, 0);
    }
    public static void SetColliderSizeAndPositionByRect(BoxCollider2D boxCollider, Rect rect)
    {
        boxCollider.transform.localScale = rect.size;
        boxCollider.transform.position = rect.position + rect.size / 2;
    }
}
