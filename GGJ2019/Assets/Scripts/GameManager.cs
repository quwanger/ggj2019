using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public List<GameObject> playerSpawnPoints = new List<GameObject>();
	public List<GameObject> itemSpawnPoints = new List<GameObject>();

    // public PlayerController[] playerPrefabs;

	public float time;

	// Use this for initialization
	void Start () {
		time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
	}
}
