using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipModule : MonoBehaviour {

    public SpriteRenderer hexImage;
    public Sprite brokenImage;
    public Sprite brokenImage2;
    public int id;
    public int mass;
    protected bool partOfPlayer = false;
    public bool invincible;
    public int health = 2;
    public Vector2 xyPos;
    public Vector2[] connectionPositions = { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1)};
    public Ship ship;

    protected int maxHealth;

    void Start()
    {
        if(hexImage == null)//So we don't have to assign the sprite renderer when we can just get it from the single child
        {
            hexImage = GetComponentInChildren<SpriteRenderer>();
        }

        maxHealth = health;
    }

    public void SetMothership(Ship s)//Sets the ship that the shipModule is a part of
    {
        transform.SetParent(s.transform);
        ship = s;
        if(ship.gameObject.tag == "Player")
        {
            partOfPlayer = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)//Handles bullet collisions
    {
        if (health > 0)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (other.tag == "Projectile")
            {
                if (partOfPlayer)
                {
                    if (bullet.originShip != transform.parent.gameObject)
                    {
                        bullet.Destroy(this);
                    }
                }
                else
                {
                    if (PlayZoneManager.instance.player)
                        if (bullet.originShip == PlayZoneManager.instance.player.gameObject)
                        {
                            bullet.Destroy(this);
                        }
                }
            }
        }
        if (invincible) return;
        if (health <= 0)
        {
            ship.DestroyNode(xyPos);
            if (partOfPlayer && id == GameManager.instance.database.Cockpit.representativeModule.id)//When the player's cockpit is destoryed
            {
                PlayZoneManager.instance.MissionFailed();
            }
        }

        //Apply broken graphics
        if(health == 1)
        {
            hexImage.sprite = brokenImage2;
        }
        else if(maxHealth > 2)
        {
            if(health <= 3)
            {
                hexImage.sprite = brokenImage;
            }
        }

    }


    void OnDestroy()
    {
        if (PlayZoneManager.instance.smallExplosion != null)
        {
            PlayZoneManager.instance.smallExplosion.transform.position = transform.position;
            PlayZoneManager.instance.smallExplosion.Emit(1);
        }
    }

}
