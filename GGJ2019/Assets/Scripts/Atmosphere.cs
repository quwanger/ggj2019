using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour {

    private Planet homePlanet;
    public Planet HomePlanet { get { return homePlanet; } }

    void Start()
    {
        homePlanet = transform.parent.GetComponent<Planet>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Item"))
        {
            ItemController item = col.transform.GetComponent<ItemController>();
            if (item.teamId == homePlanet.planetId)
            {
                //it is a GOOD item
                item.itemState = ItemManager.ItemState.Atmosphere_Friendly;
            }
            else
            {
                //it is a BAD item
                item.itemState = ItemManager.ItemState.Atmosphere_Enemy;
            }
        }
    }
}
