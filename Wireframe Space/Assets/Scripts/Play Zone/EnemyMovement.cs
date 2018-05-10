using UnityEngine;

//The enemy ai
public class EnemyMovement : MonoBehaviour
{
    public float rotationFactor = 1f;

    public float followRange = 60;

    public float rotationOffset;

    private Rigidbody2D rb;

    private Ship enemyShip;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyShip = GetComponent<Ship>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayZoneManager.instance.player && Vector2.Distance(enemyShip.transform.position, PlayZoneManager.instance.player.transform.position) < followRange)
        {
            //Get direction need to face
            Vector3 desiredDirection = PlayZoneManager.instance.player.transform.position - transform.position;

            //Get current facing direction
            Vector2 currentFacingRotation = new Vector2(Mathf.Cos((rb.rotation + rotationOffset) * Mathf.Deg2Rad), Mathf.Sin((rb.rotation + rotationOffset) * Mathf.Deg2Rad));

            //The angle between the direction and the desired direction
            float signedRotation = Vector2.SignedAngle(currentFacingRotation, desiredDirection);

            //A dampening effect to make it look like the enemy is more reactionary! SWEET
            float rotationMagnitude = Mathf.Clamp(Mathf.Abs(signedRotation) * Mathf.Deg2Rad, 0, 1);

            rb.AddTorque((Mathf.Sign(signedRotation) * rotationFactor * enemyShip.rotationTorque * rotationMagnitude));

            rb.AddForce((currentFacingRotation * enemyShip.moveSpeed));
        }

    }
}