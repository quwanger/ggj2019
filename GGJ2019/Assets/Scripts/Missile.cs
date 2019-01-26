using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public PlayerController owner;
    public Vector2 direction;
    public float speed;
    public Rigidbody2D rigidbody2d;
    public float mass;

    private float maxGravDist = 20f;
    private float maxGravity = 25f;

    private float maxGravDistItems = 10f;
    private float maxGravityItems = 1f;

    void Awake()
    {
        mass = 10f;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Setup(PlayerController _owner, Vector2 _direction, float _speed)
    {
        owner = _owner;
        direction = _direction;
        speed = _speed;
        rigidbody2d.AddForce(direction.normalized * speed);
    }

    void FixedUpdate()
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

        foreach (GameObject planet in planets)
        {
            //do not use gravity for home planet
            if (owner.teamId == planet.GetComponent<Planet>().planetId) break;

            float dist = Vector3.Distance(planet.transform.position, transform.position);
            Planet p = planet.GetComponent<Planet>();
            if (dist <= maxGravDist)
            {
                Vector3 v = planet.transform.position - transform.position;
                Vector2 gravForce = v.normalized * (1.0f - (dist / maxGravDist)) * maxGravity;
                rigidbody2d.AddForce(gravForce);
            }
        }

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            float dist = Vector3.Distance(item.transform.position, transform.position);
            if (dist <= maxGravDistItems)
            {
                Vector3 v = item.transform.position - transform.position;
                Vector2 gravForce = v.normalized * (1.0f - (dist / maxGravDistItems)) * (maxGravityItems * item.GetComponent<ItemController>().mass);
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

}
