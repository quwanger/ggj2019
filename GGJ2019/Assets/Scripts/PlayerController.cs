using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public XboxController controller;
    public int controllerId;
    public int speed = 5;
    public int teamId = 0;
    public Planet homePlanet;

    public bool isReady = false;

    public int missileCount = 5;

    public SpriteRenderer spriteRenderer;

    public GameObject missilePrefab;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
	void Start ()
    {
        SetupControls();
    }
	
	void Update ()
    {
        ControllerInput();
    }

    private void SetupControls()
    {
        controller = new XboxController(controllerId);
    }

    private void ControllerInput()
    {
        float teamInputModifier = teamId == 1 ? -1 : 1;
        transform.Rotate(new Vector3(0f, 0f, teamInputModifier * speed * Input.GetAxis(controller.joyLeftVert)));

        transform.GetChild(0).transform.Rotate(new Vector3(0f, 0f, -speed * Input.GetAxis(controller.joyRightHori)));

        if(Input.GetButtonDown(controller.rb) || Input.GetButtonDown(controller.lb))
        {
            SpawnMissile();
        }
    }

    public void SetPlayerSprite(Sprite ship)
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ship;
    }
    private void SpawnMissile()
    {
        if (homePlanet.MissileCount > 0)
        {
            GameObject missile = Instantiate(missilePrefab, transform.GetChild(0).transform.position, transform.GetChild(0).transform.rotation, null);
            //TODO: Set the proper direction of the missile based on the crosshair
            Vector2 facingDirection = transform.GetChild(0).right.normalized;
            missile.GetComponent<Missile>().Setup(this, facingDirection, 400f, teamId, spriteRenderer.color);
            Destroy(missile, 10f);
            homePlanet.ConsumeMissile();
        }
        else
        {
            // NO MISSILES!
        }
    }
}
