using System.Collections.Generic;
using UnityEngine;

public class SlowTowerAttributes : TowerAttributes
{
    //Inherited attributes: damage, cooldown, firingRange, bulletFired, baseCost
    [SerializeField]
    private float baseSplashRange, movementSpeedMultiplier, splashDamage, debuffDuration;
    [SerializeField]
    private GameObject nova;

    private float splashRange
    {
        get
        {
            float output = baseSplashRange;
            if (GameManager.SharedInstance.NumberOfTowers == 1)
            {
                output += soleTowerBuff.splashRangeIncrease;
            }
            return output;
        }
    }

    protected override void FireBulletAtEnemy(GameObject trackedObject)
    {
        //Gets a copy of the bullet from the object pooler
        GameObject bulletInstance = ObjectPooler.SharedInstance.GetPooledObject(bulletFired.tag);
        //Then, the bullet is repositioned to the firing indicator's position
        bulletInstance.transform.position = bulletHolder.transform.position;
        //Finally, the bullet's attributes are set up and the bullet is enabled and ready to go
        bulletInstance.GetComponent<BulletDispatcher>().SetupProjectile(damage, trackedObject, splashDamage: splashDamage, debuffDuration: debuffDuration, splashRange: splashRange, movementSpeedMultiplier: movementSpeedMultiplier);
        bulletInstance.SetActive(true);

        //Gets a copy of the bullet from the object pooler
        GameObject effectInstance = ObjectPooler.SharedInstance.GetPooledObject(nova.tag);
        //Then, the bullet is repositioned to the firing indicator's position
        effectInstance.transform.position = transform.position;
        //Setup the hitbox script of the splash effect
        HitboxScript splash = effectInstance.GetComponent<HitboxScript>();
        Buff splashBuff = new Buff(name: "Slow", duration: debuffDuration, speedMultiplier: movementSpeedMultiplier);
        splash.SetupHitboxScript(splashDamage, splashRange, new List<Buff>() { splashBuff });
        effectInstance.SetActive(true);
    }
}
