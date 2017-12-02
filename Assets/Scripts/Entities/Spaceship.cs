using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public int numParts = 5;

	// Use this for initialization
	void Start()
    {
        
    }

    public void Initialise()
    {
        // Make sure NumItems is non-negative
        if (numParts < 0)
        {
            numParts = 0;
            return;
        }

        // Fill the list
        m_xParts = new List<SpaceshipPart>(numParts);
        for (uint u = 0; u < numParts; u++)
        {
            SpaceshipPart xPart;
            xPart.m_bPurchased = false; // No saved profile data, so initialise everything to false
            m_xParts.Add(xPart);
        }
    }
	
	// Update is called once per frame
	void Update()
    {
		// TODO: Display equipped spaceship parts? (Do this event-based ideally)
	}

    public void OnPartPurchased(int iPartIndex)
    {
        // Mark as purchased
        if( iPartIndex < numParts)
        {
            SpaceshipPart xPart = m_xParts[iPartIndex];
            xPart.m_bPurchased = true;
        }

        // TODO: Modify spaceship model
    }

    struct SpaceshipPart
    {
        public bool m_bPurchased;
    }

    List<SpaceshipPart> m_xParts;
}
