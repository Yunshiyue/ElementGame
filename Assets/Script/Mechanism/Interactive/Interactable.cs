using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
abstract public class Interactable : MonoBehaviour
{
    //务必记得在子类中赋值
    public string objectNameOnUI;
    public abstract void Interactive();
    public string GetObjectNameOnUI() { return objectNameOnUI; }
}
