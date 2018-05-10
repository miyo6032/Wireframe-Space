public class PierceBullet : Bullet
{
    public int hitTimes;

    //When called, will either continue removing health of the module, or will 
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
