﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Image arrow;

	// Use this for initialization
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        // Draw an arrow to where the base is
        WorldTile baseTile = Core.OuEstLeBase(true);
        if (baseTile != null)
        {
            Vector3 deltaPos = baseTile.transform.position + new Vector3(5.0f, 0.0f, 5.0f) - Core.GetCore().thePlayer.transform.position;

            arrow.enabled = deltaPos.magnitude >= 10.0f;
            float fAngle = Mathf.Atan2(deltaPos.z, deltaPos.x) * Mathf.Rad2Deg;
            arrow.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, fAngle - 90.0f);
        }
    }

    public void PopulateCarryingCapacity(Text text)
    {
        int iCollectedResources = Core.GetCore().thePlayer.collector.collectedResources.Count;
        int iCapacity = Core.GetCore().thePlayer.collector.capacity;

        text.text = iCollectedResources + " / " + iCapacity;
    }
}