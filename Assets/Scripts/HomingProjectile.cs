using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum HomingType
{
    Precise, //The bullet takes a beeline towards the enemy's transform position
    Off_Homing, //The bullet accelerates towards the enemy, making it less accurate and sometimes missing the enemy

    //These options are added purely for fun
}

public class HomingProjectile : Projectile
{
    [SerializeField]
    protected GameObject trackedEnemy;
    protected Vector3 enemyPosition;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    protected HomingType homingType = HomingType.Precise;

    //First, get a bullet from the pool, say bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet")
    //After which, setup the bullet's attributes using bullet.GetComponent<HomingProjectile>().SetupProjectile(params)
    public void SetupProjectile(float damage, GameObject trackedEnemy, List<Buff> bulletBuffs = null, HomingType homingType = HomingType.Precise, bool isPenetrating = false)
    {
        //The damage of this projectile will be assigned to the damage as specified in SetupProjectile(params)
        //This goes on for the other variables. Feel free to add more variables if you like
        this.damage = damage;
        this.trackedEnemy = trackedEnemy;
        this.homingType = homingType;
        this.isPenetrating = isPenetrating;
        this.bulletBuffs = bulletBuffs;
    }

    protected void MoveTowardsEnemy()
    {
        //Moves the bullet towards the enemy at a given speed
        Vector3 directionToEnemy = (enemyPosition - transform.position).normalized;
        transform.position += directionToEnemy * speed * Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        //If the enemy is still alive and isn't destroyed or disabled
        if (trackedEnemy != null && trackedEnemy.activeSelf)
        {
            //Track the enemy's transform position, and move towards the enemy
            enemyPosition = trackedEnemy.transform.position;
            MoveTowardsEnemy();
        }
        else
        {
            //If the enemy is dead, the bullet moves towards where the enemy was anyway. Once
            //the bullet is there, the bullet will disable.
            if (Vector3.Distance(enemyPosition, transform.position) < 0.5f)
            {
                gameObject.SetActive(false);
            }
            MoveTowardsEnemy();
        }
    }
}
