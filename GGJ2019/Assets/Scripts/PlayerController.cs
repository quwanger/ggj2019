using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public XboxController controller;
    public int controllerId;
    public int speed = 5;

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
        transform.RotateAround(this.transform.position, Vector3.back, speed * Input.GetAxis(controller.joyLeftVert));

        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //transform.rotation = Quaternion.LookRotation(this.transform.position - mousePosition, Vector3.forward);
    }
}
