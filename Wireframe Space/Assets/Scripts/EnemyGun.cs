using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{

    public float fireCooldown = 1.0f;

    public float range = 20f;

    public float aimTime = 90f;

    public float staggerLag;

    private bool canFire = true;//Used with fireCooldown to create a timed fireing mechanism

    public GameObject bullet;

    public Ship target;

    void Start()
    {
        target = PlayZoneManager.instance.player;
    }

    void Update()
    {
        if (target)
        {
            float angle = Vector2.SignedAngle((Vector2)transform.position - (Vector2)target.transform.position, Vector2.left);

            Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.back);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * aimTime);

            if (canFire && Vector2.Distance(target.transform.position, transform.position) < range)
            {
                Invoke("Fire", staggerLag);
                canFire = false;
                Invoke("ResetCooldown", fireCooldown);//Will allow the player to fire after this is called in fireCooldown seconds.
            }
        }
    }

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

