using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public int NumItems = 5; // Get this from somewhere?
    public bool ShowBuiltItems = false;

    // Use this for initialization
    void Start()
	{
        // Make sure NumItems is non-negative
        if( NumItems < 0 )
        {
            NumItems = 0;
            return;
        }

        // Fill the list
        m_xItems = new List<ListItem>( NumItems );
        for( uint u = 0; u < NumItems; u++ )
        {
            ListItem xItem;
            xItem.m_bBuilt = false; // No saved profile data, so initialise everything to false
            m_xItems.Add(xItem);
        }
	}
	
	// Update is called once per frame
	void Update()
	{
        // TODO: Populate all the text and images with the right stuff

        // First populate the quantities of the ores

        // Then, for the current item index, populate the image, the cost (or hide if can't
        // afford) and hide/show the right build button and increment/decrement buttons.

	}

	// OnClick functions
	public void Listbox_Decrement()
	{
        // Decrement the current item index, if possible
        if (m_iCurrentItemIndex > 0)
        {
            int iIndexToTry = m_iCurrentItemIndex - 1;

            while (iIndexToTry >= 0)
            {
                ListItem xListItem = m_xItems[iIndexToTry];

                // If we shouldn't show this item, try the next one
                if (!ShowBuiltItems && xListItem.m_bBuilt)
                {
                    iIndexToTry--;
                }
                else
                {
                    m_iCurrentItemIndex = iIndexToTry;
                    return;
                }
            }
        }
	}

	public void Listbox_Increment()
	{
        if( m_iCurrentItemIndex + 1 < NumItems )
        {
            int iIndexToTry = m_iCurrentItemIndex + 1;

            while (iIndexToTry < NumItems)
            {
                ListItem xListItem = m_xItems[iIndexToTry];

                if (!ShowBuiltItems && xListItem.m_bBuilt)
                {
                    iIndexToTry++;
                }
                else
                {
                    m_iCurrentItemIndex = iIndexToTry;
                    return;
                }
            }
        }
	}

	public void BuildItem()
	{
        if (m_iCurrentItemIndex < NumItems)
        {
            ListItem xItem = m_xItems[m_iCurrentItemIndex];
            xItem.m_bBuilt = true;

            // Do something else?
        }
	}

    struct ListItem
    {
        public bool m_bBuilt; // Whether the player has built this item
    }

    // List of all items that can be built
    List<ListItem> m_xItems;

    // Currently selected item
    int m_iCurrentItemIndex;
}
