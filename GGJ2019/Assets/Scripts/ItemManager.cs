using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerController : MonoBehaviour {
	public float spawnTime = 3f; // How long between each spawn.
	public ItemController[] itemTypes;

	public Transform[] spawnPoints; // An array of the spawn points the item can spawn from.


	void Start () {
		// Spawns items
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}
	
	void Spawn () {
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);

		System.Random random = new System.Random();
		int randomIndex = random.Next(itemTypes.Length);

		ItemController item = itemTypes[randomIndex];

		Instantiate (item, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
	}

	void Update () {
	}
}
