using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Scandium, Red, Green, Blue }

public class CollectedResource : MonoBehaviour {
    public static float followDistance = 1.0F;
    public float speed = 8.0F;
    public ResourceType type;

    // The object that this one is following
    public Transform leader;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 distance = leader.position - transform.position;
        Vector3 moveDirection = Vector3.zero;
		if (distance.magnitude > followDistance ) {
            // We have to catch up
            moveDirection = transform.TransformDirection(distance) * speed;
        }
        transform.Translate(moveDirection * Time.deltaTime);
    }
}
