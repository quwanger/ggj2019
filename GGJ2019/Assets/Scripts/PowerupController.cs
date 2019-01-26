using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : ItemController
{
    public ItemManager.Powerups powerup;

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
            case ItemManager.Powerups.Moon:
                TriggerMoon(col);
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

    private void TriggerAsteroid(Collision2D col)
    {
        //Debug.Log("Powerup Triggered: Asteroid");
    }

    private void TriggerMoon(Collision2D col)
    {
        //Debug.Log("Powerup Triggered: Moon");
    }

    private void TriggerStar(Collision2D col)
    {
        //Debug.Log("Powerup Triggered: Star");
    }

    private void TriggerMissile(Collision2D col)
    {
        //Debug.Log("Powerup Triggered: Missile");
    }
}
