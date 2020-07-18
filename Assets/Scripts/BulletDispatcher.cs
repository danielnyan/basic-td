using UnityEngine;

public class BulletDispatcher : MonoBehaviour
{

    //Under TowerAttributes.cs, there is a field where you can select the bullet that TowerAttributes fires
    //However, it may not be certain what component is attached to that bullet. As such, this script would 
    //make things more convenient because all TowerAttributes.cs has to do is reference the BulletDispatcher.cs
    //script and call SetupProjectile, then the dispatcher will get the respective component. The component is 
    //specified by attaching this script to a projectile, and then typing the desired component under the field 
    //bulletType

    [SerializeField]
    private string bulletType;

    public void SetupProjectile(float damage, GameObject trackedEnemy, float splashDamage = 0f, 
        float debuffDuration = 0f, float splashRange = 0f, float movementSpeedMultiplier = 0f, 
        int numberOfJumps = 0, float jumpRange = 0f)
    {
        if (bulletType == "HomingProjectile")
        {
            GetComponent<HomingProjectile>().SetupProjectile(damage, trackedEnemy);
        }
        else if (bulletType == "LaserProjectile")
        {
            //The laser projectile script is found in the child object called "Offset"
            GetComponentInChildren<LaserProjectile>().SetupProjectile(damage, trackedEnemy);
        }
        else if (bulletType == "SplashProjectile")
        {
            GetComponent<SplashProjectile>().SetupProjectile(damage: damage, splashDamage: splashDamage, trackedEnemy: trackedEnemy, debuffDuration: debuffDuration, splashRange: splashRange, movementSpeedMultiplier: movementSpeedMultiplier);
        }
        else if (bulletType == "ChainProjectile")
        {
            GetComponent<ChainProjectile>().SetupProjectile(damage: damage, trackedEnemy: trackedEnemy, numberOfJumps: numberOfJumps, jumpRange: jumpRange);
        }
    }
}
