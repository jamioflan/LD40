using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour, IInteractable
{

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
    public float accelerationUnits = 8.0f;
    public float angularSpeedUnits = 100;
    public Vector3 targetPosition;

    public ResourceCollector collector;
    public EnemyLair lair;
    private UnityEngine.AI.NavMeshAgent agent;
    public State state;
    public Transform target;
    public float detectionRadius = 100.0f;
    public float fSlowTime = 0.0f;
    public float fStunTime = 0.0f;
	// Use this for initialization
	void Start () {
        collector = GetComponentInChildren<ResourceCollector>();
        meTime = Random.Range(1.0f, 2.0f);
        state = State.IDLE;
        collector.capacity = 1;

	}

    public void Slow(float fTime)
    {
        fSlowTime = fTime;
    }

    public void Stun(float fTime)
    {
        fStunTime = fTime;
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

        if (Time.time < 0.2f) {
            return;
        }
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.speed = speedUnits * (baseSpeed + aggression) * (fSlowTime > 0.0f ? 0.5f : 1.0f);
        agent.acceleration = accelerationUnits * (1 + aggression);
        agent.angularSpeed = angularSpeedUnits * (1 + aggression);

        fSlowTime -= Time.deltaTime;
        fStunTime -= Time.deltaTime;


        if (fStunTime > 0.0f)
            agent.speed = 0.0f;


        if (agent.isOnNavMesh)
        {
            NavMeshPath path = agent.path;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }
            // Check to make sure the target is still there
            if (state == State.FOLLOWING && target == null)
                UpdateTarget();
            if ((meTime -= Time.deltaTime) < 0)
            {
                meTime = Random.Range(1.0f, 2.0f);
                if (collector.CanReceive())
                    UpdateTarget();
            }
            switch (state)
            {
                case State.FOLLOWING:
                    if ((target.transform.position - transform.position).magnitude < captureRadius)
                    {
                        target.GetComponent<CollectedResource>().SetOwner(collector);
                        state = State.RETURNING;
                        //agent.destination = lair.transform.position + new Vector3(5, 0, 5);
                        agent.destination = lair.holder.position;
                        targetPosition = agent.destination;
                    }
                    else
                    {
                        agent.destination = target.transform.position;
                        targetPosition = agent.destination;
                    }
                    break;
                case State.RETURNING:
                    if ((agent.destination - transform.position).magnitude < 2.0f)
                    {
                        while (collector.collectedResources.Count > 0)
                        {
                            collector.collectedResources[collector.collectedResources.Count - 1].SetOwner(lair);
                        }
                    }
                    Debug.Log(lair.holder.position);
                    Debug.Log(agent.destination + " - " + transform.position + " (" + agent.nextPosition + ") " + agent.desiredVelocity);
                    state = State.IDLE;
                    break;
                case State.IDLE:
                    agent.destination = transform.position;
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
            if (collider == null)
                continue;
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

    public MeshRenderer ring;

    public void Hover(bool bInRange)
    {
        ring.enabled = true;
    }

    public void Unhover()
    {
        ring.enabled = false;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Interact(PlayerBehaviour player, int mouseButton)
    {
    }
}
