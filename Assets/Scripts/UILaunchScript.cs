using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UILaunchScript : MonoBehaviour
{
    public Text[] PlayerNames;
    public InputField MaxScore;
    public SetupInfo Info;

    // Use this for initialization
    void Start()
    {
        GameObject.DontDestroyOnLoad(Info.gameObject);
        MaxScore.text = "3";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Exit") > .1)
        {
            Exit();
        }
        if (Input.GetAxisRaw("Submit") > .1)
        {
            Submit();
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

    /// <summary>
    /// Validates input and loads new level
    /// </summary>
    public void Submit()
    {
        Info.PlayerNames = new List<string>();
        foreach (Text t in PlayerNames)
        {
            string name = t.text.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                Info.PlayerNames.Add(name);
            }
        }
        if (Info.PlayerNames.Count < 2)
        {
            return;
        }

        Info.MaxScore = int.Parse(MaxScore.text);

        Application.LoadLevel("battle");
    }

    /// <summary>
    /// Clamps the max score value between 1 and 10
    /// </summary>
    public void ClampScore()
    {
        MaxScore.text = ((int)Mathf.Clamp(float.Parse(MaxScore.text), 1f, 10f)).ToString();
    }
}