using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour
{
    public GameObject shipGlove;
    public GameObject shipMagnet;

    private LineRenderer lineRenderer;
    private float counter;
    private float totalDistanceToTravel;

    private bool goForward = true;
    private bool isArmShooting = false;

    private GameObject grabbedObject;

    //speed modifier for heavier items
    private float grabbedObjectWeight = 1f;

    public Transform shipCenter;
    public Vector3 destination;

    public float goSpeed;
    public float returnSpeed;

    public bool grabMode = false;
    public bool pushMode = false;

    public float grabRadius;
    public float pushRadius; 

    //radius of inactive grab to avoid grabbing objects that are already in the atmosphere (extremely close to the player)
    public float inactiveGrabRadius = 2f;
    public float pushForce = 100f;

    //allows player to move the arm after grabbing an object
    public bool canMoveAndGrab = true;

    PlayerController playerController = null;    

    void Start ()
    {
        playerController = gameObject.GetComponentInParent<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material.color = Color.white;
        lineRenderer.SetWidth(0.02f, 0.02f);

        shipMagnet.GetComponent<SpriteRenderer>().enabled = false;
        shipGlove.GetComponent<SpriteRenderer>().enabled = false;
        shipGlove.GetComponent<BoxCollider2D>().enabled = false;

        goSpeed = 5f;
        returnSpeed = 3f;
        grabRadius = 1f;
        pushRadius = 1f;
	}

    private bool armActive = false;
    private bool triggersRefreshed = true;
    private float grabDistance = 11f;
    private float pushDistance = 8.5f;

    void Update()
    {
        // check for new mode if the arm isn't active
        if (!armActive && triggersRefreshed)
        {
            //toggle the controls
            if (Input.GetAxis(playerController.controller.lt) != 0)
            {
                StartGrab();
            }
            else if (Input.GetAxis(playerController.controller.rt) != 0)
            {
                StartPush();
            }
        }
        else
        {
            if (Input.GetAxis(playerController.controller.rt) == 0 && Input.GetAxis(playerController.controller.lt) == 0)
            {
                triggersRefreshed = true;
            }
        }

        if (armActive)
        {
            LaunchArm(destination);
        }
    }

    private void StartGrab()
    {
        armActive = true;
        grabMode = true;
        pushMode = false;
        triggersRefreshed = false;

        counter = 0;

        shipMagnet.GetComponent<SpriteRenderer>().enabled = true;

        destination = shipCenter.position - (-playerController.transform.GetChild(0).transform.right * grabDistance);
        //LaunchArm(destination);
    }

    private void StartPush()
    {
        armActive = true;
        grabMode = false;
        pushMode = true;
        triggersRefreshed = false;

        counter = 0;

        shipGlove.GetComponent<SpriteRenderer>().enabled = true;
        shipGlove.GetComponent<BoxCollider2D>().enabled = true;

        destination = shipCenter.position - (-playerController.transform.GetChild(0).transform.right * pushDistance);
        //LaunchArm(destination);
    }

    /// <summary>
    /// Create a shockwave that pushes rigidbodies away (direction dependent)
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="direction"></param>
    void PushRadius(Vector2 currentPosition, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(currentPosition, radius);

        for  (int a = 0; a < hitColliders.Length; a++)
        {
            if (hitColliders[a].gameObject.tag.Equals("Item") || hitColliders[a].gameObject.tag.Equals("Powerup") )
            {
                Vector2 itemDirection = new Vector2(hitColliders[a].transform.position.x - shipGlove.transform.position.x, hitColliders[a].transform.position.y - shipGlove.transform.position.y);
                hitColliders[a].GetComponent<Rigidbody2D>().AddForce(itemDirection.normalized * pushForce);
            }
        }
    }

    

    /// <summary>
    /// function launches arm and returns it to it's origin.
    /// </summary>
    void LaunchArm(Vector3 finalDestination)
    {
        lineRenderer.SetPosition(0, shipCenter.position);
        totalDistanceToTravel = Vector3.Distance(shipCenter.position, finalDestination);

        //Launch the arm
        if (goForward)
        {
            counter += goSpeed / 10;

            float x = Mathf.MoveTowards(0, totalDistanceToTravel, counter);
            
            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(finalDestination - shipCenter.position) + shipCenter.position;
            lineRenderer.SetPosition(1, pointAlongLine);

            if (Vector3.Distance(shipCenter.position, pointAlongLine) > inactiveGrabRadius && grabMode)
            {
                GrabObject(pointAlongLine, grabRadius);
                shipMagnet.transform.position = pointAlongLine;
            }

            //push as you move along...
            if (pushMode)
            {
                PushRadius(pointAlongLine, pushRadius);
                shipGlove.transform.position = pointAlongLine;
            }


            Vector3 myLength = Vector3.zero;
            myLength = pointAlongLine - shipCenter.position;

            //max distance
            if (Mathf.Ceil(myLength.magnitude) >= Mathf.Floor(totalDistanceToTravel) )
            {
                if (pushMode)
                {
                    shipGlove.GetComponent<BoxCollider2D>().enabled = false;
                }

                goForward = false;
            }
        }
        //Return the arm
        else
        {    
            counter -= returnSpeed /10;

            float x = Mathf.MoveTowards(0, totalDistanceToTravel, counter);
          
            //get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(finalDestination - shipCenter.position) + shipCenter.position;
            lineRenderer.SetPosition(1, pointAlongLine);

            if (pushMode)
            {
                shipGlove.transform.position = pointAlongLine;
            }
            else
            {
                shipMagnet.transform.position = pointAlongLine;

                if (grabbedObject)
                {
                    grabbedObject.transform.position = pointAlongLine;
                }
            }

            Vector3 myLength = pointAlongLine - shipCenter.position;
            if (counter < 0.1f)
            {
                goForward = true;
                grabbedObject = null;
                armActive = false;
                if (pushMode)
                {
                    shipGlove.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    shipMagnet.GetComponent<SpriteRenderer>().enabled = false;
                }
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

        Collider2D closestItem = null;
        float minDist = Mathf.Infinity;
        foreach (Collider2D c in hitColliders)
        {
            float dist = Vector3.Distance(c.gameObject.transform.position, center);

            if (dist < minDist && (c.gameObject.tag.Equals("Item") || c.gameObject.tag.Equals("Powerup")))
            {
                closestItem = c;
                minDist = dist;
                grabbedObject = closestItem.gameObject;
                //hitColliders = new Collider2D[0];
                //return the arm once it's grabbed an object
            }
        }

        if (grabbedObject)
        {
            goForward = false;

            if (grabbedObject.GetComponent<ItemController>().itemState == ItemManager.ItemState.Stuck)
            {
                grabbedObject.GetComponent<ItemController>().GrabOffPlanet(grabbedObject.GetComponent<ItemController>().HomePlanet);
            }
            else
            {
                grabbedObject.GetComponent<ItemController>().itemState = ItemManager.ItemState.Hooked;
            }
        }
    }

} 

