using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Holds the basic parameters so that other projectiles (such as HomingProjectile.cs) can be derived from
    //this script

    [SerializeField]
    protected float damage;
    [SerializeField]
    protected GameObject hitEffect;
    [SerializeField]
    protected bool isPenetrating = false;
    [SerializeField]
    protected List<Buff> bulletBuffs;

    //Exposes the damage parameter to other scripts, but makes that parameter read-only
    public float Damage { get { return damage; } }
    public GameObject HitEffect { get { return hitEffect; } }
    public bool IsPenetrating { get { return isPenetrating; } }
    public List<Buff> BulletBuffs { get { return bulletBuffs; } }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.gameObject.GetComponent<EnemyAttributes>().TakeBullet(gameObject);
            InstantiateHitEffect(HitEffect, other.transform.position, other.transform.rotation);
            //Disables the bullet that hit the enemy once damage is dealt, if the bullet is not penetrating
            if (!IsPenetrating)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected virtual void InstantiateHitEffect(GameObject effect, Vector3 position, Quaternion rotation)
    {
        if (effect != null)
        {
            //Gets a copy of the hit effect from the object pool
            GameObject effectInstance = ObjectPooler.SharedInstance.GetPooledObject(effect.tag);
            //Moves the hit effect to a given position and rotation
            effectInstance.transform.position = position;
            effectInstance.transform.rotation = rotation;
            effectInstance.SetActive(true);

            //The particle effect will be killed in another script called KillParticles
        }
    }
}
