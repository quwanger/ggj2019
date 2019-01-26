using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour {

    private LineRenderer lineRenderer;
    private float counter;
    private float dist;

    public Transform origin;
    public Transform destination;

    public GameObject grabbedObject;

    public float drawSpeed = 6f;

    public bool goForward = true;

	// Use this for initialization
	void Start () {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, origin.position);
        lineRenderer.SetWidth(0.45f, 0.45f);       
	}

    // Update is called once per frame
    void Update() {

        dist = Vector3.Distance(origin.position, destination.position);

        if (goForward)
        {
            counter += 0.06f;

            float x = Mathf.Lerp(0, dist, counter);

            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;

            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            lineRenderer.SetPosition(1, pointAlongLine);

            Vector3 myLength = pointAlongLine - pointA;

            if (goForward && myLength.magnitude == dist)
            {
                Debug.Log("line complete");
                goForward = false;
            }

        }
        else
        {
            counter -= 0.001f;

            float x = Mathf.Lerp(0, dist, counter);

            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;

            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            lineRenderer.SetPosition(1, pointAlongLine);
            grabbedObject.transform.position = pointAlongLine;

            Vector3 myLength = pointAlongLine - pointA;
          

        }
    }
    
}

