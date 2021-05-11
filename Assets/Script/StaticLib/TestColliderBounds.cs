using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColliderBounds : MonoBehaviour
{
    public BoxCollider2D a;
    public BoxCollider2D b;
    public BoxCollider2D c;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            doit();
        }
    }
    private void doit()
    {
        ColliderBoundsCalculator.SetColliderSizeAndPositionByRect(c, ColliderBoundsCalculator.GetColliderIntersection(a, b));
    }
}
