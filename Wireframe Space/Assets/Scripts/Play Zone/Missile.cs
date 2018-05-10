using UnityEngine;

//The missle bullet type - handles area damage explosions
public class Missile : Bullet {

    public float ExplosionRadius;

    Collider2D col;

    bool exploded = false;

    protected override void Start()
    {
        base.Start();
        col = GetComponent<Collider2D>();
    }

    //Calculates the area damage in the area
    //the function get called once, and then again to destroy it
    public override void Destroy(ShipModule mod)
    {

        mod.health--;

        if (exploded)
        {
            Destroy(gameObject);
            return;
        }

        exploded = true;

        //Finds all ship modules in a certain radius
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

        foreach(Collider2D collision in collisions)
        {
            if (collision.GetComponent<ShipModule>() && collision.GetComponent<ShipModule>() != mod)
            {
                collision.GetComponent<ShipModule>().OnTriggerEnter2D(col);
            }
        }

        PlayZoneManager.instance.missileExplosion.transform.position = transform.position;
        PlayZoneManager.instance.missileExplosion.Emit(1);

    }

}
