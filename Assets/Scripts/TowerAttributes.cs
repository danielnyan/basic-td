using System.Collections.Generic;
using UnityEngine;

public class TowerAttributes : MonoBehaviour
{
    [SerializeField]
    protected GameObject bulletHolder;
    [SerializeField]
    protected float baseDamage;
    [SerializeField]
    protected float baseCooldown; //Base time between each bullet fire
    protected float currentCooldown;
    [SerializeField]
    protected float baseFiringRange;
    [SerializeField]
    protected GameObject bulletFired; //Holds a copy of the bullet so that it can be obtained through object pooling
    [SerializeField]
    protected int baseCost;
    [SerializeField]
    protected SoleTowerBuff soleTowerBuff;

    protected float damage
    {
        get
        {
            float output = baseDamage;
            if (GameManager.SharedInstance.NumberOfTowers == 1)
            {
                output += soleTowerBuff.damageIncrease;
            }
            return output;
        }
    }

    protected float cooldown
    {
        get
        {
            float output = baseCooldown;
            if (GameManager.SharedInstance.NumberOfTowers == 1)
            {
                output += soleTowerBuff.cooldownIncrease;
            }
            return output;
        }
    }

    protected float firingRange
    {
        get
        {
            float output = baseFiringRange;
            if (GameManager.SharedInstance.NumberOfTowers == 1)
            {
                output += soleTowerBuff.rangeIncrease;
            }
            return output;
        }
    }

    public int BaseCost { get { return baseCost; } }

    // Use this for initialization
    private void OnEnable()
    {
        currentCooldown = 0f;
    }

    protected List<Collider> GetEnemiesInRange(float radius)
    {
        //Draws a sphere around the tower with radius equal to the firing range
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, radius);
        //Initialises the output value
        List<Collider> output = new List<Collider>();

        for (int i = 0; i < objectsInRange.Length; i++)
        {
            //If the objects hit by OverlapSphere are indeed enemies, they are added into the list enemiesInRange
            if (objectsInRange[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                output.Add(objectsInRange[i]);
            }
        }
        return output;
    }

    protected GameObject GetLastEnemy(List<Collider> enemiesInRange)
    {
        //This function is meant to be used in conjunction with GetEnemiesInRange.
        //It returns the enemy which is closest to reaching the opposite side of the screen

        //Initialises the process with the first element of the list Enemies in Range
        GameObject enemy = enemiesInRange[0].gameObject;
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            //For each enemy in the list, check how far they've progressed. Here, progress is defined
            //as the z position of the enemy, though you can modify this to make it more sophisticated
            if (enemiesInRange[i].transform.position.z > enemy.transform.position.z)
            {
                enemy = enemiesInRange[i].gameObject;
            }
        }
        return enemy;
        //At the end of the process, we would have an enemy which has travelled the furthest distance
    }

    protected virtual void FireBulletAtEnemy(GameObject trackedObject)
    {
        //Gets a copy of the bullet from the object pooler
        GameObject bulletInstance = ObjectPooler.SharedInstance.GetPooledObject(bulletFired.tag);
        //Then, the bullet is repositioned to the firing indicator's position
        bulletInstance.transform.position = bulletHolder.transform.position;
        //Finally, the bullet's attributes are set up and the bullet is enabled and ready to go
        bulletInstance.GetComponent<BulletDispatcher>().SetupProjectile(damage, trackedObject);
        bulletInstance.SetActive(true);
    }

    //The Update function handles cooldown, and fires a bullet if the cooldown is zero and there are enemies in range
    private void Update()
    {
        //If the current cooldown reaches zero, i.e. the tower can fire, the tower checks if there is an enemy in range
        if (currentCooldown <= 0f)
        {
            //Gets the list of enemies in range. This list can be an empty list if there are no enemies
            List<Collider> enemiesInRange = GetEnemiesInRange(firingRange);

            //If there are enemies in range, the enemy closest to the opposite side of the screen is selected
            if (enemiesInRange.Count > 0)
            {
                GameObject enemy = GetLastEnemy(enemiesInRange);
                FireBulletAtEnemy(enemy);
                currentCooldown = cooldown;
            }
            //If there are no enemies in range, the stuff in the block earlier will not be executed so that the
            //tower does not fire bullets at nothing
        }
        //And obviously the tower cannot fire if the current cooldown is not zero.
    }

    private void FixedUpdate()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }
    }
}
