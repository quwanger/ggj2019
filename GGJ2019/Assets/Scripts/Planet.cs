using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public int planetId;

    public float rotationSpeed = 5f;
    [Range(1.5f, 2.5f)]
    public float atmosphereRadius = 2f;
    public Color teamColor;

    public GameObject rocketHUD;
    public GameObject healthHUD;

    private int missileCount = 20;
    public int MissileCount { get { return missileCount; } }

    // this value is the distance from the planet center to the edge of the atmosphere
    private float maxGravDist = 8f;
    public float MaxGravDist { get { return maxGravDist; } }
    private float maxGravity = 0.75f;
    public float MaxGravity { get { return maxGravity; } }

    [SerializeField]
    private SpriteRenderer spritePlanet;
    [SerializeField]
    private SpriteRenderer spriteAtmosphere;
    [SerializeField]
    private Transform atmosphere;

    private List<House> houses = new List<House>();

    private List<ItemController> defensiveObjects = new List<ItemController>();

    void Awake()
    {

        int randomID = Random.Range(0, 1);

        if(randomID > 0.5)
        {
            if (planetId == 1)
                Setup(2f, new Color(Random.Range(0.5f, 1f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f)));
            if (planetId == 2)
                Setup(2f, new Color(Random.Range(0f, 0.35f), Random.Range(0.6f, 1f), Random.Range(0.6f, 1f)));
        }
        else
        {
            if (planetId == 1)
                Setup(2f, new Color(Random.Range(0f, 0.35f), Random.Range(0.6f, 1f), Random.Range(0.6f, 1f)));
            if (planetId == 2)
                Setup(2f, new Color(Random.Range(0.5f, 1f), Random.Range(0f, 0.5f), Random.Range(0f, 0.4f)));

        }
    }
    public void Setup(float _atmosphereRadius, Color _teamColor)
    {
        //set values
        teamColor = _teamColor;
        atmosphereRadius = _atmosphereRadius;
        // add random rotation direction
        rotationSpeed = Random.Range(0f, 1f) > 0.5f ? rotationSpeed : rotationSpeed * -1f;
        //assign new values to world objects
        //atmosphere.transform.localScale = new Vector3(atmosphereRadius, atmosphereRadius, atmosphereRadius);
        spritePlanet.color = teamColor;
        spriteAtmosphere.color = new Color(teamColor.r, teamColor.g, teamColor.b, 0.35f);
        rocketHUD.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = missileCount.ToString();
        healthHUD.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "10/10";
    }

    public void AcceptItem(ItemController item)
    {
        defensiveObjects.Add(item);
    }

    public void RemoveItemFromPlanet(ItemController item)
    {
        if(defensiveObjects.Contains(item))
        {
            defensiveObjects.Remove(item);
        }
    }

    public void ConsumeMissile()
    {
        missileCount--;
        rocketHUD.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = missileCount.ToString();
    }

    public void GetMissiles(int missiles)
    {
        missileCount += missiles;
        rocketHUD.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = missileCount.ToString();
    }

    public void RegisterHouse(House h)
    {
        houses.Add(h);
    }

    public void DestroyHouse(House h)
    {
        rotationSpeed += 3;
        int houseCount = 0;
        foreach (House House in houses) {
            if(House.houseState == GameManager.HouseState.Alive) {
                houseCount += 1;
            }
        }

        healthHUD.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = houseCount.ToString() + "/10";

        foreach(House House in houses) {
            if(House.houseState == GameManager.HouseState.Alive) {
                return;
            }
        }

        GameManager.instance.EndGame(this);
    }

    void FixedUpdate()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
    }

    void Update()
    {
    }
}
