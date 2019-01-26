using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour {

    private LineRenderer lineRenderer;
    private float counter;
    private float dist;

    private bool goForward = true;
    private bool isArmShooting = false;

    private GameObject grabbedObject;

    public Transform origin;
    public Transform destination;

    public float goSpeed = 6f;
    public float returnSpeed = 1f;

    public bool grabMode = true;
    public bool pushMode = false;

    public float grabRadius = 0f;
    public float pushRadius = 0f;

    public float pushForce = 10f;

    //allows player to move the arm after grabbing an object
    public bool canMoveAndGrab = true;

	void Start () {

        lineRenderer = GetComponent<LineRenderer>();    
	}

    void Update() {

        //to be replaced with controller
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
            isArmShooting = true;

        if(isArmShooting)
            LaunchArm();           
    }

    /// <summary>
    /// Create a shockwave that pushes rigidbodies away (direction dependent)
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="direction"></param>
    void PushRadius(Vector2 center, float radius, Vector2 direction)
    {
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        
        for  (int a = 0; a < hitColliders.Length; a++)
        {
            hitColliders[a].SendMessage("Push", direction.normalized * pushForce);
          
        }
    }

    /// <summary>
    /// function launches arm and returns it to it's origin.
    /// </summary>
    void LaunchArm()
    {

        lineRenderer.SetPosition(0, origin.position);
        lineRenderer.SetWidth(0.45f, 0.45f);

        dist = Vector3.Distance(origin.position, destination.position);

        Vector3 myLength = Vector3.zero;

        Vector3 pointA = Vector3.zero;
        Vector3 pointB = Vector3.zero;

        if (goForward)
        {
            counter += goSpeed / 10;

            float x = Mathf.MoveTowards(0, dist, counter);


            pointA = origin.position;
            pointB = destination.position;

            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            lineRenderer.SetPosition(1, pointAlongLine);
         
            if(Vector3.Distance(origin.position,pointAlongLine) > 3f && grabMode)
                GrabObject(pointAlongLine, grabRadius);

            myLength = pointAlongLine - pointA;

            //max distance
            if (goForward && myLength.magnitude == dist)
            {
                Debug.Log("line complete");
                goForward = false;
            }

            //push as you move along...

            if(pushMode)
             PushRadius(destination.position, pushRadius, destination.position - origin.position);

        }
        else
        {
           
            counter -= returnSpeed /10;

            float x = Mathf.MoveTowards(0, dist, counter);

            pointA = origin.position;
            pointB = destination.position;

            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            lineRenderer.SetPosition(1, pointAlongLine);


            //grab the object
            if(grabbedObject != null)
                grabbedObject.transform.position = pointAlongLine;

             myLength = pointAlongLine - pointA;
            Debug.Log("im returning my lenght is " + myLength.magnitude);
            if (myLength.magnitude < 0.1f)
            {
                Debug.Log("Launch Complete");
                goForward = true;
                isArmShooting = false;
                grabbedObject = null;
 
            }

        }
    }

    /// <summary>
    ///  Grab closest object within radius
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    void GrabObject(Vector2 center, float radius)
    {


        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

     
        Collider2D cMin = null;
        float minDist = Mathf.Infinity;
        foreach (Collider2D c in hitColliders)
        {
            float dist = Vector3.Distance(c.gameObject.transform.position, center);
            if (dist < minDist)
            {
                cMin = c;
                minDist = dist;
                grabbedObject = cMin.gameObject;
                hitColliders = new Collider2D[0];
                goForward = false;
            }
        }
    }

}

