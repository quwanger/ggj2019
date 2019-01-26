using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
    public Vector2 direction;
    public float speed;
	public int teamId;
    public float mass;

    public float rotationSpeed;

    private GameObject[] planets;

    public ItemManager.ItemState itemState = ItemManager.ItemState.Idle;

    [SerializeField]
    private Rigidbody2D rigidbody2d;

    private Planet homePlanet = null;

    public void Setup(int _id, Vector2 _direction, float _speed, float _mass)
    {
        teamId = _id;
        direction = _direction;
        speed = _speed;
        mass = _mass;
        rotationSpeed = Random.Range(-10f, 10f);

        rigidbody2d.AddForce(direction.normalized * speed);

        foreach(GameObject p in planets)
        {
            Planet planet = p.GetComponent<Planet>();
            if (planet.planetId == teamId)
            {
                GetComponent<SpriteRenderer>().color = planet.teamColor;
                break;
            }
        }
    }
	void Start ()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        Setup(0, direction, speed, mass);
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        //if the current item is exploding or stuck, we don't care what it hits
        if(itemState == ItemManager.ItemState.Exploding || itemState == ItemManager.ItemState.Stuck)
        {
            return;
        }

        // check if it's a friendly planet
        if(col.gameObject.CompareTag("HouseBorder"))
        {
            if (itemState == ItemManager.ItemState.Atmosphere_Enemy)
            {
                //explode
                Explode();
            }
            else if (itemState == ItemManager.ItemState.Atmosphere_Friendly)
            {
                Stick(col.transform.parent.GetComponent<Planet>());
            }
        }
        //check if its hitting an item on a planet
        else if(col.gameObject.CompareTag("Item"))
        {
            //hitting another item
            ItemController item = col.gameObject.GetComponent<ItemController>();
            if(item.itemState == ItemManager.ItemState.Stuck)
            {
                if(item.teamId == teamId)
                {
                    // same team, stick item to rest of shield
                    Stick(item.homePlanet);
                }
                else
                {
                    // destroy item and item it hits
                    Explode();
                    item.Explode();
                }
            }
        }
    }

    private void Explode()
    {
        if(homePlanet) homePlanet.RemoveItemFromPlanet(this);
        Destroy(this.gameObject);
    }

    private void Stick(Planet planet)
    {
        homePlanet = planet;
        planet.AcceptItem(this);
        transform.SetParent(planet.transform);
        //rigidbody2d.simulated = false;
        rigidbody2d.bodyType = RigidbodyType2D.Static;
        itemState = ItemManager.ItemState.Stuck;
    }

    void FixedUpdate()
    {
        // only rotate is flying through space
        if(rigidbody2d.bodyType != RigidbodyType2D.Static) transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);

        foreach (GameObject planet in planets)
        {
            float dist = Vector3.Distance(planet.transform.position, transform.position);
            Planet p = planet.GetComponent<Planet>();
            if (dist <= p.MaxGravDist)
            {
                Vector3 v = planet.transform.position - transform.position;
                Vector2 gravForce = v.normalized * (1.0f - dist / p.MaxGravDist) * (p.MaxGravity/mass);
                rigidbody2d.AddForce(gravForce);
            }
        }
    }
}
