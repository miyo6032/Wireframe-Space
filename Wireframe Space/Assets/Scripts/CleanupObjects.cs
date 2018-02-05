using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to clean up bullets in the play zone
public class CleanupObjects : MonoBehaviour {

	void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Projectile")
        {
            Destroy(other.gameObject);
        }
    }

}
