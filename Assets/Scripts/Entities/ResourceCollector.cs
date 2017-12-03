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
    // Try and add a resource. Returns false if we're already at carrying capacity
    public bool AddResource(ResourceBase resource)
    {
        if (collectedResources.Count == capacity)
        {
            return false; // Maybe trigger a message to the UI? TODO
        }
        // Find the last object in the list (or the player if it's empty)
        Transform last = collectedResources.Count == 0 ? transform : collectedResources[collectedResources.Count - 1].transform;
        // Create a new resource behind the player
        // First work out what sort we should be making
        CollectedResource template;
        switch (resource.type)
        {
            case ResourceBase.ResourceType.SCANDIUM:
                template = scandiumTemplate;
                break;
            case ResourceBase.ResourceType.GEMS:
                template = gemTemplate;
                break;
            case ResourceBase.ResourceType.FUEL:
                template = fuelTemplate;
                break;
            case ResourceBase.ResourceType.BEAMS:
                template = beamsTemplate;
                break;
            default: // This is just to make VS happy...
                template = scandiumTemplate;
                break;
        }
        CollectedResource newResource = Instantiate<CollectedResource>(template, resource.transform.position, resource.transform.rotation);
        Destroy(resource.gameObject);
        newResource.leader = last;
        if (collectedResources.Count == 0)
            newResource.followDistance = 0.0f;
        collectedResources.Add(newResource);
        return true;
    }

    public void Yield(CollectedResource resource, Transform receiver = null)
    {
        int idx = collectedResources.IndexOf(resource);
        if (idx >= 0)
        {
            resource.SetLeader(receiver);
            collectedResources.RemoveAt(idx);

            if (idx < collectedResources.Count)
            {
                collectedResources[idx].leader = idx == 0 ? transform : collectedResources[idx - 1].transform;
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
