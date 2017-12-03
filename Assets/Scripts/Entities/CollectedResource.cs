﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollectedResource : ResourceBase
{
    public float followDistance = 1.0F;
    public float speed = 8.0F;

    // The object that this one is following
    public Transform leader;
    // The object that owns this one
    public Transform owner;
    
	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (leader != null)
        {

            Vector3 distance = leader.position - transform.position;
            Vector3 moveDirection = Vector3.zero;
            float fDistanceAway = distance.magnitude - followDistance;
            if (fDistanceAway > 0)
            {
                moveDirection = distance.normalized * Mathf.Min(speed, fDistanceAway / Time.fixedDeltaTime);
            }
            else if (leader == Core.GetCore().theWorkbench.resourceReceiver)
            {
                Debug.Log("HERE");
                SetOwner(Core.GetCore().theWorkbench);
            }
            transform.position += (moveDirection * Time.fixedDeltaTime);
        }
    }

    public void LeaveOwner()
    {
        if (owner != null)
        {
            ResourceCollector collector = owner.GetComponent<ResourceCollector>();
            if (collector != null)
            {
                collector.Yield(this);
            }
        }
    }

    public override void SetOwner(ResourceCollector collector)
    {
        if (owner == collector.transform)
        {
            return;
        }
        if (collector.AddResource(this) )
        {
            LeaveOwner();
        }
        owner = collector.transform;
    }

    public void SetOwner(Transform receiver)
    {
        LeaveOwner();
        owner = receiver;
        leader = receiver;
    }

    public void SetOwner(Workbench bench)
    {
        LeaveOwner();
        bench.ReceiveResource(type);
        Destroy(gameObject);
    }

    public void SetOwner()
    {
        LeaveOwner();
        owner = null;
        leader = null;
    }


    public override void Interact(PlayerBehaviour player, int mouseButton)
    {
        base.Interact(player, mouseButton);
        if (mouseButton == 1)
        {
            // Only discard if the player owns this
            if (owner == player.collector.transform)
            {
                SetOwner();
            }
        }
    }

}
