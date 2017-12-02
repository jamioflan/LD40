using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollectedResource : ResourceBase {
    public static float followDistance = 1.0F;
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
            if (distance.magnitude > followDistance)
            {
                moveDirection = distance * speed;
            }
            transform.position += (moveDirection * Time.fixedDeltaTime);
        }
    }

    void SetLeader(Transform receiver)
    {
        Workbench bench = receiver.GetComponent<Workbench>();
        if (bench != null)
        {
            // If it's a workbench we destroy this and increment the workshop's count
            bench.ReceiveResource(type);
            Destroy(this);
        }
        else
        {
            leader = receiver;
        }
    }
}
