using UnityEngine;

//The player's gun
public class Gun: MonoBehaviour {

    //Used to quantify the gun's relative power
    public int relativeFirepower;

    public float fireCooldown = 1.0f;

    //Used with fireCooldown to create a timed fireing mechanism
    private bool canFire = true;

    public float staggerLag = 0;

    public GameObject bullet;

    void Update()
    {
        //Converts the mouse position in the screen to world space.
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angle = Vector2.SignedAngle(Vector2.right, mousePosition - transform.position);

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Input.GetButton("Fire1") && canFire)
        {
            canFire = false;
            Invoke("Fire", staggerLag);
            //Will allow the player to fire after this is called in fireCooldown seconds.
            Invoke("ResetCooldown", fireCooldown);
        }
    }

    //The fire function - overriden if different bullet emmisions are desired
    protected virtual void Fire()
    {
        GameObject instance = Instantiate(bullet, transform.position, transform.rotation);
        GameObject mothership = transform.parent.parent.gameObject;
        instance.GetComponent<Bullet>().originShip = mothership;
        instance.GetComponent<Rigidbody2D>().velocity = mothership.GetComponent<Rigidbody2D>().velocity;
    }

    void ResetCooldown()
    {
        canFire = true;
    }

}
