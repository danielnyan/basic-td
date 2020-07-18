using UnityEngine;

public class ChainTowerAttributes : TowerAttributes
{
    //Inherited attributes: damage, cooldown, firingRange, bulletFired, baseCost
    [SerializeField]
    private int baseNumberOfJumps;
    [SerializeField]
    private float baseJumpRange;

    private int numberOfJumps {
        get
        {
            int output = baseNumberOfJumps;
            if (GameManager.SharedInstance.NumberOfTowers == 1)
            {
                output += soleTowerBuff.jumpIncreases;
            }
            return output;
        }
    }

    private float jumpRange
    {
        get
        {
            float output = baseJumpRange;
            if (GameManager.SharedInstance.NumberOfTowers == 1)
            {
                output += soleTowerBuff.jumpRangeIncrease;
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
        bulletInstance.GetComponent<BulletDispatcher>().SetupProjectile(damage, trackedObject, numberOfJumps: numberOfJumps, jumpRange: jumpRange);
        bulletInstance.SetActive(true);
    }
}
