using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour {

    private LineRenderer robotArm;
    private float counter;
    private float dist;

    private bool goForward = true;
    private bool isArmShooting = false;

    private GameObject grabbedObject;

    //speed modifier for heavier items
    private float grabbedObjectWeight = 1f;

    public Transform origin;
    public Vector3 destination;

    public float goSpeed;
    public float returnSpeed;

    public bool grabMode = true;
    public bool pushMode = false;

    public float grabRadius;
    public float pushRadius; 

    //radius of inactive grab to avoid grabbing objects that are already in the atmosphere (extremely close to the player)
    public float inactiveGrabRadius = 2f;
    public float pushForce = 100f;

    //allows player to move the arm after grabbing an object
    public bool canMoveAndGrab = true;

    PlayerController pController = null;

    GameObject glove;

    void Start () {

        pController = this.gameObject.GetComponentInParent<PlayerController>();
        robotArm = GetComponent<LineRenderer>();

        goSpeed = 9f;
        returnSpeed = 3f;
        grabRadius = 1f;
        pushRadius = 1f;


        //get glove object
        GameObject glove = null;



        foreach (Transform child in this.transform)
        {
            if (child.tag == "Glove")
                glove = child.gameObject;
        }


        glove.GetComponent<SpriteRenderer>().enabled = false;
        glove.GetComponent<BoxCollider2D>().enabled = false;

        if (destination == null)
        {
            //  destination = GameObject.FindGameObjectWithTag("1_crosshair").transform;

            //destination = pController.transform.GetChild(0).transform;
        }
	}

    private bool m_isAxisInUse = false;

    void Update()
    {

        if (Input.GetAxis(pController.controller.rt) != 0)
        {
            grabMode = true;
            pushMode = false;
        }

        if (Input.GetAxis(pController.controller.lt) != 0)
        {
            grabMode = false;
            pushMode = true;
        }


    


        GameObject.FindGameObjectWithTag("1_crosshair").transform.position = origin.position - (-pController.transform.GetChild(0).transform.right * 18);
        Debug.Log(Input.GetButtonDown(pController.controller.rt));

        //to be replaced with controller
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0) || (Input.GetAxis(pController.controller.rt) > 0 && !m_isAxisInUse) || (Input.GetAxis(pController.controller.lt) > 0 && !m_isAxisInUse))
        {

            Debug.Log(Input.GetButtonDown(pController.controller.rt));
            /*destination = GameObject.FindGameObjectWithTag("1_crosshair").transform;
            Vector3 staticDestination = destination.position;*/
            destination = GameObject.FindGameObjectWithTag("1_crosshair").transform.position = origin.position - (-pController.transform.GetChild(0).transform.right * 18);
            isArmShooting = true;
            m_isAxisInUse = true;
            if (pushMode)
            {

                glove = GameObject.FindGameObjectWithTag("Glove");

                glove.GetComponent<SpriteRenderer>().enabled = true;
                glove.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        if (isArmShooting)
        {
            LaunchArm(destination);
        }
        if (Input.GetAxis(pController.controller.rt) == 0 && Input.GetAxis(pController.controller.lt) == 0 && !isArmShooting)
        {

            m_isAxisInUse = false;
        }
    }

    /// <summary>
    /// Create a shockwave that pushes rigidbodies away (direction dependent)
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="direction"></param>
    void PushRadius(Vector2 currentPosition, float radius, Vector2 direction)
    {

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(currentPosition, radius);

        for  (int a = 0; a < hitColliders.Length; a++)
        {
            if (hitColliders[a].gameObject.tag.Equals("Item"))
             hitColliders[a].SendMessage("Push", direction.normalized * pushForce);
            //set the glove to the position of the end of the line
            glove.transform.position = currentPosition;
        }


    }
    Vector3 staticDestination = Vector3.zero;
    /// <summary>
    /// function launches arm and returns it to it's origin.
    /// </summary>
    void LaunchArm(Vector3 pointB)
    {

        robotArm.SetPosition(0, origin.position);
        robotArm.SetWidth(0.3f, 0.3f);

        dist = Vector3.Distance(origin.position, pointB);

        Vector3 myLength = Vector3.zero;
        Vector3 pointA = Vector3.zero;

        if (pushMode)
            dist = dist * 0.5f;

        //Launch the arm
        if (goForward)
        {
            counter += goSpeed / 10;

            float x = Mathf.MoveTowards(0, dist, counter);

            pointA = origin.position;


            
            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            robotArm.SetPosition(1, pointAlongLine);
         
            if(Vector3.Distance(origin.position,pointAlongLine) > inactiveGrabRadius && grabMode)
                GrabObject(pointAlongLine, grabRadius);

            //push as you move along...
           if (pushMode)
                PushRadius(pointAlongLine, pushRadius, destination - origin.position);

            myLength = pointAlongLine - pointA;

            //max distance
            if (Mathf.Ceil(  myLength.magnitude) >= Mathf.Floor(dist) )
            {
                if (pushMode)
                {
                    glove.GetComponent<SpriteRenderer>().enabled = false;
                    glove.GetComponent<BoxCollider2D>().enabled = false;
                    glove.transform.position = origin.position;
                }

                //Debug.Log("line complete");
                goForward = false;
                
            }
        }
        //Return the arm
        else
        {    
            counter -= returnSpeed /10;

            float x = Mathf.MoveTowards(0, dist, counter);

            pointA = origin.position;

          
            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            robotArm.SetPosition(1, pointAlongLine);


            //grab the object
            if(grabbedObject != null)
                grabbedObject.transform.position = pointAlongLine;

             myLength = pointAlongLine - pointA;
             //Debug.Log("I am returning to the origin |||| my length is " + myLength.magnitude);
            if (myLength.magnitude < 0.1f)
            {
                //Debug.Log("Launch Complete");
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

            if (dist < minDist && c.gameObject.tag.Equals("Item"))
            {
                cMin = c;
                minDist = dist;
                grabbedObject = cMin.gameObject;
                hitColliders = new Collider2D[0];
                //return the arm once it's grabbed an object
                goForward = false;


            }
        }
    }

} 

