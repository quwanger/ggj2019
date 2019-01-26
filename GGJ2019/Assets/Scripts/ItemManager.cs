using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemManager : MonoBehaviour {
    public enum ItemState
    {	
		Recently_Spawned,
        Idle,
        Atmosphere_Friendly,
        Atmosphere_Enemy,
        Stuck,
        Exploding
    }

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
		int randomItemIndex = Random.Range(0, itemTypes.Length);
		
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
		float spawnPositionX = Random.Range(leftBound, rightBound);
		float targetPositionX = Random.Range(leftBound+1, rightBound-1);

		// Get the cameras top and bottom bound, and randomly choose one
		// Get 2d camera's position in world space
		Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		Vector3 topRightCorner = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
		Vector3 bottomLeftCorner = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

		float[] yPositions = { bottomLeftCorner.y - 1, topRightCorner.y + 1 };
		float spawnPositionY = yPositions[Random.Range(0, 2)];

		// Move the new item towards a random point between the planets along the middle of the screen
		Vector2 targetPosition = new Vector2(targetPositionX, 0f);
		Vector3 spawnPosition = new Vector3(spawnPositionX, spawnPositionY, 0);

		Vector2 direction = targetPosition - new Vector2(spawnPositionX, spawnPositionY);

		Debug.DrawLine(spawnPosition, targetPosition, Color.green, 2);
        
        // Instantiate item with the random x and y parameters
        GameObject item = Instantiate (itemTypes[randomItemIndex], spawnPosition, Quaternion.identity);
        ItemController itemController = item.GetComponent<ItemController>();
        itemController.Setup(Random.Range(1, 3), direction, Random.Range(5f, 15f), Random.Range(1f, 5f));
	}

	void Update () {
	}
}
