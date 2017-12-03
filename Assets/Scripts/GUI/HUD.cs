using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

	// Use this for initialization
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        // Draw an arrow to where the base is
        /*WorldTile baseTile = Core.OuEstLeBase(true);
        if (baseTile != null)
        {
            Vector3 worldPos = baseTile.transform.position + new Vector3(5.0f, 5.0f, 0.0f);
            Vector3 screenPos = Camera.main.WorldToViewportPoint(worldPos);

            if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1)
            {
                Debug.Log("already on screen, don't bother with the rest!");
                return;
            }

            onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2; //2D version, new mapping
            max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y)); //get largest offset
            onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.5f, 0.5f); //undo mapping
            Debug.Log(onScreenPos);
        }*/
    }

    public void PopulateCarryingCapacity(Text text)
    {
        int iCollectedResources = Core.GetCore().thePlayer.collector.collectedResources.Count;
        int iCapacity = Core.GetCore().thePlayer.collector.capacity;

        text.text = iCollectedResources + " / " + iCapacity;
    }
}