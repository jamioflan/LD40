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

    public IInteractable hoveringOver = null;

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

        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool bInRange = false;
        if (Physics.Raycast(ray, out hit) && hit.collider != null)
        {
            IInteractable newHover = hit.collider.GetComponentInParent<IInteractable>();
            if (newHover != null)
            {
                bInRange = (hit.collider.transform.position - transform.position).magnitude < interactDistance;

                if (hoveringOver != null && newHover != hoveringOver)
                {
                    hoveringOver.Unhover();
                }

                hoveringOver = newHover;
                hoveringOver.Hover(bInRange);
            }
            else
            {
                if (hoveringOver != null)
                {
                    hoveringOver.Unhover();
                    hoveringOver = null;
                }
            }
        }
        else
        {
            if (hoveringOver != null)
            {
                hoveringOver.Unhover();
                hoveringOver = null;
            }
        }

        if(hoveringOver != null)
        {
            if (bInRange)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    hoveringOver.Interact(this, 0);
                }
                if (Input.GetMouseButtonUp(1))
                {
                    hoveringOver.Interact(this, 1);
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    // Play a nastay SFX
                }
            }
        }
	}


    

    public void UnlockSkill(int iSkillIndex)
    {
        // TODO
    }
}
