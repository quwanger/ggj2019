using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

    private Planet homePlanet;
    public Planet HomePlanet { get { return homePlanet; } }
    void Start()
    {
        homePlanet = transform.parent.parent.GetComponent<Planet>();
        homePlanet.RegisterHouse(this);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Item"))
        {
            ItemController item = col.transform.GetComponent<ItemController>();
            if (item.teamId == homePlanet.planetId)
            {
                //it is a GOOD item
            }
            else
            {
                //it is a BAD item
                // blow up house
                item.Explode(3);
                homePlanet.DestroyHouse(this);
            }
        }
        else if(col.CompareTag("Missile"))
        {
            col.GetComponent<Missile>().Explode();
            homePlanet.DestroyHouse(this);
        }
    }
}
