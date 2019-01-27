using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public XboxController controller;
    public int controllerId;
    public int playerId;
    public int speed = 5;
    public int teamId = 0;
    public Planet homePlanet;

    public GameObject readyCheckText;
    public GameObject readyCheckImage;

    public GameManager gameManager;

    public bool isReady = false;

    public int missileCount = 5;

    public SpriteRenderer spriteRenderer;

    public GameObject missilePrefab;

    public SpriteRenderer magnet;
    public SpriteRenderer fist;
    private AudioManager audioManager;
    public AudioManager AudioManager { get { return audioManager; } }

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
	void Start ()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        SetupControls();

        magnet.color = spriteRenderer.color;
        fist.color = spriteRenderer.color;
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

        Vector3 prevJoyStick = Vector3.up;

        Quaternion currRot = this.transform.rotation;
        float teamInputModifier = teamId == 1 ? -1 : 1;

        float timeCount = 0.0f;
        Vector3 angles = new Vector3(0, 0, Mathf.Atan2(Input.GetAxis(controller.joyLeftVert), Input.GetAxis(controller.joyLeftHori)) * 180 / Mathf.PI);

        Vector3 boom = angles;
        angles = angles + new Vector3(0, 0, -90f);
        Quaternion _targetRotation = Quaternion.Euler(angles);

        _targetRotation = Quaternion.EulerAngles(_targetRotation.eulerAngles + new Vector3(90f, 90f, 90f));


        if (boom.magnitude > 0.01f)
            transform.rotation = Quaternion.RotateTowards(currRot, Quaternion.Euler(angles.x, angles.y, angles.z), 2f);

        
     

       //this is good
       //transform.rotation = Quaternion.Euler(angles.x, angles.y, angles.z);

            //transform.Rotate(new Vector3(0f, 0f, teamInputModifier * speed * Input.GetAxis(controller.joyLeftVert)));


            //transform.Rotate(new Vector3(0f, 0f, teamInputModifier * speed * Input.GetAxis(controller.joyLeftVert)));

        Vector3 rotation = new Vector3(0, 0, -Mathf.Atan2(Input.GetAxis(controller.joyRightVert), Input.GetAxis(controller.joyRightHori)) * 180 / Mathf.PI);

        //if(rotation.magnitude > 0.01f)
        if (Mathf.Abs(Input.GetAxis(controller.joyRightVert)) > 0.1f || Mathf.Abs(Input.GetAxis(controller.joyRightHori)) > 0.1f)
        {
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(Input.GetAxis(controller.joyRightVert), Input.GetAxis(controller.joyRightHori)) * 180 / Mathf.PI);
        }

        //transform.GetChild(0).transform.Rotate(new Vector3(0f, 0f, -speed * Input.GetAxis(controller.joyRightHori)));


        if (Input.GetButtonDown(controller.joyRightClick) || Input.GetButtonDown(controller.joyLeftClick) || Input.GetButtonDown(controller.a) || Input.GetButtonDown(controller.x))
        {
            if (gameManager.CheckAllPlayersReady() == true)
            {
                SpawnMissile();
            }
            
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
