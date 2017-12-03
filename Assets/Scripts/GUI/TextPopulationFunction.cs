using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class TextPplnFn : UnityEvent<Text>
{

}

public class TextPopulationFunction : MonoBehaviour
{
    public TextPplnFn populationFunction;

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
        Text text = gameObject.GetComponent<Text>();
        populationFunction.Invoke(text);
    }
}
