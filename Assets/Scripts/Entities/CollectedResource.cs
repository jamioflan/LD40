﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollectedResource : ResourceBase
{
    public float followDistance = 1.0F;
    public float speed = 8.0F;

    // The object that this one is following
    public Transform leader;
    
	// Use this for initialization
	void Start () {
		
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
            transform.position += (moveDirection * Time.fixedDeltaTime);
        }
    }

    public void SetLeader(Transform receiver)
    {
        if (receiver != null)
        {
            Workbench bench = receiver.GetComponent<Workbench>();
            if (bench != null)
            {
                // If it's a workbench we destroy this and increment the workshop's count
                bench.ReceiveResource(type);
                Destroy(this);
                return;
            }
        }

        leader = receiver;

    }

    public override void Interact(PlayerBehaviour player, int mouseButton)
    {
        base.Interact(player, mouseButton);
        if (mouseButton == 1)
        {
            player.collector.Yield(this);
        }
    }
}
