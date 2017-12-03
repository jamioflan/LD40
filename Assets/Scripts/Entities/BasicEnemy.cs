using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

    public enum State
    {
        IDLE,
        FOLLOWING,
        GUARDING,
        PATROLLING,
        RETURNING
    }

    public float meTime;
    private UnityEngine.AI.NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        meTime = Random.Range(1.0f, 2.0f);

	}
	
	// Update is called once per frame
	void Update () {
        if ((meTime -= Time.deltaTime) < 0)
        {
            //nearbyOre = DetectOre();
        }
        agent.destination = Core.GetCore().thePlayer.transform.position;
	}
}
