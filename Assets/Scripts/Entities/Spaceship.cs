using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
	// Use this for initialization
	void Start()
    {
        for (int i = 0; i < m_xParts.Count; i++)
        {
            SpaceshipPart xPart = m_xParts[i];
            GameObject xPartOrbject = xPart.m_xGameObject;
            xPartOrbject.SetActive(xPart.m_bPurchased);
        }
    }
	
	// Update is called once per frame
	void Update()
    {

	}

    public void OnPartPurchased(int iPartIndex)
    {
        // Mark as purchased
        if (iPartIndex < m_xParts.Count)
        {
            SpaceshipPart xPart = m_xParts[iPartIndex];
            xPart.m_bPurchased = true;
            GameObject xPartOrbject = xPart.m_xGameObject;
            xPartOrbject.SetActive(true);
        }
    }

    [System.Serializable]
    public class SpaceshipPart
    {
        public bool m_bPurchased;
        public GameObject m_xGameObject;
    }

    public List<SpaceshipPart> m_xParts = new List<SpaceshipPart>();
}
