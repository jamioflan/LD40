using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcefield : MonoBehaviour {

	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            while( player.collector.collectedResources.Count > 0)
            {
                CollectedResource resource = player.collector.collectedResources[0];
                resource.SetOwner(Core.GetCore().theWorkbench.resourceReceiver);
                resource.ownerType = CollectedResource.OwnerType.Workbench;
                resource.followDistance = 0.0f;
            }
        }
    }
}
