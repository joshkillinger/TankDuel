using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public int NumberOfPlayers = 1;
    private GameObject[] players;
    public Transform[] playerSpawnPoints;
    private PlayerScript[] playerScripts;

    public GameObject ExplosionEffect;
    public GameObject GlobalMessageText;
    private Text globalMessageText;

    private int maxScore;
    public GameObject ResultsPrefab;

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (instance != null)
        {
            Object.Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        SetupInfo setupInfo = GameObject.FindGameObjectWithTag("SetupInfo").GetComponent<SetupInfo>();
        NumberOfPlayers = setupInfo.PlayerNames.Count;

        players = new GameObject[NumberOfPlayers];
        playerScripts = new PlayerScript[NumberOfPlayers];

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            players[i] = (GameObject)GameObject.Instantiate((Object)PlayerPrefabs[i], playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);
            players[i].name = setupInfo.PlayerNames[i];
            playerScripts[i] = players[i].GetComponent<PlayerScript>();
        }

        globalMessageText = GlobalMessageText.GetComponent<Text>();
        StartCoroutine(SetGlobalMessage("GO!", 2f));

        maxScore = setupInfo.MaxScore;

        GameObject.Destroy(setupInfo.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Exit") > .1)
        {
            EndLevel();
        }
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    private void EndLevel()
    {
        GameResults results = GameObject.Instantiate<GameObject>(ResultsPrefab).GetComponent<GameResults>();
        foreach (PlayerScript player in playerScripts)
        {
            results.AddScore(player.name, player.Kills);
        }

        Application.LoadLevel("score");
    }

    public void Explode(Vector3 location, GameObject owner, float damage)
    {
        GameObject explosion = Object.Instantiate<GameObject>(ExplosionEffect);
        explosion.transform.position = location;
        Object.Destroy(explosion, 5f);

        for (int i = 0; i < players.Length; i++)
        {
            float distance = Vector3.SqrMagnitude(players[i].transform.position - location) / 2;
            float power = damage / distance;
            if (playerScripts[i].TakeDamage((int)power, location))
            {
                //score kill for owner
                owner.GetComponent<PlayerScript>().Kills++;
                string message = owner.name + " killed " + playerScripts[i].name + "!";
                StartCoroutine(SetGlobalMessage(message, 3f));
                StartCoroutine(Respawn(playerScripts[i]));
                Debug.Log(message);
            }
        }
    }

    public IEnumerator SetGlobalMessage(string message, float time)
    {
        globalMessageText.text = message;

        yield return new WaitForSeconds(time);

        globalMessageText.text = "";
    }

    private IEnumerator Respawn(PlayerScript player)
    {
        yield return new WaitForSeconds(3f);

        CheckForMaxScore();

        player.Respawn(playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length - 1)]);
    }

    private void CheckForMaxScore()
    {
        bool maxReached = false;
        foreach (PlayerScript p in playerScripts)
        {
            if (p.Kills >= maxScore)
            {
                maxReached = true;
                break;
            }
        }

        //go to score screen
        if (maxReached)
        {
            EndLevel();
        }
    }
}