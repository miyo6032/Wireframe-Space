using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceBullet : Bullet
{
    public int hitTimes;

    public override void Destroy(ShipModule mod)
    {
        while (mod.health > 0) {
            mod.health--;
            hitTimes--;
            if (hitTimes <= 0)
            {
                Destroy(gameObject);
                break;
            }
        }
    }

}
