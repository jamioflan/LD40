using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour, IInteractable
{
    public int nGem = 0;
    public int nFuel = 0;
    public int nBeams = 0;
    public int nScOre = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool Pay(int costGem, int costFuel, int costBeams)
    {
        if (costGem < nGem || costFuel < nFuel || costBeams < nBeams)
        {
            return false;
        }
        else
        {
            nGem -= costGem;
            nFuel -= costFuel;
            nBeams -= costBeams;
            return true;
        }
    }

    public void ReceiveResource(ResourceBase.ResourceType type)
    {
        switch (type)
        {
            case ResourceBase.ResourceType.BEAMS:
                nBeams++;
                break;
            case ResourceBase.ResourceType.GEMS:
                nGem++;
                break;
            case ResourceBase.ResourceType.FUEL:
                nFuel++;
                break;
            case ResourceBase.ResourceType.SCANDIUM:
                nScOre++;
                break;
            default:
                break;
        }
    }

    public void Hover(bool bInRange)
    {
        //ring.enabled = true;
        //ring.material = bInRange ? green : red;
    }

    public void Unhover()
    {
        //ring.enabled = false;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Interact(PlayerBehaviour player, int mouseButton)
    {
        if (mouseButton == 0)
        {
            // Open the work bench menu
            Core.GetCore().theWorkbenchMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
