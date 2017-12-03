using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : MonoBehaviour {

    /**********************
     * Resource Templates *
     *********************/
    public CollectedResource scandiumTemplate;
    public CollectedResource gemTemplate;
    public CollectedResource fuelTemplate;
    public CollectedResource beamsTemplate;

    public List<CollectedResource> collectedResources;
    public int capacity = 5;
    public bool CanReceive()
    {
        return collectedResources.Count < capacity;
    }
    // Try and add a resource. Returns false if we're already at carrying capacity
    public bool AddResource(CollectedResource resource)
    {
        if (!CanReceive())
        {
            return false;
        }

        // Find the last object in the list (or the player if it's empty)
        Transform last = collectedResources.Count == 0 ? transform : collectedResources[collectedResources.Count - 1].transform;

        resource.leader = last;
        resource.followDistance = collectedResources.Count == 0 ? 0.0f : 1.0f;

        collectedResources.Add(resource);
        return true;
    }

    public void Yield(CollectedResource resource, Transform receiver = null)
    {
        int idx = collectedResources.IndexOf(resource);
        if (idx >= 0)
        {
            collectedResources.RemoveAt(idx);

            if (idx < collectedResources.Count)
            {
                if (idx == 0)
                {
                    collectedResources[0].leader = transform;
                    collectedResources[0].followDistance = 0.0f;
                }
                else
                {
                    collectedResources[idx].leader = collectedResources[idx - 1].transform;
                    collectedResources[idx].followDistance = 1.0f;
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
