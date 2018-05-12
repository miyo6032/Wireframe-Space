using UnityEngine;

public class ShipModule : MonoBehaviour {

    public SpriteRenderer hexImage;
    public int id;
    protected bool partOfPlayer = false;
    public bool invincible;
    public int health = 2;
    public Vector2 xyPos;
    public Ship ship;

    private Module stats;

    void Start()
    {
        if(hexImage == null)//So we don't have to assign the sprite renderer when we can just get it from the single child
        {
            hexImage = GetComponentInChildren<SpriteRenderer>();
        }

        stats = GameManager.instance.database.GetModuleStats(id);

        health = stats.maxHealth;
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


    public void OnCollisionEnter2D(Collision2D collision)//Handles collision with other modules
    {
        if (collision.otherCollider.GetComponent<ShipModule>())
        {
            if (health > 0)
            {
                health--;
            }
        }
        UpdateDamage();
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
        UpdateDamage();
    }

    void UpdateDamage()
    {
        if (invincible) return;
        if (health <= 0)
        {
            ship.DestroyNode(xyPos);
            if (partOfPlayer && id == GameManager.instance.database.Cockpit.id)//When the player's cockpit is destoryed
            {
                PlayZoneManager.instance.MissionFailed();
            }
        }

        //Apply broken graphics
        if (health == 1)
        {
            hexImage.sprite = stats.brokenImage2;
        }
        else if (stats.maxHealth > 2)
        {
            if (health <= 3)
            {
                hexImage.sprite = stats.brokenImage;
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
