using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    private static Core theCore;

    public WorkbenchMenu theWorkbenchMenu;
    public Workbench theWorkbench;
    public Spaceship theSpaceship;
    public PlayerBehaviour thePlayer;
    public WorldBuilder worldBuilder;

    public static Core GetCore()
    {
        return theCore;
    }

    private void Start()
    {
        if (theCore != null)
        {
            // There can be only one!
            Destroy(theCore.gameObject);
        }
        theCore = this;

        theWorkbenchMenu.Initialise();
        //theSpaceship.Initialise();
        worldBuilder.GenerateWorld();
    }
    
    private void Update()
    {

    }


}
