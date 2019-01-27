using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemManager : MonoBehaviour {
    public enum ItemState
    {	
		Idle,
        Atmosphere_Friendly,
        Atmosphere_Enemy,
        Stuck,
        Exploding,
        Hooked,
        NoCollide
    }

    public enum Powerups
    {
        None,
        Missile,
        Asteroid,
        Star
    }

    [SerializeField]
    public float spawnTime = 1;
    private float powerupSpawnTime = 1f;
	public GameObject[] itemTypes;

    public List<GameObject> items = new List<GameObject>();
    public float minSpeed;

    public bool isSpewing;
    public float maxSpeed;
    public GameObject missile;
    public GameObject asteroid;
    public GameObject star;
    public GameObject moon;

    public GameObject itemExplosion;

	void Start () {
	}

    void EvaluateSpawningMechanics() {
        int randomNumber = Random.Range(0, 101);

        if(randomNumber < 20 && isSpewing == false && items.Count < 20) {
            isSpewing = true;
            spawnTime = 0.2f;
            return;
        } else {
            isSpewing = false;
        }


        if(items.Count < 15) {
            spawnTime = 0.7f;
        } else if(items.Count < 25) {
            spawnTime = 1;
        } else {
            spawnTime = 2;
        }
    }

    IEnumerator StartSpawner() {
         while(true) {
            yield return new WaitForSeconds(spawnTime);
            Spawn();
        }
    }

	public void Setup () {
		// Spawns items
        InvokeRepeating ("EvaluateSpawningMechanics", 10.0f, 3.0f);

        StartCoroutine(StartSpawner());

        // Spawns powerups
        InvokeRepeating("SpawnPowerups", powerupSpawnTime, powerupSpawnTime);
    }

    private float chanceOfSpawningPowerup = 0.5f;
    void SpawnPowerups()
    {
        float powerupToSpawn = Random.Range(0f, 1f);
        if(powerupToSpawn <= chanceOfSpawningPowerup)
        {
            //spawn powerup
            powerupToSpawn = Random.Range(0f, 1f);
            GameObject objectToSpawn = null;
            Powerups powerupType = Powerups.None;
            float powerupSpeedMin = 0f;
            float powerupSpeedMax = 0f;
            float powerupMassMin = 0f;
            float powerupMassMax = 0f;

            if (powerupToSpawn < 0.8f)
            {
                // spawn missile
                objectToSpawn = missile;
                powerupType = Powerups.Missile;

                powerupSpeedMin = 1f;
                powerupSpeedMax = 5f;
                powerupMassMin = 1f;
                powerupMassMax = 5f;
            }
            else if (powerupToSpawn < 0.95f)
            {
                // spawn star
                objectToSpawn = star;
                powerupType = Powerups.Star;

                powerupSpeedMin = 1f;
                powerupSpeedMax = 5f;
                powerupMassMin = 1f;
                powerupMassMax = 5f;
            }
            else
            {
                // spawn asteroid
                objectToSpawn = asteroid;
                powerupType = Powerups.Asteroid;

                powerupSpeedMin = 5f;
                powerupSpeedMax = 15f;
                powerupMassMin = 5f;
                powerupMassMax = 15f;
            }


            // Instantiate item with the random x and y parameters
            Vector3 spawnPosition = GetSpawnPosition();
            GameObject item = Instantiate(objectToSpawn, GetSpawnPosition(), Quaternion.identity);
            PowerupController powerup = item.GetComponent<PowerupController>();
            powerup.Setup(0, GetSpawnDirection(spawnPosition), Random.Range(powerupSpeedMin, powerupSpeedMax), Random.Range(powerupMassMin, powerupMassMax));
            powerup.SetPowerupType(powerupType);
        }
    }

    Vector3 GetSpawnPosition()
    {
        // Get the cameras top and bottom bound, and randomly choose one
        // Get 2d camera's position in world space
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 topRightCorner = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        Vector3 bottomLeftCorner = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

        float[] yPositions = { bottomLeftCorner.y - 1, topRightCorner.y + 1 };
        float spawnPositionY = yPositions[Random.Range(0, 2)];

        float spawnPositionX = Random.Range(GetBounds().y, GetBounds().x);

        Vector3 spawnPosition = new Vector3(spawnPositionX, spawnPositionY, 0);
        return spawnPosition;        
    }

    Vector2 GetBounds()
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Atmosphere");
        Vector2 bounds = new Vector2(0f, 0f);
        foreach (GameObject planet in planets)
        {
            CircleCollider2D planetCollider = planet.GetComponent<CircleCollider2D>();

            if (planetCollider.bounds.center.x > 0)
            {
                bounds.x = planetCollider.bounds.center.x - planetCollider.bounds.extents.x;
            }
            else
            {
                bounds.y = planetCollider.bounds.center.x + planetCollider.bounds.extents.x;
            }
        }

        return bounds;
    }

    Vector2 GetSpawnDirection(Vector3 spawnPosition)
    {
        float targetPositionX = Random.Range(GetBounds().y + 1, GetBounds().x - 1);

        Vector2 targetPosition = new Vector2(targetPositionX, 0f);

        Vector2 direction = targetPosition - new Vector2(spawnPosition.x, spawnPosition.y);
        return direction;
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

		// Debug.DrawLine(spawnPosition, targetPosition, Color.green, 2);
        
        // Instantiate item with the random x and y parameters
        GameObject item = Instantiate (itemTypes[randomItemIndex], spawnPosition, Quaternion.identity);
        items.Add(item);
        ItemController itemController = item.GetComponent<ItemController>();
        itemController.Setup(Random.Range(1, 3), direction, Random.Range(minSpeed, maxSpeed), Random.Range(1f, 5f));
	}

	void Update () {
	}
 
}
