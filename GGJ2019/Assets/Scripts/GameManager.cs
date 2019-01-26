using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public List<GameObject> playerSpawnPoints = new List<GameObject>();
	public List<GameObject> itemSpawnPoints = new List<GameObject>();

    public PlayerController[] playerPrefabs;

    public Planet planet01;

	public float time;

	void Start () {
		time = 0.0f;
	}
	
	void Update () {
        SetupPlayers();

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

    private void SpawnPlayer(int controllerId)
    {
        if (playerPrefabs[controllerId - 1].controllerId != controllerId)
        {
            playerPrefabs[controllerId - 1].controllerId = controllerId;
            PlayerController player = Instantiate(playerPrefabs[controllerId - 1], planet01.transform.position, Quaternion.identity);
        }
    }

    private void SpawnPositions()
    {
        float planetRadius = 10.0f;
    }
}
