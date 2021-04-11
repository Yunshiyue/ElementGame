using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderFire : MonoBehaviour
{
    public void ThunderFireFinsh()
    {
        gameObject.GetComponent<Animator>().SetBool("effect", false);
    }
}
