using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class displayCond : UnityEvent<Button>
{

}


public class ButtonDisplayCondition : MonoBehaviour
{
    public displayCond dispCond;

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
        Button button = gameObject.GetComponent<Button>();
        dispCond.Invoke(button);
    }
}
