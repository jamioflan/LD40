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
    public CollectedResource gemTemplate;
    public CollectedResource fuelTemplate;
    public CollectedResource beamsTemplate;

	public List<CollectedResource> collectedResources;
	public int capacity = 5;

	public float speed = 1.0F;

    public float currentYSpeed = 0.0F;

    public Transform body, head;
    public float fBodyAngle = 0.0f, fHeadAngle = 0.0f;
    public Transform followPoint;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		CharacterController controller = GetComponent<CharacterController> ();
        Vector3 horizontalVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * speed;
        Vector3 velocity = controller.velocity;
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = 0;
            }
            velocity.y += Input.GetAxis("Jump") * jumpSpeed;
        }
        else
        {
            velocity += Physics.gravity * Time.fixedDeltaTime;

        }

        if (velocity.magnitude > 1.0f)
        {
            float fAngle = Mathf.Rad2Deg * Mathf.Atan2(velocity.z, velocity.x);
            fBodyAngle = Mathf.LerpAngle(fBodyAngle, fAngle, 8.0f * Time.fixedDeltaTime);
            body.localEulerAngles = new Vector3(0.0f, 180.0f - fBodyAngle, 0.0f);
        }
         
        fHeadAngle = Mathf.LerpAngle(fHeadAngle, fBodyAngle, 4.0f * Time.fixedDeltaTime);
        head.localEulerAngles = new Vector3(0.0f, 180.0f - fHeadAngle, 0.0f);

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;

        controller.Move(velocity * Time.fixedDeltaTime);

        if (Input.GetMouseButtonDown(0) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) )
            {
                if (hit.collider != null && (hit.collider.transform.position - transform.position).magnitude < interactDistance)
                {
                    
                    if ( hit.collider.GetComponentInParent<ResourceBase>() != null)
                    {
                        if (AddResource(hit.collider.GetComponentInParent<ResourceBase>() ) )
                        {
                            Destroy(hit.collider.GetComponentInParent<ResourceBase>().gameObject);
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
                        Yield(hit.collider.GetComponent<CollectedResource>() );

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
        Transform last = collectedResources.Count == 0 ? followPoint : collectedResources[collectedResources.Count - 1].transform;
        // Create a new resource behind the player
        // First work out what sort we should be making
        CollectedResource template;
        switch (resource.type)
        {
            case ResourceBase.ResourceType.SCANDIUM:
                template = scandiumTemplate;
                break;
            case ResourceBase.ResourceType.GEMS:
                template = gemTemplate;
                break;
            case ResourceBase.ResourceType.FUEL:
                template = fuelTemplate;
                break;
            case ResourceBase.ResourceType.BEAMS:
                template = beamsTemplate;
                break;
            default: // This is just to make VS happy...
                template = scandiumTemplate;
                break;
        }
        CollectedResource newResource = Instantiate<CollectedResource>(template, resource.transform.position, resource.transform.rotation);
        newResource.leader = last;
        if (collectedResources.Count == 0)
            newResource.followDistance = 0.0f;
        collectedResources.Add(newResource);
        return true;
    }

    void Yield(CollectedResource resource, Transform receiver = null)
    {
        int idx = collectedResources.IndexOf(resource);
        if (idx >= 0)
        {
            resource.SetLeader(receiver);
            collectedResources.RemoveAt(idx);
            if (idx  < collectedResources.Count)
            {
                collectedResources[idx].leader = collectedResources[idx - 1].transform;
            }
        }
    }

    public void UnlockSkill(int iSkillIndex)
    {
        // TODO
    }
}
