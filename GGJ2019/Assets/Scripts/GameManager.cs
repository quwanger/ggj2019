using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public ItemManager ItemManager { get { return itemManager; } }

    public GameObject ui;
    public GameObject hud;
    public GameObject end;

    private ItemManager itemManager;

    public PlayerController playerPrefab;

    public List<PlayerController> team1 = new List<PlayerController>();
    public List<PlayerController> team2 = new List<PlayerController>();

    public GameObject[] team1ReadyImageChecks;
    public GameObject[] team1ReadyTextChecks;
    public GameObject[] team2ReadyImageChecks;
    public GameObject[] team2ReadyTextChecks;

    public GameObject[] playerCheckmarks;

    public Planet planet1;
    public Planet planet2;

    public Sprite[] ships;

    private AudioManager audioManager;

    public enum HouseState {
        Alive,
        Destroyed
    }

    public bool gameInProgress = false;
    public bool gameEnded = false;

    public float time;

	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);

        itemManager = GetComponent<ItemManager>();
	}

    void StartGame()
    {
        

        gameInProgress = true;
        ui.SetActive(false);
        end.SetActive(false);
        hud.SetActive(true);
		itemManager.Setup();
    }

    public void EndGame(Planet losingPlanet)
    {
        Debug.Log(losingPlanet.name + " LOSES!");
        gameInProgress = false;
        gameEnded = true;
        end.SetActive(true);

       
   
    }

	void Start () {
		time = 0.0f;
        audioManager = FindObjectOfType<AudioManager>();
    }
	
	void Update () {
        if (!gameInProgress)
        {
            SetupPlayers();
        }

        if (gameEnded)
        {
            if (Input.GetButtonDown("Start_1") || Input.GetButtonDown("Start_2") || Input.GetButtonDown("Start_3") || Input.GetButtonDown("Start_4"))
            {
                audioManager.PlaySound("gets");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                gameEnded = false;
            }
        }

		time += Time.deltaTime;
	}

    private void SetupPlayers() {
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

        audioManager.PlaySound("gets");

        if (p.playerId == 1)
        {
            playerCheckmarks[0].SetActive(true);
        }
        else if (p.playerId == 2)
        {
            playerCheckmarks[1].SetActive(true);
        }
        else if (p.playerId == 3)
        {
            playerCheckmarks[2].SetActive(true);
        }
        else if (p.playerId == 4)
        {
            playerCheckmarks[3].SetActive(true);
        }

        if (CheckAllPlayersReady())
        {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        //Wait for 1 second
        yield return new WaitForSeconds(2);
        StartGame();
    }

    public bool CheckAllPlayersReady()
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

        audioManager.PlaySound("gets");

        if (onTeam1) {
            player.SetPlayerSprite(ships[team1.Count]);
            team1.Add(player);
            player.teamId = 1;
            player.spriteRenderer.color = planet1.teamColor;
            player.homePlanet = planet1;

            if(team1.Count == 1) {
                team1ReadyImageChecks[0].GetComponent<Image>().color = planet1.teamColor;
                team1ReadyTextChecks[0].GetComponent<Text>().text = "PRESS 'START'\n TO READY UP";
                player.playerId = 1;
                player.readyCheckText = team1ReadyTextChecks[0];
                player.readyCheckImage = team1ReadyImageChecks[0];
            } else {
                team1ReadyImageChecks[1].GetComponent<Image>().color = planet1.teamColor;
                team1ReadyTextChecks[1].GetComponent<Text>().text = "PRESS 'START'\n TO READY UP";
                player.playerId = 3;
                player.readyCheckText = team1ReadyTextChecks[1];
                player.readyCheckImage = team1ReadyImageChecks[1];
            }

        } else {
            player.SetPlayerSprite(ships[team2.Count]);
            team2.Add(player);
            player.teamId = 2;
            player.spriteRenderer.color = planet2.teamColor;
            player.homePlanet = planet2;

            if(team1.Count == 1) {
                team2ReadyImageChecks[0].GetComponent<Image>().color = planet2.teamColor;
                team2ReadyTextChecks[0].GetComponent<Text>().text = "PRESS 'START'\n TO READY UP";
                player.playerId = 2;
                player.readyCheckText = team2ReadyTextChecks[0];
                player.readyCheckImage = team2ReadyImageChecks[0];
            } else {
                team2ReadyImageChecks[1].GetComponent<Image>().color = planet2.teamColor;
                team2ReadyTextChecks[1].GetComponent<Text>().text = "PRESS 'START'\n TO READY UP";
                player.playerId = 4;
                player.readyCheckText = team2ReadyTextChecks[1];
                player.readyCheckImage = team2ReadyImageChecks[1];
            }
        }
    }
}
