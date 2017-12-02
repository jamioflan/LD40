using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    private static Core theCore;

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
    }
    
    private void Update()
    {

    }


}
