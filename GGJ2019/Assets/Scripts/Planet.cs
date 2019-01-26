using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public int planetId;

    public float rotationSpeed = 10f;
    public float gravitationalPull = 100f;
    [Range(1.5f, 2.5f)]
    public float atmosphereRadius = 2f;
    public Color teamColor;

    public float maxGravDist = 10.0f;
    public float maxGravity = 0.25f;

    [SerializeField]
    private SpriteRenderer spritePlanet;
    [SerializeField]
    private SpriteRenderer spriteAtmosphere;
    [SerializeField]
    private Transform atmosphere;

    private List<House> houses = new List<House>();

    void Start()
    {
        Setup(Random.Range(1.5f, 2.5f), new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }
    public void Setup(float _atmosphereRadius, Color _teamColor)
    {
        //set values
        teamColor = _teamColor;
        atmosphereRadius = _atmosphereRadius;

        //assign new values to world objects
        atmosphere.transform.localScale = new Vector3(atmosphereRadius, atmosphereRadius, atmosphereRadius);
        spritePlanet.color = teamColor;
        spriteAtmosphere.color = new Color(teamColor.r, teamColor.g, teamColor.b, 0.35f);
    }
    void FixedUpdate()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
    }
}
