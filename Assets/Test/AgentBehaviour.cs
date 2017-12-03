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
    private NavMeshAgent agent;
    private Rigidbody rigidBody;
    private State state;
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

    }

	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case State.ON_MESH:
                if (Input.GetMouseButtonDown(0))
                {

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {

                        agent.SetDestination(hit.point);

                    }
                }
                if (Input.GetAxis("Jump") > 0)
                {
                    state = State.OFF_MESH;
                    SetRigid(true);
                    rigidBody.AddForce(new Vector3(0, 20, 0), ForceMode.Impulse);
                }
                break;
        }

        

	}
}
