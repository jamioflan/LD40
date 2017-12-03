using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public GameObject[] walls = new GameObject[Directions.iNUM_DIRECTIONS];
    public MeshRenderer floorMesh;
    public Workbench myWorkbench;
    public Spaceship mySpaceship;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
