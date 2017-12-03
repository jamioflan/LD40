﻿using System.Collections;
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

    public bool bWin = false;
    public float fTimeSinceWin = 0.0f;
    public Camera datTastyCamera;

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
        if (!bWin)
        {
            Core.GetCore().CheckForWin();
        }

        if (bWin)
        {
            fTimeSinceWin += Time.deltaTime;

            datTastyCamera.transform.position = Vector3.Lerp(datTastyCamera.transform.position, theSpaceship.transform.position + new Vector3(0.0f, 10.0f, -10.0f), Time.deltaTime);

            if (fTimeSinceWin < 5.0f)
            {
                theSpaceship.transform.localPosition = Random.onUnitSphere * 0.5f;
            }
            else
            {
                theSpaceship.transform.localPosition = new Vector3(0.0f, fTimeSinceWin * fTimeSinceWin, 0.0f);
            }
        }
    }

    public static WorldTile OuEstLeBase(bool bSilVousPlait = false)
    {
        if(bSilVousPlait)
        {
            return theCore.worldBuilder.theBase;
        }

        return null;
    }

    public void CheckForWin()
    {
        foreach(WorkbenchMenu.ListItem item in theWorkbenchMenu.m_xItems)
        {
            if (item.m_bSpaceshipPart && !item.m_bBuilt)
                return;
        }

        bWin = true;
        fTimeSinceWin = 0.0f;

        datTastyCamera = thePlayer.GetComponentInChildren<Camera>();
        if (datTastyCamera != null)
        {
            datTastyCamera.transform.SetParent(null);
        }
        thePlayer.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);

        theSpaceship.pfx.Play();
    }
}
