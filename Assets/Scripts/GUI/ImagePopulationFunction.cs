using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ImagePplnFn : UnityEvent<Image>
{

}

public class ImagePopulationFunction : MonoBehaviour
{
    public ImagePplnFn populationFunction;

    // Use this for initialization
    void Start()
    {
		
	}
	
	// Update is called once per frame
	void Update()
    {
        Image image = gameObject.GetComponent<Image>();
        populationFunction.Invoke(image);
    }
}
