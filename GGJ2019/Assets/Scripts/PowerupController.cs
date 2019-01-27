using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : ItemController
{
    public ItemManager.Powerups powerup;

    public GameObject explosion;

    public override void Setup(int _id, Vector2 _direction, float _speed, float _mass)
    {
        direction = _direction;
        speed = _speed;
        rotationSpeed = Random.Range(-10f, 10f);
        mass = _mass;

        rigidbody2d.AddForce(direction.normalized * speed);

        StartCoroutine(removeIfOutOBounds());
    }

    public void SetPowerupType(ItemManager.Powerups powerupType)
    {
        powerup = powerupType;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        TriggerPowerup(col);
    }

    public void TriggerPowerup(Collision2D col)
    {
        switch(powerup)
        {
            case ItemManager.Powerups.Asteroid:
                TriggerAsteroid(col);
                break;
            case ItemManager.Powerups.Star:
                TriggerStar(col);
                break;
            case ItemManager.Powerups.Missile:
                TriggerMissile(col);
                break;
            default:
                break;
        }
    }

    private float asteroidExplodingForce = 500f;

    private void TriggerAsteroid(Collision2D col)
    {
        bool triggerExplosion = false;
        if (col.gameObject.CompareTag("Item"))
        {
            if (col.gameObject.GetComponent<ItemController>().itemState == ItemManager.ItemState.Stuck)
            {
                triggerExplosion = true;
            }
        }
        else if(col.gameObject.CompareTag("House") || col.gameObject.CompareTag("HouseBorder"))
        {
            triggerExplosion = true;
        }

        if (!triggerExplosion) return;

        transform.GetComponent<CircleCollider2D>().enabled = false;

        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        Transform closestPlanet = transform;
        float minDistance = Mathf.Infinity;
        foreach (GameObject p in planets)
        {
            float distanceBetweenThisAndPlanet = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(p.transform.position.x, p.transform.position.y));
            if (distanceBetweenThisAndPlanet < minDistance)
            {
                minDistance = distanceBetweenThisAndPlanet;
                closestPlanet = p.transform;
            }
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        for (int a = 0; a < hitColliders.Length; a++)
        {
            if (hitColliders[a].gameObject.tag.Equals("Item"))
            {
                ItemController item = hitColliders[a].GetComponent<ItemController>();
                if (item.itemState == ItemManager.ItemState.Stuck)
                {
                    item.ExplodeOffPlanet(item.HomePlanet);
                }
            }
        }

        for (int a = 0; a < hitColliders.Length; a++)
        {
            if (hitColliders[a].gameObject.tag.Equals("Item") || hitColliders[a].gameObject.tag.Equals("Powerup"))
            {
                Vector2 itemDirection = new Vector2(hitColliders[a].transform.position.x - closestPlanet.position.x, hitColliders[a].transform.position.y - closestPlanet.position.y);
                hitColliders[a].GetComponent<Rigidbody2D>().AddForce(itemDirection.normalized * asteroidExplodingForce);
            }
        }

        GameManager.instance.shaker.TriggerShake(0.5f, 0.8f);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void TriggerStar(Collision2D col)
    {
        //Debug.Log("Powerup Triggered: Star");
    }

    private void TriggerMissile(Collision2D col)
    {
        //Debug.Log("Powerup Triggered: Missile");
        if(col.gameObject.CompareTag("HouseBorder"))
        {
            col.transform.parent.GetComponent<Planet>().GetMissiles(1);
            Destroy(this.gameObject);
        }
    }
}
