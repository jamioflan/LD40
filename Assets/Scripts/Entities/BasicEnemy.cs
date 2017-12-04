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
        GUARDING_MOVING,
        PATROLLING,
        PATROLLING_MOVING,
        RETURNING
    }
    struct InterestPair { public CollectedResource resource; public float interest; };

    public float patrolChance = 0.4f; // Chance to enter patrol mode from idle
    public float walkRange = 20.0f; // Distance to move in one block in either patrol or guard mode
    public float walkWait = 0.8f; // Time to wait between movements in either patrol or guard mode
    public float infightingChance = 0.0f;
    public float doNothingChance = 1.0f;
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
    public CollectedResource target;
    public float detectionRadius = 40.0f;
    public float fSlowTime = 0.0f;
    public float fStunTime = 0.0f;
    public float fWaitTime = 0.0f;
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
            case CollectedResource.OwnerType.Workbench:
                return 0.0f;
            case CollectedResource.OwnerType.Lair:
            case CollectedResource.OwnerType.Enemy:
                return infightingChance;
            default: // None - i.e. dropped
                return droppedInterest;
        }
    }

    bool IsTargetValid(CollectedResource target)
    {
        return (
            target != null &&
            target.owner != transform && 
            target.owner != lair.transform
            );
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

            switch (state)
            {
                case State.FOLLOWING:
                    if (!IsTargetValid(target))
                    {
                        state = State.IDLE;
                        target = null;
                    }
                    else if ((target.transform.position - transform.position).magnitude < captureRadius)
                    {
                        target.GetComponent<CollectedResource>().SetOwner(collector);
                        target = null;
                        state = State.RETURNING;
                        agent.destination = lair.holder.position;
                        targetPosition = agent.destination;
                    }
                    else if ((target.transform.position - transform.position).magnitude > detectionRadius * 1.2)
                    {
                        state = State.IDLE;
                        target = null;
                    }
                    else
                    {
                        agent.destination = target.transform.position;
                        targetPosition = agent.destination;
                    }
                    break;
                case State.RETURNING:
                    if (collector.collectedResources.Count == 0)
                    {
                        // We've lost our resource :( :(
                        aggression = 0.0f;
                        agent.ResetPath();
                        state = State.IDLE;
                    }
                    else if ((lair.holder.position - transform.position).magnitude < 2.0f)
                    {
                        while (collector.collectedResources.Count > 0)
                        {
                            collector.collectedResources[collector.collectedResources.Count - 1].SetOwner(lair);
                        }
                        aggression = 0.0f;
                        agent.ResetPath();
                        state = State.IDLE;
                    }
                    break;
                case State.IDLE:
                    if (Random.Range(0, 1) < patrolChance)
                    {
                        state = State.PATROLLING;
                    }
                    else
                    {
                        state = State.GUARDING;
                    }
                    break;
                case State.PATROLLING:
                    if (UpdateTarget())
                        break;
                    fWaitTime -= Time.deltaTime;
                    if (fWaitTime < 0)
                    {
                        agent.destination = GetRandomPointOnMesh(transform.position, walkRange);
                        state = State.PATROLLING_MOVING;
                    }
                    break;
                case State.PATROLLING_MOVING:
                    if (UpdateTarget())
                        break;
                    if ((agent.destination - transform.position).magnitude < 1.0f)
                    {
                        fWaitTime = walkWait;
                        state = State.PATROLLING;
                    }
                    break;
                case State.GUARDING:
                    if (UpdateTarget())
                        break;
                    fWaitTime -= Time.deltaTime;
                    if (fWaitTime < 0)
                    {
                        agent.destination = GetRandomPointOnMesh(lair.transform.position, walkRange);
                        state = State.GUARDING_MOVING;
                    }
                    break;
                case State.GUARDING_MOVING:
                    if (UpdateTarget())
                        break;
                    if ((agent.destination - transform.position).magnitude < 1.0f)
                    {
                        fWaitTime = walkWait;
                        state = State.GUARDING;
                    }
                    break;
            }
        }
    }

    Vector3 GetRandomPointOnMesh(Vector3 center, float radius)
    {
        int maxAttempts = 10;
        NavMeshHit hit;
        while (maxAttempts >= 0) {
            Vector3 random = Random.insideUnitSphere * radius;
            random.y = 0;
            if (NavMesh.SamplePosition(center + random, out hit, 3.0f, NavMesh.AllAreas) ) {
                return hit.position;
            }
            --maxAttempts;
        }
        return center;
    }

    bool UpdateTarget()
    {
        if ((meTime -= Time.deltaTime) > 0)
             return false;
   
        meTime = Random.Range(1.0f, 2.0f);
        float totalInterest = doNothingChance;
        aggression = 0.0f;

        List<InterestPair> interestingResources = new List<InterestPair>();
        foreach (Collider collider in Physics.OverlapSphere(transform.position, detectionRadius, Physics.AllLayers, QueryTriggerInteraction.Collide))
        {
            if (collider == null)
                continue;
            InterestPair pair = new InterestPair();
            pair.resource = collider.GetComponent<CollectedResource>();
            if (!IsTargetValid(pair.resource) )
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
                target = pair.resource;
                fWaitTime = 0;
                state = State.FOLLOWING;
                aggression += 2;
                return true;
            }
        }

        // We must have decided to do nothing


        return false;
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
