using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {


    public float interactDistance = 4.0F;
    public float jumpSpeed = 8.0F;

    public ResourceCollector collector;

	public float speed = 1.0F;

    public float currentYSpeed = 0.0F;

    public Transform body, head;
    public float fBodyAngle = 0.0f, fHeadAngle = 0.0f;
    public Transform followPoint;

    // Use this for initialization
    void Start () {
        collector = GetComponentInChildren<ResourceCollector>();
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
                        collector.AddResource(hit.collider.GetComponentInParent<ResourceBase>());
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
                        collector.Yield(hit.collider.GetComponent<CollectedResource>() );

                    }
                }
            }
        }
	}


    

    public void UnlockSkill(int iSkillIndex)
    {
        // TODO
    }
}
