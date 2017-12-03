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
    struct InterestPair { public CollectedResource resource; public float interest; };


    public float infightingChance = 0.0f;
    public float droppedInterest = 4.0f;
    public float captureRadius = 2.0f;
    public float meTime;
    public float aggression = 0.0f;
    public float baseSpeed = 1.0f;
    public float speedUnits = 3.0f;

    public ResourceCollector collector;
    private UnityEngine.AI.NavMeshAgent agent;
    public State state;
    public Transform target;
    public float detectionRadius = 100.0f;
    public float fSlowTime = 0.0f;
    public float fStunTime = 0.0f;
	// Use this for initialization
	void Start () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        collector = GetComponentInChildren<ResourceCollector>();
        meTime = Random.Range(1.0f, 2.0f);
        state = State.IDLE;
        collector.capacity = 1;

	}

    public void Slow(float fTime)
    {
        fSlowTime = fTime;
    }

    float GetBaseInterest(CollectedResource.OwnerType type)
    {
        switch(type)
        {
            case CollectedResource.OwnerType.Player:
                return 1.0f;
            case CollectedResource.OwnerType.Lair:
            case CollectedResource.OwnerType.Workbench:
                return 0.0f;
            case CollectedResource.OwnerType.Enemy:
                return infightingChance;
            default: // None - i.e. dropped
                return droppedInterest;
        }
    }
    // Update is called once per frame
    void Update() {
        fSlowTime -= Time.deltaTime;
        fStunTime -= Time.deltaTime;

        agent.speed = speedUnits * (baseSpeed + aggression) * (fSlowTime > 0.0f ? 0.5f : 1.0f) ;

        if (fStunTime > 0.0f)
            agent.speed = 0.0f;

        if (agent.isOnNavMesh)
        {
            // Check to make sure the target is still there
            if (target == null)
                UpdateTarget();
            if ((meTime -= Time.deltaTime) < 0)
            {
                if (collector.CanReceive())
                    UpdateTarget();
            }
            switch (state)
            {
                case State.FOLLOWING:
                    if ((target.transform.position-transform.position).magnitude < captureRadius)
                    {
                        target.GetComponent<CollectedResource>().SetOwner(collector);
                        state = State.RETURNING;
                    }
                    agent.destination = target.transform.position;
                    break;

            }
        }
    }

    void UpdateTarget()
    {
        float totalInterest = 0.0f;
        aggression = 0.0f;

        List<InterestPair> interestingResources = new List<InterestPair>();
        foreach (Collider collider in Physics.OverlapSphere(transform.position, detectionRadius, Physics.AllLayers, QueryTriggerInteraction.Collide))
        {
            InterestPair pair = new InterestPair();
            pair.resource = collider.GetComponent<CollectedResource>();
            if (pair.resource == null || pair.resource.owner == transform) // Don't get distracted by something you're carrying!
                continue;
            pair.interest = GetBaseInterest(pair.resource.ownerType);
            if (pair.interest == 0.0f)
            {
                continue;
            }
            aggression += 1.0f; // Aggression isn't affected by distance?
                                         // Be more interested in things closer to you
            float distance = (transform.position - pair.resource.transform.position).magnitude;
            if (distance > 1)
            {
                pair.interest /= distance;
            }
            totalInterest += pair.interest;

            interestingResources.Add(pair);
        }
        float specificInterest = Random.Range(0, totalInterest);
        foreach (InterestPair pair in interestingResources)
        {
            if ((specificInterest -= pair.interest) < 0)
            {
                target = pair.resource.transform;
                state = State.FOLLOWING;
                return;
            }
        }
        if (target == null)
        {
            agent.destination = transform.position;
            state = State.IDLE;
        }
    }
 
}
