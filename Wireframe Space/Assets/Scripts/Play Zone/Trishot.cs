using UnityEngine;

public class Trishot : Gun {


    public float spreadDegree;

    protected override void Fire()//Shoots 3 bullets in a spread shot
    {
        if (Input.GetButton("Fire1"))
        {
            GameObject instance = Instantiate(bullet, transform.position, transform.rotation);

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Converts the mouse position in the screen to world space.
            float angle = Vector2.SignedAngle(Vector2.right, mousePosition - transform.position);
            Quaternion spreadRotation = Quaternion.AngleAxis(angle + spreadDegree, Vector3.forward);
            Quaternion spreadRotation2 = Quaternion.AngleAxis(angle - spreadDegree, Vector3.forward);
            GameObject instance2 = Instantiate(bullet, transform.position, spreadRotation);
            GameObject instance3 = Instantiate(bullet, transform.position, spreadRotation2);

            GameObject mothership = transform.parent.parent.gameObject;
            instance.GetComponent<Bullet>().originShip = mothership;
            instance.GetComponent<Rigidbody2D>().velocity = mothership.GetComponent<Rigidbody2D>().velocity;
            instance2.GetComponent<Bullet>().originShip = mothership;
            instance2.GetComponent<Rigidbody2D>().velocity = mothership.GetComponent<Rigidbody2D>().velocity;
            instance3.GetComponent<Bullet>().originShip = mothership;
            instance3.GetComponent<Rigidbody2D>().velocity = mothership.GetComponent<Rigidbody2D>().velocity;
        }
    }

}
