using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    private static Core theCore;

    public WorkbenchMenu theWorkbenchMenu;
    public HUD theHUD;
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

        //theSpaceship.Initialise();
        worldBuilder.GenerateWorld();
    }
    
    private void Update()
    {

    }

    public static WorldTile OuEstLeBase(bool bSilVousPlait = false)
    {
        if(bSilVousPlait)
        {
            return theCore.worldBuilder.theBase;
        }

        return null;
    }
}
