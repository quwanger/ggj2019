using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
    public Vector2 direction;
    public float speed;
	public int teamId;
    public float mass;

    public float rotationSpeed;

    protected GameObject[] planets;

    public ItemManager.ItemState itemState = ItemManager.ItemState.Idle;

    [SerializeField]
    protected Rigidbody2D rigidbody2d;

    [SerializeField]
    private int itemTier;

    private Planet homePlanet = null;
    public Planet HomePlanet { get { return homePlanet; } }

    protected IEnumerator removeIfOutOBounds() {
        while(true) {
            if (homePlanet) {
                yield return new WaitForSeconds(3f);
            }

            Vector3 itemPosition = transform.position;

            Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 topRightOfScreen = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
            Vector3 bottomLeftOfScreen = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

            if (itemPosition.y > topRightOfScreen.y + 2 ||
                itemPosition.y < bottomLeftOfScreen.y - 2||
                itemPosition.x > topRightOfScreen.x + 2||
                itemPosition.x < bottomLeftOfScreen.x - 2) {
                DestroyItem();
            } 

            yield return new WaitForSeconds(3f);
        }
    }

    public virtual void Setup(int _id, Vector2 _direction, float _speed, float _mass)
    {
        teamId = _id;
        direction = _direction;
        speed = _speed;
        rotationSpeed = Random.Range(-10f, 10f);
        mass = itemTier * itemTier;

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

        StartCoroutine (removeIfOutOBounds());
    }
	void Awake ()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
	}

    protected virtual void OnCollisionEnter2D(Collision2D col)
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
                Explode(3);
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
                    if (itemState != ItemManager.ItemState.Idle)
                    {
                        // same team, stick item to rest of shield
                        Stick(item.homePlanet);
                    }
                }
                else
                {
                    // destroy item and item it hits
                    int currentItemTier = itemTier;
                    Explode(item.itemTier);
                    item.Explode(currentItemTier);
                }
            }
        }
    }

    public virtual void Explode(int targetLevel)
    {
        itemTier -= targetLevel;

        if (itemTier <= 0)
        {
            if (homePlanet) homePlanet.RemoveItemFromPlanet(this);
            DestroyItem();
            Instantiate(GameManager.instance.ItemManager.itemExplosion, transform.position, Quaternion.identity);
        }
    }

    public void DestroyItem() {
        GameManager.instance.ItemManager.items.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    private void Stick(Planet planet)
    {
        homePlanet = planet;
        planet.AcceptItem(this);
        transform.SetParent(planet.transform);
        rigidbody2d.bodyType = RigidbodyType2D.Static;
        itemState = ItemManager.ItemState.Stuck;
    }

    public void GrabOffPlanet(Planet planet)
    {
        itemState = ItemManager.ItemState.Hooked;
        homePlanet = null;
        planet.RemoveItemFromPlanet(this);
        transform.SetParent(null);
        StartCoroutine(ReenableCollisions());
    }

    IEnumerator ReenableCollisions()
    {
        yield return new WaitForSeconds(0.5f);
        rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
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
                Vector2 gravForce = v.normalized * (1.0f - (dist / p.MaxGravDist)) * (p.MaxGravity*mass);
                rigidbody2d.AddForce(gravForce);
            }
        }

        GameObject[] moons = GameObject.FindGameObjectsWithTag("Moon");
        foreach (GameObject moon in moons)
        {
            float dist = Vector3.Distance(moon.transform.position, transform.position);
            Moon m = moon.GetComponent<Moon>();
            if (dist <= m.maxGravDist)
            {
                Vector3 v = moon.transform.position - transform.position;
                Vector2 gravForce = v.normalized * (1.0f - (dist / m.maxGravDist)) * (m.maxGravity * mass);
                rigidbody2d.AddForce(gravForce);
            }
        }
    }

    public void Push(Vector2 direction)
    {
        rigidbody2d.AddForce(direction);
    }
}
