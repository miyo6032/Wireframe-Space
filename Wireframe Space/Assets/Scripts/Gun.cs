using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun: MonoBehaviour {

    public int relativeFirepower;

    public float fireCooldown = 1.0f;

    private bool canFire = true;//Used with fireCooldown to create a timed fireing mechanism

    public float staggerLag = 0;

    public GameObject bullet;

    void Update()
    {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Converts the mouse position in the screen to world space.

        float angle = Vector2.SignedAngle(Vector2.right, mousePosition - transform.position);

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Input.GetButton("Fire1") && canFire)
        {
            Invoke("Fire", staggerLag);
            canFire = false;
            Invoke("ResetCooldown", fireCooldown);//Will allow the player to fire after this is called in fireCooldown seconds.
        }
    }

    protected virtual void Fire()
    {
        if (Input.GetButton("Fire1"))
        {
            GameObject instance = Instantiate(bullet, transform.position, transform.rotation);
            GameObject mothership = transform.parent.parent.gameObject;
            instance.GetComponent<Bullet>().originShip = mothership;
            instance.GetComponent<Rigidbody2D>().velocity = mothership.GetComponent<Rigidbody2D>().velocity;
        }
    }

    void ResetCooldown()
    {
        canFire = true;
    }

}
