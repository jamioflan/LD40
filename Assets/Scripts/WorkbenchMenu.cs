using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkbenchMenu : MonoBehaviour
{
    public bool ShowBuiltItems = false;

    // Use this for initialization
    void Start()
	{
        
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
        if( m_iCurrentItemIndex + 1 < m_xItems.Count)
        {
            int iIndexToTry = m_iCurrentItemIndex + 1;

            while (iIndexToTry < m_xItems.Count)
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
        if (m_iCurrentItemIndex < m_xItems.Count)
        {
            ListItem xItem = m_xItems[m_iCurrentItemIndex];
            xItem.m_bBuilt = true;

            // Make sure we still have a valid selection
            if (!ShowBuiltItems)
            {
                // Try to jump forwards to a new selection
                bool bFoundValidItem = false;

                if (m_iCurrentItemIndex + 1 < m_xItems.Count)
                {
                    int iIndexToTry = m_iCurrentItemIndex + 1;

                    while (iIndexToTry < m_xItems.Count)
                    {
                        ListItem xListItem = m_xItems[iIndexToTry];

                        if (xListItem.m_bBuilt)
                        {
                            iIndexToTry++;
                        }
                        else
                        {
                            m_iCurrentItemIndex = iIndexToTry;
                            bFoundValidItem = true;
                            break;
                        }
                    }
                }

                if( !bFoundValidItem )
                {
                    // Try to jump backwards instead
                    if (m_iCurrentItemIndex > 0)
                    {
                        int iIndexToTry = m_iCurrentItemIndex - 1;

                        while (iIndexToTry >= 0)
                        {
                            ListItem xListItem = m_xItems[iIndexToTry];

                            if (xListItem.m_bBuilt)
                            {
                                iIndexToTry--;
                            }
                            else
                            {
                                m_iCurrentItemIndex = iIndexToTry;
                                bFoundValidItem = true;
                                break;
                            }
                        }
                    }
                }

                if( !bFoundValidItem )
                {
                    // Just set the index to 0, but nothing should be displaying if we get here
                    m_iCurrentItemIndex = 0;
                }
            }

            // Pay and build
            if (Core.GetCore().theWorkbench.Pay(xItem.m_iGreenCost, xItem.m_iRedCost, xItem.m_iBlueCost))
            {
                if (xItem.m_bSpaceshipPart)
                {
                    Core.GetCore().theSpaceship.OnPartPurchased(xItem.m_iIndex);
                }
                else
                {
                    Core.GetCore().thePlayer.UnlockSkill(xItem.m_iIndex);
                }
            }
        }
	}

    public void BackOut()
    {
        Core.GetCore().theWorkbenchMenu.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    [System.Serializable]
    public struct ListItem
    {
        public string m_xDebugName;

        // Whether it's a spaceship part (if not it's a skill)
        public bool m_bSpaceshipPart;

        // The index of the spaceship part/skill
        public int m_iIndex;

        // Cost
        public int m_iGreenCost;
        public int m_iRedCost;
        public int m_iBlueCost;

        // Whether the player has built this item
        public bool m_bBuilt;
    }

    // List of all items that can be built
    public List<ListItem> m_xItems = new List<ListItem>();

    // Currently selected item
    int m_iCurrentItemIndex;
}
