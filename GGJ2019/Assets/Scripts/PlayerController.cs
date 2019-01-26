using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public XboxController controller;
    public int controllerId;
    public int speed = 5;
    public int teamId = 0;

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
    }
}
