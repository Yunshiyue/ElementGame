using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAutoFetchedGlod : RemoteAutoFetch
{
    protected override void GetFetched()
    {
        BagPlayer bag = player.GetComponent<BagPlayer>();
        bag.SetGlod(bag.GetGlod() + 1);
    }
}
