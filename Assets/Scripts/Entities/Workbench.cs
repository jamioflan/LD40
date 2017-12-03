using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour {

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
}
