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

    public ItemSpawnerController.ItemState itemState = ItemSpawnerController.ItemState.Idle;

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
        Setup(Mathf.FloorToInt(Random.Range(1,3)), new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-2, 2)), Random.Range(5, 15), Random.Range(1,5));
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        //if the current item is exploding or stuck, we don't care what it hits
        if(itemState == ItemSpawnerController.ItemState.Exploding || itemState == ItemSpawnerController.ItemState.Stuck)
        {
            return;
        }

        // check if it's a friendly planet
        if(col.gameObject.CompareTag("HouseBorder"))
        {
            if (itemState == ItemSpawnerController.ItemState.Atmosphere_Enemy)
            {
                //explode
                Explode();
            }
            else if (itemState == ItemSpawnerController.ItemState.Atmosphere_Friendly)
            {
                Stick(col.transform.parent.GetComponent<Planet>());
            }
        }
        //check if its hitting an item on a planet
        else if(col.gameObject.CompareTag("Item"))
        {
            //hitting another item
            ItemController item = col.gameObject.GetComponent<ItemController>();
            if(item.itemState == ItemSpawnerController.ItemState.Stuck)
            {
                if(item.teamId == teamId)
                {
                    // same team
                    Stick(item.homePlanet);
                }
                else
                {
                    Explode();
                }
            }
        }
    }

    private void Explode()
    {
        Destroy(this.gameObject);
    }

    private void Stick(Planet planet)
    {
        homePlanet = planet;
        planet.AcceptItem(this);
        transform.SetParent(planet.transform);
        //rigidbody2d.simulated = false;
        rigidbody2d.bodyType = RigidbodyType2D.Static;
        itemState = ItemSpawnerController.ItemState.Stuck;
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
