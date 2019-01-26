using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private ItemManager itemManager;

    public PlayerController[] playerPrefabs;

	public float time;

	void Awake () {
		itemManager = GetComponent<ItemManager>();

		InitializeGame();
	}

	void InitializeGame() {
		itemManager.Setup();
	}

	void Start () {
		time = 0.0f;
	}
	
	void Update () {
		time += Time.deltaTime;
	}
}
