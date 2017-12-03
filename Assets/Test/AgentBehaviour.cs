using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBehaviour : MonoBehaviour {

    private enum State
    {
        ON_MESH,
        OFF_MESH
    }
    public NavMeshAgent agent;
    private Rigidbody rigidBody;
    private State state;
    public Vector3 velocity;
    public Vector3 destination;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        state = State.ON_MESH;
	}
	
    void SetRigid(bool rigid)
    {
        rigidBody.isKinematic = !rigid;
        rigidBody.useGravity = rigid;
        agent.enabled = !rigid;

    }

	// Update is called once per frame
	void FixedUpdate () {
        switch (state)
        {
            case State.ON_MESH:
                velocity = agent.velocity;
                if (Input.GetMouseButtonDown(0))
                {

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {

                        agent.SetDestination(hit.point);

                    }
                }
                destination = agent.destination;
                if (Input.GetAxis("Jump") > 0)
                {
                    state = State.OFF_MESH;
                    SetRigid(true);
                    rigidBody.AddForce(new Vector3(0, 100000, 0) * Time.deltaTime);
                }
                break;
            case State.OFF_MESH:
                velocity = rigidBody.velocity;
                break;

        }

	}

    private void OnCollisionEnter(Collision collision)
    {
        SetRigid(false);
        state = State.ON_MESH;
        agent.SetDestination(destination);
    }

    void Jump()
    {
        Vector3 distance = (agent.destination - agent.nextPosition).normalized;
        Vector3 jump = agent.velocity;
        jump.x /= 5;
        jump.z /= 5;
        if (distance.y > 0)
        {
            jump.y = 20;
        }
        else
        {
            jump.y = 13;
            jump.x *= 2;
        }
        state = State.OFF_MESH;
        SetRigid(true);
        rigidBody.AddForce(jump, ForceMode.Impulse);
        /*
        Debug.Log(agent.steeringTarget +", " + agent.nextPosition);
        Debug.Log(agent.path.corners.Length);
        
        foreach(Vector3 corner in agent.path.corners)
        {
            Debug.Log(corner);
        }
        Vector3 distance = (agent.steeringTarget - agent.nextPosition);
        state = State.OFF_MESH;
        SetRigid(true);
        
        float yDistance = distance.y;
        distance *= 2;

        if (yDistance > 0)
        {
            distance += new Vector3(0, yDistance * 8, 0);
        }
        rigidBody.AddForce(distance * Time.deltaTime, ForceMode.Impulse);
        */
    }
}
