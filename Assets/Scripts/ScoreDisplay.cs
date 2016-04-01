using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    public class Score : IComparable
    {
        public string PlayerName;
        public int PlayerScore;
        public int PlayerNumber;

        public Score(string name, int score, int player)
        {
            PlayerName = name;
            PlayerScore = score;
            PlayerNumber = player;
        }

        /// <summary>
        /// Sorts largest first
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Score otherScore = obj as Score;
            if (otherScore != null)
                return otherScore.PlayerScore - PlayerScore;
            else
                throw new ArgumentException("Object is not a Score");
        }
    }

    public Text Winner;
    public Text[] Names;
    public Text[] Scores;

    public GameObject[] TankPrefabs;
    public float RotationSpeed = 45f;

    private List<Score> scores = new List<Score>();

    public void AddScore(Score score)
    {
        scores.Add(score);
    }

    public void DisplayScores()
    {
        scores.Sort();

        for (int i = 0; i < scores.Count; i++)
        {
            Names[i].text = scores[i].PlayerName;
            Scores[i].text = scores[i].PlayerScore.ToString();
        }

        Winner.text = "Winner: " + scores[0].PlayerName + "!";
        GameObject tank = GameObject.Instantiate<GameObject>(TankPrefabs[scores[0].PlayerNumber]);
        StartCoroutine(Rotation(tank));
    }

    private IEnumerator Rotation(GameObject tank)
    {
        while (true)
        {
            tank.transform.Rotate(new Vector3(0, Time.deltaTime * RotationSpeed, 0));
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetAxisRaw("Exit") > .1)
        {
            Exit();
        }

        if (Input.GetAxisRaw("Submit") > .1)
        {
            Application.LoadLevel("start");
        }
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}