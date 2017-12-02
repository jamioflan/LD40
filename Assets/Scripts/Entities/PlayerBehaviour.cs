using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	public float baseSpeed = 6.0F;

    /**********************
     * Resource Templates *
     *********************/
	public CollectedResource scandiumTemplate;
    public CollectedResource greenTemplate;
    public CollectedResource redTemplate;
    public CollectedResource blueTemplate;

	public List<CollectedResource> collectedResources;
	public uint capacity = 5;

	public Vector3 moveDirection = Vector3.zero;
	public float speed = 0.0F;

	// Use this for initialization
	void Start () {
        // For a test spawn in two resources
        AddResource(ResourceType.Scandium);
        AddResource(ResourceType.Green);
        AddResource(ResourceType.Red);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		CharacterController controller = GetComponent<CharacterController> ();
		moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		moveDirection = transform.TransformDirection (moveDirection);
		moveDirection *= speed;
		controller.Move (moveDirection * Time.fixedDeltaTime);
	}

    // Try and add a resource. Returns false if we're already at carrying capacity
    bool AddResource(ResourceType type)
    {
        if (collectedResources.Count == capacity)
        {
            return false; // Maybe trigger a message to the UI? TODO
        }
        // Find the last object in the list (or the player if it's empty)
        Transform last = collectedResources.Count == 0 ? transform : collectedResources[collectedResources.Count - 1].transform;
        // Create a new resource behind the player
        // First work out what sort we should be making
        CollectedResource template;
        switch (type)
        {
            case ResourceType.Scandium:
                template = scandiumTemplate;
                break;
            case ResourceType.Green:
                template = greenTemplate;
                break;
            case ResourceType.Red:
                template = redTemplate;
                break;
            case ResourceType.Blue:
                template = blueTemplate;
                break;
            default: // This is just to make VS happy...
                template = scandiumTemplate;
                break;
        }
        Debug.Log(type);
        CollectedResource newResource = Instantiate<CollectedResource>(template, last.position - last.forward * CollectedResource.followDistance, last.rotation);
        newResource.leader = last;
        newResource.type = type;
        collectedResources.Add(newResource);
        return true;
    }
}
