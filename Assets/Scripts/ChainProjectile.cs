using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainProjectile : HomingProjectile
{
    private float jumpRange;
    private int numberOfJumps;
    private Vector3 initialPosition;
    [SerializeField]
    private GameObject chainEffect;

    public void SetupProjectile(float damage, GameObject trackedEnemy, int numberOfJumps, float jumpRange)
    {
        this.damage = damage;
        this.trackedEnemy = trackedEnemy;
        this.numberOfJumps = numberOfJumps;
        this.jumpRange = jumpRange;
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

    protected GameObject GetNearestEnemy(List<Collider> enemiesInRange)
    {
        //This function is meant to be used in conjunction with GetEnemiesInRange.

        //Initialises the process with the first element of the list Enemies in Range
        GameObject enemy = enemiesInRange[0].gameObject;
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            //For each enemy in the list, check the distance between the projectile and the enemy and
            //see if this distance is the closest one
            if (Vector3.Distance(transform.position, enemiesInRange[i].transform.position) <
                Vector3.Distance(transform.position, enemy.transform.position))
            {
                enemy = enemiesInRange[i].gameObject;
            }
        }
        return enemy;
    }

    private void OnEnable()
    {
        initialPosition = transform.position;
    }

    // Creates the lightning particle for the chain effect
    protected virtual void InstantiateChainEffect()
    {
        //Gets a copy of the hit effect from the object pool
        GameObject effectInstance = ObjectPooler.SharedInstance.GetPooledObject(chainEffect.tag);
        //Moves the hit effect to a given position and rotation
        effectInstance.transform.position = Vector3.zero;
        effectInstance.GetComponent<SetupLightning>().Setup(initialPosition, transform.position);
        effectInstance.SetActive(true);

        //The particle effect will be killed in another script called KillParticles
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //Checks if the enemy that's hit was the target enemy
        if (other.gameObject == trackedEnemy)
        {
            //Deals damage to the enemy that was hit
            other.GetComponent<EnemyAttributes>().TakeBullet(gameObject, false);
            InstantiateChainEffect();

            //Check if the bullet still has any jumps remaining
            if (numberOfJumps > 0)
            {
                //Checks if there are any enemies for the chain bullet to jump to
                List<Collider> enemiesInRange = GetEnemiesInRange(jumpRange);
                
                //Iterates through the list to remove the enemy that the bullet just hit 
                //Otherwise, the bullet will hit the same enemy multiple times in a row
                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    if (enemiesInRange[i].gameObject == trackedEnemy)
                    {
                        enemiesInRange.RemoveAt(i);
                        break;
                    }
                }

                if (enemiesInRange.Count > 0)
                {
                    //Gets the point of contact, which is probably also the bullet's current position
                    Vector3 contactPoint = transform.position;

                    //Gets a copy of the chain effect, which is a copy of the projectile itself
                    GameObject chainEffectInstance = ObjectPooler.SharedInstance.GetPooledObject(gameObject.tag);

                    //Get the enemy that the chain is going to jump towards
                    GameObject nextEnemy = GetNearestEnemy(enemiesInRange);

                    //Sets up the effects of the chain effect such that the number of jumps is reduced by 1
                    chainEffectInstance.GetComponent<ChainProjectile>().SetupProjectile(damage, nextEnemy, numberOfJumps - 1, jumpRange);

                    //...and produces the chain effect at that location
                    InstantiateHitEffect(chainEffectInstance, contactPoint, Quaternion.identity);
                }
            }
            gameObject.SetActive(false);
        }
    }
}
