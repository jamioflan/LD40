using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Text highscore, latestScore;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        highscore.text = "Highscore: " + Score.instance.highscore;
        latestScore.text = "Latest: " + Score.instance.latestScore;
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
