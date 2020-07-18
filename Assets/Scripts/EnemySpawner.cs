using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnBounds; //This cuboid must have a BoxCollider component which is a trigger
    [SerializeField]
    private GameObject endPoint; //The target position where the enemies go. Has the same specifications as spawnBounds
    [SerializeField]
    private float baseSpawnTime;
    private float currentSpawnTime;


    //Obtains the boundary of a box collider and returns a random point within that boundary
    private Vector3 GetRandomPoint(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponent<Collider>();

        //Finds the bounds of the collider
        float minX = collider.bounds.min.x;
        float maxX = collider.bounds.max.x;
        float minY = collider.bounds.min.y;
        float maxY = collider.bounds.max.y;
        float minZ = collider.bounds.min.z;
        float maxZ = collider.bounds.max.z;

        //Returns a random location within the bounds of the collider
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
    }

    public GameObject SpawnEnemy(string enemyTag, GameObject spawnBounds = null, GameObject endPoint = null)
    {
        //If null is provided for either spawnBounds or endPoint, then the innate values in the script
        //are used instead. This will not modify the innate values of spawnBounds and endPoint in the script
        if (spawnBounds == null)
        {
            spawnBounds = this.spawnBounds;
        }
        if (endPoint == null)
        {
            endPoint = this.endPoint;
        }

        Vector3 spawnLocation = GetRandomPoint(spawnBounds);
        Vector3 targetLocation = GetRandomPoint(endPoint);

        //Draws the enemy from the object pool
        GameObject enemyInstance = ObjectPooler.SharedInstance.GetPooledObject(enemyTag);
        if (enemyInstance != null)
        {
            enemyInstance.transform.position = spawnLocation;

            //Initialises the look direction, while eliminating the difference in height (i.e. y-axis)
            Vector3 lookDirection = targetLocation - spawnLocation;
            lookDirection = new Vector3(lookDirection.x, 0, lookDirection.z);

            //Initialises the enemy to look at the target location.
            enemyInstance.transform.rotation = Quaternion.LookRotation(lookDirection);
            enemyInstance.SetActive(true);

            //Allocates a health bar to the enemy
            HealthbarSpawn.instance.AllocateHealthbar(enemyInstance);

            //Returns the enemy so that the enemy's attributes can be set up by other functions, if necessary.
            return enemyInstance;
        }
        else return null;
    }

    private void OnEnable()
    {
        currentSpawnTime = baseSpawnTime;
        hasSpawnedBoss = false;
    }

    //This handles the spawning of enemies, as well as which enemies are spawned at what difficulty levels.
    //The events are currently hardcoded for now within the Update() function.
    private bool hasSpawnedBoss;

    private void Update()
    {
        //Fetches the difficulty level from the game manager
        float difficultyLevel = GameManager.SharedInstance.DifficultyLevel;

        if (currentSpawnTime <= 0f)
        {
            //Returns a random number between 0 and 1
            float randomNumber = Random.value;

            //The spawn mechanics are hardcoded here. It varies according to what's the current difficulty level
            if (difficultyLevel < 2f)
            {
                if (randomNumber < 0.8f)
                {
                    //Spawns the first enemy in the list of enemies to spawn, i.e. Type A, with a 80% chance
                    GameObject enemy = SpawnEnemy("Type A");

                    //Reset current spawn time such that spawn time for Type A increases exponentially with difficulty level
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                }
                else
                {
                    //Setups and spawns a Type B enemy with a 20% chance
                    GameObject enemy = SpawnEnemy("Type B");
                    currentSpawnTime = baseSpawnTime;
                }
            }
            else if (difficultyLevel < 3f)
            {
                if (randomNumber < 0.5f)
                {
                    //Spawns the first enemy in the list of enemies to spawn, i.e. Type A, with a 50% chance
                    GameObject enemy = SpawnEnemy("Type A");
                    //Reset current spawn time such that spawn time for Type A increases exponentially with difficulty level
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                }
                else if (randomNumber >= 0.5f && randomNumber < 0.8f)
                {
                    //Setups and spawns a Type B enemy with a 20% chance
                    GameObject enemy = SpawnEnemy("Type B");
                    currentSpawnTime = baseSpawnTime;
                }
                else
                {
                    //Spawns a Type C enemy with a 20% chance
                    GameObject enemy = SpawnEnemy("Type C");
                    //Reset current spawn time such that spawn time for Type C increases exponentially with difficulty level
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                }
            }
            else if (difficultyLevel < 3.2f)
            {
                if (!hasSpawnedBoss)
                {
                    //Spawns the boss enemy, Type D, fourth element of the list
                    GameObject enemy = SpawnEnemy("Type D");
                    hasSpawnedBoss = true;
                }
                else if (hasSpawnedBoss)
                {
                    //Locks the current spawn time, preventing spawning for this period
                    currentSpawnTime = baseSpawnTime;
                }
            }
            else if (difficultyLevel < 4f)
            {
                hasSpawnedBoss = false;
                if (randomNumber < 0.5f)
                {
                    //Spawns the first enemy in the list of enemies to spawn, i.e. Type A, with a 50% chance
                    GameObject enemy = SpawnEnemy("Type A");
                    //Reset current spawn time such that spawn time for Type A increases exponentially with difficulty level
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                }
                else if (randomNumber >= 0.5f && randomNumber < 0.8f)
                {
                    //Setups and spawns a Type B enemy with a 20% chance
                    GameObject enemy = SpawnEnemy("Type B");
                    currentSpawnTime = baseSpawnTime;
                }
                else
                {
                    //Spawns a Type C enemy with a 20% chance
                    GameObject enemy = SpawnEnemy("Type C");
                    //Reset current spawn time such that spawn time for Type C increases exponentially with difficulty level
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                }
            }
            else if (difficultyLevel < 4.2f)
            {
                if (!hasSpawnedBoss)
                {
                    //Spawns the boss enemy, Type D, fourth element of the list
                    GameObject enemy = SpawnEnemy("Type D");
                    hasSpawnedBoss = true;
                }
                else if (hasSpawnedBoss)
                {
                    //Generates other enemies on top of the boss
                    if (randomNumber < 0.5f)
                    {
                        //Spawns the first enemy in the list of enemies to spawn, i.e. Type A, with a 50% chance
                        GameObject enemy = SpawnEnemy("Type A");
                        //Reset current spawn time such that spawn time for Type A increases exponentially with difficulty level
                        currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                    }
                    else if (randomNumber >= 0.5f && randomNumber < 0.8f)
                    {
                        //Setups and spawns a Type B enemy with a 20% chance
                        GameObject enemy = SpawnEnemy("Type B");
                        currentSpawnTime = baseSpawnTime;
                    }
                    else
                    {
                        //Spawns a Type C enemy with a 20% chance
                        GameObject enemy = SpawnEnemy("Type C");//Reset current spawn time such that spawn time for Type C increases exponentially with difficulty level
                        currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                    }
                }
            }
            else if (difficultyLevel < 5f)
            {
                hasSpawnedBoss = false;
                if (randomNumber < 0.4f)
                {
                    //Spawns the first enemy in the list of enemies to spawn, i.e. Type A, with a 40% chance
                    GameObject enemy = SpawnEnemy("Type A");
                    //Reset current spawn time such that spawn time for Type A increases exponentially with difficulty level
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                }
                else if (randomNumber >= 0.4f && randomNumber < 0.7f)
                {
                    //Setups and spawns a Type B enemy with a 30% chance
                    GameObject enemy = SpawnEnemy("Type B");
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel - 4f, 2f));
                }
                else
                {
                    //Spawns a Type C enemy with a 30% chance
                    GameObject enemy = SpawnEnemy("Type C");
                    //Reset current spawn time such that spawn time for Type C increases exponentially with difficulty level
                    currentSpawnTime = baseSpawnTime / (0.5f + Mathf.Pow(difficultyLevel, 2));
                }
            }
            else
            {
                if (randomNumber < 0.4f)
                {
                    //Spawns the first enemy in the list of enemies to spawn, i.e. Type A, with a 40% chance
                    GameObject enemy = SpawnEnemy("Type A");
                    //Makes the Type A enemy spawn virtually instantaneously
                    currentSpawnTime = 0.05f;
                }
                else if (randomNumber >= 0.4f && randomNumber < 0.6f)
                {
                    //Setups and spawns a Type B enemy with a 30% chance
                    GameObject enemy = SpawnEnemy("Type B");
                    currentSpawnTime = 0.5f;
                }
                else if (randomNumber >= 0.6f && randomNumber < 0.99f)
                {
                    //Spawns a Type C enemy with a 29% chance
                    GameObject enemy = SpawnEnemy("Type C");
                    //Reset current spawn time such that spawn time for Type C increases exponentially with difficulty level
                    currentSpawnTime = 0.05f;
                }
                else
                {
                    //Setups and spawns a Type D enemy with a 1% chance
                    GameObject enemy = SpawnEnemy("Type D");
                    currentSpawnTime = baseSpawnTime;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //If the current spawn time is greater than 0, i.e. cooldown is in effect, let it count down to zero.
        if (currentSpawnTime > 0f)
        {
            currentSpawnTime -= Time.fixedDeltaTime;
        }
    }
}
