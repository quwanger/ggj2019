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


        Vector3 pointA = Vector3.zero ;
        Vector3 pointB = Vector3.zero;

        if (goForward)
        {
            counter += 0.06f;

            float x = Mathf.Lerp(0, dist, counter);

             pointA = origin.position;
             pointB = destination.position;

            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            lineRenderer.SetPosition(1, pointAlongLine);

            Vector3 myLength = pointAlongLine - pointA;

            if (goForward && myLength.magnitude == dist)
            {
                Debug.Log("line complete");
                goForward = false;
                PushRadius(destination.position, 1f, destination.position - origin.position);
            }

        }
        else
        {
            counter -= 0.009f;

            float x = Mathf.Lerp(0, dist, counter);

            pointA = origin.position;
            pointB = destination.position;

            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            lineRenderer.SetPosition(1, pointAlongLine);

           
            Vector3 myLength = pointAlongLine - pointA;

            //grab the object
            grabbedObject.transform.position = pointAlongLine;


            
        }

    }


    void PushRadius(Vector2 center, float radius, Vector2 direction)
    {
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        int i = 0;
        while (i < hitColliders.Length)
        {
            hitColliders[i].SendMessage("Push", direction.normalized * 100);
            i++;
        }
    }

}

