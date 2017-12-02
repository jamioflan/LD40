using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour {

    public int nGreen = 0;
    public int nRed = 0;
    public int nBlue = 0;
    public int nScOre = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool Pay(int costGreen, int costRed, int costBlue)
    {
        if (costGreen < nGreen || costRed < nRed || costBlue < nBlue)
        {
            return false;
        }
        else
        {
            nGreen -= costGreen;
            nRed -= costRed;
            nBlue -= costBlue;
            return true;
        }
    }

    public void ReceiveResource(ResourceBase.ResourceType type)
    {
        switch (type)
        {
            case ResourceBase.ResourceType.Blue:
                nBlue++;
                break;
            case ResourceBase.ResourceType.Green:
                nGreen++;
                break;
            case ResourceBase.ResourceType.Red:
                nRed++;
                break;
            case ResourceBase.ResourceType.Scandium:
                nScOre++;
                break;
            default:
                break;
        }
    }
}
