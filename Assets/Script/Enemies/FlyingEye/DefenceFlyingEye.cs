

using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class DefenceFlyingEye : Defence
{
   

    protected override void Awake()
    {
        base.Awake();
       
    }

    public override void AttackCheck()
    {
        SetStatistic();
        Damage();
    }
    
}
