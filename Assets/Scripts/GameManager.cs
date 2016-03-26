using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public int NumberOfPlayers = 1;
    private GameObject[] players;
    private GameObject[] playerSpawnPoints;
    private PlayerScript[] playerScripts;

    public GameObject ExplosionEffect;
    public GameObject GlobalMessageText;
    private Text globalMessageText;

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

        players = new GameObject[NumberOfPlayers];
        //PlayerSpawnPoints = new GameObject[NumberOfPlayers];
        playerSpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        playerScripts = new PlayerScript[NumberOfPlayers];

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            players[i] = (GameObject)GameObject.Instantiate((Object)PlayerPrefabs[i], playerSpawnPoints[i].transform.position, playerSpawnPoints[i].transform.rotation);
            playerScripts[i] = players[i].GetComponent<PlayerScript>();
        }

        globalMessageText = GlobalMessageText.GetComponent<Text>();
        StartCoroutine(SetGlobalMessage("GO!", 2f));
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
            if (playerScripts[i].TakeDamage((int)power))
            {
                //score kill for owner
                owner.GetComponent<PlayerScript>().Kills++;
                string message = owner.tag + " killed " + playerScripts[i].tag + "!";
                StartCoroutine(SetGlobalMessage(message, 3f));
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
}