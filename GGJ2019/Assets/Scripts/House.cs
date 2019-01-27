using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

    private Planet homePlanet;

    private AudioManager audioManager;

    public GameManager.HouseState houseState = GameManager.HouseState.Alive;

    public Planet HomePlanet { get { return homePlanet; } }
    void Start()
    {
        homePlanet = transform.parent.parent.GetComponent<Planet>();
        homePlanet.RegisterHouse(this);

        audioManager = FindObjectOfType<AudioManager>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Item"))
        {
            ItemController item = col.transform.GetComponent<ItemController>();
            if (item.itemState == ItemManager.ItemState.NoCollide) return;
            if (item.teamId == homePlanet.planetId)
            {
                //it is a GOOD item
            }
            else
            {
                //it is a BAD item
                // blow up house
                item.Explode(3);

                if(houseState == GameManager.HouseState.Alive) {
                    DestroyHouse();
                }
            }
        }
        else if(col.CompareTag("Missile"))
        {
            col.GetComponent<Missile>().Explode();
            if(houseState == GameManager.HouseState.Alive) {
                DestroyHouse();
            }
        }
    }

    public void DestroyHouse() {
        GameManager.instance.shaker.TriggerShake(0.25f);
        houseState = GameManager.HouseState.Destroyed;
        SpriteRenderer spriteR = this.GetComponent<SpriteRenderer>();
        Object[] rubbleSprites = Resources.LoadAll("Rubble", typeof(Sprite));
        int randomIndex = Random.Range(0, rubbleSprites.Length);

        audioManager.PlaySound("explosions");

        spriteR.color = homePlanet.teamColor;

        spriteR.sprite = (Sprite)rubbleSprites[randomIndex];

        homePlanet.DestroyHouse(this);
    }
}
