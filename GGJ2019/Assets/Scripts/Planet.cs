using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public int planetId;

    public float rotationSpeed = 5f;
    [Range(1.5f, 2.5f)]
    public float atmosphereRadius = 2f;
    public Color teamColor;

    // this value is the distance from the planet center to the edge of the atmosphere
    private float maxGravDist = 14.5f;
    public float MaxGravDist { get { return maxGravDist; } }
    private float maxGravity = 1.25f;
    public float MaxGravity { get { return maxGravity; } }

    [SerializeField]
    private SpriteRenderer spritePlanet;
    [SerializeField]
    private SpriteRenderer spriteAtmosphere;
    [SerializeField]
    private Transform atmosphere;

    // private List<House> houses = new List<House>();

    private List<ItemController> defensiveObjects = new List<ItemController>();

    void Awake()
    {
        Setup(2f, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }
    public void Setup(float _atmosphereRadius, Color _teamColor)
    {
        //set values
        teamColor = _teamColor;
        atmosphereRadius = _atmosphereRadius;
        // add random rotation direction
        rotationSpeed = Random.Range(0f, 1f) > 0.5f ? rotationSpeed : rotationSpeed * -1f;
        //assign new values to world objects
        atmosphere.transform.localScale = new Vector3(atmosphereRadius, atmosphereRadius, atmosphereRadius);
        spritePlanet.color = teamColor;
        spriteAtmosphere.color = new Color(teamColor.r, teamColor.g, teamColor.b, 0.35f);
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

    public void RegisterHouse(House h)
    {
        houses.Add(h);
    }

    public void DestroyHouse(House h)
    {
        houses.Remove(h);
        Destroy(h.gameObject);
    }

    void FixedUpdate()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
    }

    void Update()
    {
        if(GameManager.instance.gameInProgress)
        {
            if(houses.Count <= 0)
            {
                //GAME OVER, THIS TEAM LOSES
                GameManager.instance.EndGame(this);
            }
        }
    }
}
