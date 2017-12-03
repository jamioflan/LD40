using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDeposit : ResourceBase
{
    public static List<ResourceDeposit> deposits = new List<ResourceDeposit>();
    public CollectedResource template;

    public override void SetOwner(ResourceCollector collector)
    {
        if (collector.CanReceive() )
        {
            CollectedResource resource = Instantiate<CollectedResource>(template, transform.position, transform.rotation);
            Destroy(gameObject);
            resource.SetOwner(collector);
        }
    }


    // Use this for initialization
    protected override void Start () {
        base.Start();
        deposits.Add(this);
	}

    public void OnDestroy()
    {
        deposits.Remove(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
