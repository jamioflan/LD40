using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	public float baseSpeed = 6.0F;
    public float interactDistance = 4.0F;

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

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		CharacterController controller = GetComponent<CharacterController> ();
		moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
        if (!controller.isGrounded)
        {
            moveDirection += Physics.gravity;
        }
		moveDirection = transform.TransformDirection (moveDirection);
		moveDirection *= speed;
		controller.Move (moveDirection * Time.fixedDeltaTime);
        if (Input.GetMouseButtonDown(0) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) )
            {
                if (hit.collider != null)
                {
                    
                    if ( (hit.collider.transform.position - transform.position).magnitude < interactDistance && hit.collider.GetComponent<ResourceBase>() != null)
                    {
                        if (AddResource(hit.collider.GetComponent<ResourceBase>() ) )
                        {
                            Destroy(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {

                    if (hit.collider.GetComponent<CollectedResource>() != null)
                    {
                        Discard(hit.collider.GetComponent<CollectedResource>() );

                    }
                }
            }
        }
	}


    // Try and add a resource. Returns false if we're already at carrying capacity
    bool AddResource(ResourceBase resource)
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
        switch (resource.type)
        {
            case ResourceBase.ResourceType.Scandium:
                template = scandiumTemplate;
                break;
            case ResourceBase.ResourceType.Green:
                template = greenTemplate;
                break;
            case ResourceBase.ResourceType.Red:
                template = redTemplate;
                break;
            case ResourceBase.ResourceType.Blue:
                template = blueTemplate;
                break;
            default: // This is just to make VS happy...
                template = scandiumTemplate;
                break;
        }
        CollectedResource newResource = Instantiate<CollectedResource>(template, resource.transform.position, resource.transform.rotation);
        newResource.leader = last;
        collectedResources.Add(newResource);
        return true;
    }

    void Discard(CollectedResource resource)
    {
        int idx = collectedResources.IndexOf(resource);
        if (idx >= 0)
        {
            resource.leader = null;
            collectedResources.RemoveAt(idx);
            if (idx  < collectedResources.Count)
            {
                collectedResources[idx].leader = collectedResources[idx - 1].transform;
            }
        }
    }
}
