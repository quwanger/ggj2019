using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public List<GameObject> playerSpawnPoints = new List<GameObject>();
	public List<GameObject> itemSpawnPoints = new List<GameObject>();

    public PlayerController[] playerPrefabs;

	public float time;

	void Start () {
		time = 0.0f;
	}
	
	void Update () {
		time += Time.deltaTime;
	}
}
