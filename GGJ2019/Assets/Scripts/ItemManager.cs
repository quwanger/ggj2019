using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
	public float spawnTime;
	public GameObject[] itemTypes;

	void Start () {
	}

	public void Setup () {
		// Spawns items
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}
	
	void Spawn () {
		// Random item type
		int randomIndex = Random.Range(0, itemTypes.Length);
		ItemController item = itemTypes[randomIndex].GetComponent<ItemController>();

		// Set items props
		item.speed = Random.Range(5, 15);
		item.mass = Random.Range(1, 5);
		item.teamId = Random.Range(0, 2);

		// Get 2d camera's position in world space
		Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		Vector3 topRightCorner = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
		Vector3 bottomLeftCorner = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
		
		
		// Get the left and right bounds of where we can spawn the item
		GameObject[] planets = GameObject.FindGameObjectsWithTag("Atmosphere");
		float rightBound = 0.0f;
		float leftBound = 0.0f;
		foreach (GameObject planet in planets) {
			CircleCollider2D planetCollider = planet.GetComponent<CircleCollider2D>();

			if(planetCollider.bounds.center.x > 0) {
				rightBound = planetCollider.bounds.center.x - planetCollider.bounds.extents.x;
			} else {
				leftBound = planetCollider.bounds.center.x + planetCollider.bounds.extents.x;
			}
		}

		// Random position within those bounds
		float xPosition = Random.Range(leftBound, rightBound);
		float xPositionDirection = Random.Range(leftBound, rightBound);

		// Get the cameras world space upper and lower bounds
		int index = Random.Range(0, 2);
		float[] yPositions = { bottomLeftCorner.y, topRightCorner.y };

		// Random position within those bounds
		float yPosition = yPositions[index];

		

		// Move the new item towards a random point between the planets along the middle of the screen
		Vector2 targetPosition = new Vector2(xPositionDirection, 0f);
		Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0);

		Vector2 direction = targetPosition - new Vector2(spawnPosition.x, spawnPosition.y);
		item.direction = direction;

		// Instantiate item with the random x and y parameters
		Instantiate (item, spawnPosition, Quaternion.identity);
	}

	void Update () {
	}
}
