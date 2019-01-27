using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public PlayerController owner;
    public Vector2 direction;
    public float speed;
    public Rigidbody2D rigidbody2d;
    public float mass;
    public float teamId;

    public bool useItemGravity = false;

    private float maxGravDist = 15f;
    private float maxGravity = 50f;

    private float maxGravDistItems = 10f;
    private float maxGravityItems = 2f;

    public GameObject explosion;

    //private AudioManager audioManager;

    void Awake()
    {
        mass = 10f;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        //audioManager = FindObjectOfType<AudioManager>();
    }

    public void Setup(PlayerController _owner, Vector2 _direction, float _speed, int _teamId, Color _color)
    {
        //audioManager.PlaySound("missiles");
        owner = _owner;
        direction = _direction;
        speed = _speed;
        teamId = _teamId;
        rigidbody2d.AddForce(direction.normalized * speed);
        GetComponent<SpriteRenderer>().color = _color;
    }

    void FixedUpdate()
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

        foreach (GameObject planet in planets)
        {
            //do not use gravity for home planet
            if (owner.teamId != planet.GetComponent<Planet>().planetId)
            {
                float dist = Vector3.Distance(planet.transform.position, transform.position);
                Planet p = planet.GetComponent<Planet>();
                if (dist <= maxGravDist)
                {
                    Vector3 v = planet.transform.position - transform.position;
                    Vector2 gravForce = v.normalized * (1.0f - (dist / maxGravDist)) * maxGravity;
                    rigidbody2d.AddForce(gravForce);
                }
            }
        }

        if (useItemGravity)
        {
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            foreach (GameObject item in items)
            {
                float dist = Vector3.Distance(item.transform.position, transform.position);
                if (dist <= maxGravDistItems && item.GetComponent<ItemController>().itemState != ItemManager.ItemState.Stuck)
                {
                    Vector3 v = item.transform.position - transform.position;
                    Vector2 gravForce = v.normalized * (1.0f - (dist / maxGravDistItems)) * (maxGravityItems * item.GetComponent<ItemController>().mass);
                    rigidbody2d.AddForce(gravForce);
                }
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

        Vector2 moveDirection = rigidbody2d.velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        bool explode = true;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 3f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if(hitColliders[i].CompareTag("Item") || hitColliders[i].CompareTag("Missile"))
            {
                hitColliders[i].GetComponent<Rigidbody2D>().AddForce((hitColliders[i].transform.position - transform.position) * 100f);
            }
            
            i++;
        }


        if (col.gameObject.CompareTag("Item"))
        {
            if (col.gameObject.GetComponent<ItemController>().itemState != ItemManager.ItemState.Stuck)
            {
                col.gameObject.GetComponent<ItemController>().Explode(3);
            }
            else
            {
                if (col.gameObject.GetComponent<ItemController>().teamId != teamId)
                {
                    col.gameObject.GetComponent<ItemController>().Explode(3);
                }
                else
                {
                    explode = false;
                }
            }

        }
        else if (col.gameObject.CompareTag("Missile"))
        {
            col.gameObject.GetComponent<Missile>().Explode();
        }

        if (explode) Explode();
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

}
