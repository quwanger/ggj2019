using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public XboxController controller;
    public int controllerId;
    public int speed = 5;
    public int teamId = 0;

    public bool isReady = false;

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
        transform.Rotate(new Vector3(0f, 0f, -speed * Input.GetAxis(controller.joyLeftVert)));



        transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(Input.GetAxis(controller.joyRightVert), Input.GetAxis(controller.joyRightHori)) * 180 / Mathf.PI);

        //transform.GetChild(0).transform.Rotate(new Vector3(0f, 0f, -speed * Input.GetAxis(controller.joyRightHori)));


        if(Input.GetButtonDown(controller.x))
        {
            SpawnMissile();
        }
    }

    private void SpawnMissile()
    {
        GameObject missile = Instantiate(missilePrefab, transform.GetChild(0).transform.position, transform.GetChild(0).transform.rotation, null);
        //TODO: Set the proper direction of the missile based on the crosshair
        Vector2 facingDirection = transform.GetChild(0).right.normalized;
        Debug.Log(facingDirection);
        missile.GetComponent<Missile>().Setup(this, facingDirection, 400f);
    }
}
