using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameResults : MonoBehaviour
{
    List<string> names = new List<string>();
    List<int> scores = new List<int>();

    private ScoreDisplay scoreDisplay = null;

    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    public void AddScore(string name, int score)
    {
        names.Add(name);
        scores.Add(score);
    }

    void Update()
    {
        if (scoreDisplay == null)
        {
            GameObject canvas = GameObject.Find("ScoreCanvas");
            scoreDisplay = canvas.GetComponent<ScoreDisplay>();
        }
        else
        {
            for (int i = 0; i < names.Count; i++)
            {
                ScoreDisplay.Score score = new ScoreDisplay.Score(names[i], scores[i], i);
                scoreDisplay.AddScore(score);
            }

            scoreDisplay.DisplayScores();

            Destroy(gameObject);
        }
    }
}