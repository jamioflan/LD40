using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public static Score instance;

    public int latestScore = 0;
    public int highscore = 0;

	void Start ()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

	void Update ()
    {
		
	}

    public static void SetScore(int iScore)
    {
        instance.latestScore = iScore;
        if(iScore > instance.highscore)
        {
            instance.highscore = iScore;
        }
    }
}
