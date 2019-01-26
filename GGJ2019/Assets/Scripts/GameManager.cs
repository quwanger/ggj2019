using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public ItemManager ItemManager { get { return itemManager; } }

    public GameObject ui;

    private ItemManager itemManager;

    public PlayerController playerPrefab;

    public List<PlayerController> team1 = new List<PlayerController>();
    public List<PlayerController> team2 = new List<PlayerController>();

    public GameObject[] team1ReadyChecks;
    public GameObject[] team2ReadyChecks;

    public Planet planet1;
    public Planet planet2;

    public enum HouseState {
        Alive,
        Destroyed
    }

    public bool gameInProgress = false;

    public float time;

	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        itemManager = GetComponent<ItemManager>();
	}

    void StartGame()
    {
        gameInProgress = true;
        ui.SetActive(false);
		itemManager.Setup();
    }

    public void EndGame(Planet losingPlanet)
    {
        Debug.Log(losingPlanet.name + " LOSES!");
        gameInProgress = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	void Start () {
		time = 0.0f;
	}
	
	void Update () {
        if (!gameInProgress)
        {
            SetupPlayers();
        }

		time += Time.deltaTime;
	}

    private void SetupPlayers()
    {
        if(Input.GetButtonDown("Start_1"))
        {
            SpawnPlayer(1);
        }
        else if (Input.GetButtonDown("Start_2"))
        {
            SpawnPlayer(2);
        }
        else if (Input.GetButtonDown("Start_3"))
        {
            SpawnPlayer(3);
        }
        else if (Input.GetButtonDown("Start_4"))
        {
            SpawnPlayer(4);
        }
    }

    private void ReadyUpPlayer(PlayerController p)
    {
        p.isReady = true;
        p.readyCheckObject.GetComponent<Image>().color = Color.green;
        if(CheckAllPlayersReady())
        {
            //start game
            StartGame();
        }
    }

    private bool CheckAllPlayersReady()
    {
        foreach (PlayerController p in team1)
        {
            if (!p.isReady) return false;
        }

        foreach (PlayerController p in team2)
        {
            if (!p.isReady) return false;
        }

        return true;
    }

    private void SpawnPlayer(int controllerId) {
        foreach(PlayerController p in team1) {
            if (p.controllerId == controllerId) {
                if(!p.isReady) {
                    ReadyUpPlayer(p);
                }
                return;
            }
        }

        foreach (PlayerController p in team2) {
            if (p.controllerId == controllerId) {
                if (!p.isReady) {
                    ReadyUpPlayer(p);
                }
                return;
            }
        }

        bool onTeam1 = team1.Count == team2.Count;
        PlayerController player = Instantiate(playerPrefab, onTeam1 ? planet1.transform.position : planet2.transform.position, Quaternion.identity, onTeam1 ? planet1.transform : planet2.transform);
        player.transform.SetParent(null);
        player.controllerId = controllerId;
        
        if (onTeam1) {
            team1.Add(player);
            player.teamId = 1;
            player.spriteRenderer.color = planet1.teamColor;

            if(team1.Count == 1) {
                team1ReadyChecks[0].GetComponent<Image>().color = planet1.teamColor;
                player.readyCheckObject = team1ReadyChecks[0];
            } else {
                team1ReadyChecks[1].GetComponent<Image>().color = planet1.teamColor;
                player.readyCheckObject = team1ReadyChecks[1];
            }

        } else {
            team2.Add(player);
            player.teamId = 2;
            player.spriteRenderer.color = planet2.teamColor;

            if(team1.Count == 1) {
                team2ReadyChecks[0].GetComponent<Image>().color = planet2.teamColor;
                player.readyCheckObject = team2ReadyChecks[0];
            } else {
                team2ReadyChecks[1].GetComponent<Image>().color = planet2.teamColor;
                player.readyCheckObject = team2ReadyChecks[1];
            }
        }
    }
}
