using UnityEngine;

//The bullet object
public class Bullet : MonoBehaviour {

    private Rigidbody2D rb;

    public float bulletSpeed = 1.0f;

    public GameObject originShip;

    //Send the bullet in the correct direction
    protected virtual void Start () {

        rb = GetComponent<Rigidbody2D>();

        float bulletRotation = transform.rotation.eulerAngles.z;

        Vector2 rotation = new Vector2(Mathf.Cos(bulletRotation * Mathf.Deg2Rad), Mathf.Sin(bulletRotation * Mathf.Deg2Rad));//Gets a vector 2 from rotation

        rb.velocity += rotation * bulletSpeed;

    }

    //Called by the ship module when collided with
    public virtual void Destroy(ShipModule mod)
    {
        mod.health--;
        Destroy(gameObject);
    }
	
}
