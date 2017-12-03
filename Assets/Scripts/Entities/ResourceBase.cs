using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceBase : MonoBehaviour, IInteractable
{
    public MeshRenderer ring;
    public Material red, green;

    public enum ResourceType
    {
        SCANDIUM,
        GEMS,
        FUEL,
        BEAMS
    }

    public ResourceType type;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hover(bool bInRange)
    {
        if (ring == null) return;
        ring.enabled = true;
        ring.material = bInRange ? green : red;
    }

    public void Unhover()
    {
        if (ring == null) return;
        ring.enabled = false;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public virtual void Interact(PlayerBehaviour player, int mouseButton)
    {
        if (mouseButton == 0)
        {
            player.collector.AddResource(this);
        }
    }
}
