using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {


    public float interactDistance = 4.0F;
    public float jumpSpeed = 8.0F;

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
	public float speed = 1.0F;

    public float currentYSpeed = 0.0F;

    public Transform body, head;
    public float fBodyAngle = 0.0f, fHeadAngle = 0.0f;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		CharacterController controller = GetComponent<CharacterController> ();
		moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0.0F, Input.GetAxis ("Vertical"));
        if (controller.isGrounded && Input.GetAxis("Jump") > 0)
        {
            currentYSpeed += jumpSpeed;            
        }
        if (!controller.isGrounded)
        {
            currentYSpeed += Physics.gravity.y * Time.fixedDeltaTime;
        }
        if (controller.isGrounded && currentYSpeed < 0)
        {
            currentYSpeed = 0.0F;
        }
        // move with speed
        moveDirection *= speed;
        moveDirection.y += currentYSpeed;

        if (moveDirection.magnitude > 1.0f)
        {
            float fAngle = Mathf.Rad2Deg * Mathf.Atan2(controller.velocity.z, controller.velocity.x);
            fBodyAngle = Mathf.LerpAngle(fBodyAngle, fAngle, 8.0f * Time.fixedDeltaTime);
            body.localEulerAngles = new Vector3(0.0f, 180.0f - fBodyAngle, 0.0f);
        }
         
        fHeadAngle = Mathf.LerpAngle(fHeadAngle, fBodyAngle, 4.0f * Time.fixedDeltaTime);
        head.localEulerAngles = new Vector3(0.0f, 180.0f - fHeadAngle, 0.0f);

        controller.Move (moveDirection * Time.fixedDeltaTime);
        if (Input.GetMouseButtonDown(0) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) )
            {
                if (hit.collider != null && (hit.collider.transform.position - transform.position).magnitude < interactDistance)
                {
                    
                    if ( hit.collider.GetComponent<ResourceBase>() != null)
                    {
                        if (AddResource(hit.collider.GetComponent<ResourceBase>() ) )
                        {
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    if ( hit.collider.GetComponent<Workbench>() != null)
                    {
                        // Open the work bench menu
                        Core.GetCore().theWorkbenchMenu.gameObject.SetActive(true);
                        Time.timeScale = 0;
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
