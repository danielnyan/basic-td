using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashProjectile : HomingProjectile
{
    //Variables inherited from HomingProjectile: damage, trackedEnemy, homingType, isPenetrating
    [SerializeField]
    private float splashRange;
    [SerializeField]
    private float movementSpeedMultiplier;
    [SerializeField]
    private float splashDamage;
    [SerializeField]
    private float debuffDuration;
    [SerializeField]
    private string debuffName;

    public void SetupProjectile(float damage, float splashDamage, GameObject trackedEnemy, float debuffDuration = 0f, float splashRange = 3f, float movementSpeedMultiplier = 1f, HomingType homingType = HomingType.Precise, bool isPenetrating = false)
    {
        this.damage = damage;
        this.splashDamage = splashDamage;
        this.trackedEnemy = trackedEnemy;
        this.homingType = homingType;
        this.isPenetrating = isPenetrating;
        this.splashRange = splashRange;
        this.movementSpeedMultiplier = movementSpeedMultiplier;
        this.debuffDuration = debuffDuration;
    }


    protected override void InstantiateHitEffect(GameObject effect, Vector3 position, Quaternion rotation)
    {
        if (effect != null)
        {
            //Gets a copy of the hit effect from the object pool
            GameObject effectInstance = ObjectPooler.SharedInstance.GetPooledObject(effect.tag);
            
            //Setup the hitbox script of the splash effect
            HitboxScript splash = effectInstance.GetComponent<HitboxScript>();
            Buff splashBuff = new Buff(name: debuffName, duration: debuffDuration, speedMultiplier: movementSpeedMultiplier);
            splash.SetupHitboxScript(splashDamage, splashRange, new List<Buff>() { splashBuff });
            //Moves the hit effect to a given position and rotation
            effectInstance.transform.position = position;
            effectInstance.transform.rotation = rotation;
            effectInstance.SetActive(true);

            //The particle effect will be killed in another script called KillParticles
        }
    }

    protected override void FixedUpdate()
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
                // Spawn a nova here
                InstantiateHitEffect(hitEffect, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }
            MoveTowardsEnemy();
        }
    }
}
