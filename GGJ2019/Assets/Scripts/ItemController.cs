using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
    public Vector2 direction;
    public float speed;
	public int teamId;
    public float mass;

    public float rotationSpeed;

    [SerializeField]
    private Rigidbody2D rigidbody2d;

    public void Setup(int _id, Vector2 _direction, float _speed, float _mass)
    {
        teamId = _id;
        direction = _direction;
        speed = _speed;
        mass = _mass;
        rotationSpeed = Random.Range(-10f, 10f);

        rigidbody2d.AddForce(direction.normalized * speed);
    }
	void Start ()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");

        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        Setup(0, direction, speed, mass);
	}

    GameObject[] planets;

    void FixedUpdate()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);

        foreach (GameObject planet in planets)
        {
            float dist = Vector3.Distance(planet.transform.position, transform.position);
            Planet p = planet.GetComponent<Planet>();
            if (dist <= p.maxGravDist)
            {
                Vector3 v = planet.transform.position - transform.position;
                Vector2 gravForce = v.normalized * (1.0f - dist / p.maxGravDist) * (p.maxGravity/mass);
                rigidbody2d.AddForce(gravForce);
            }
        }
    }
}
