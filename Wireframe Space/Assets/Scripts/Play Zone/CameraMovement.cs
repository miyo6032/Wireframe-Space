using UnityEngine;

//The camera movement
public class CameraMovement : MonoBehaviour
{

    private Transform player;
    public float dampTime = 0.1f;
    private Camera c;

    void Awake()
    {
        player = PlayZoneManager.instance.player.transform;
        //The weird math here is to fix any ship rotations in the editor
        transform.rotation = Quaternion.Euler(0, 0, player.transform.eulerAngles.z - 90 + GameManager.instance.player.direction);
        c = gameObject.GetComponent<Camera>();
    }

    void FixedUpdate()//Smooth camera follow and rotation
    {
        if (player)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);

            transform.rotation = Quaternion.Euler(0, 0, player.transform.eulerAngles.z - 90 + GameManager.instance.player.direction);

            //Scrolling zooms in or out
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                c.orthographicSize = Mathf.Clamp(c.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * 2, 7, 12);
            }

        }

    }

}