using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

    private UnityEngine.AI.NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

	}
	
	// Update is called once per frame
	void Update () {
        if (agent.destination == null)
        {
            agent.destination = Core.GetCore().thePlayer.transform.position;
        }
		
	}
}
