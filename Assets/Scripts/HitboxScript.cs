using System.Collections.Generic;
using UnityEngine;

public class HitboxScript : Projectile
{
    //This script is catered towards objects with a collider which is a trigger, i.e. objects can pass through
    //the collider. This is ideal for bullets with splash effects
    [SerializeField]
    protected bool hasBuffs = true;
    [SerializeField]
    private float scale;

    protected override void OnTriggerEnter(Collider enemy)
    {
        if (enemy.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemy.GetComponent<EnemyAttributes>().TakeBullet(gameObject, hasBuffs);
        }
    }

    public void SetupHitboxScript(float damage, float scale = 1, List<Buff> bulletBuffs = null, bool isPenetrating = true)
    {
        this.damage = damage;
        this.scale = scale;
        this.isPenetrating = isPenetrating;

        if (bulletBuffs == null || bulletBuffs.Count == 0)
        {
            hasBuffs = false;
        }
        else
        {
            this.bulletBuffs = bulletBuffs;
            hasBuffs = true;
        }
    }

    private void OnEnable()
    {
        //When the splash effect is enabled, rescale the size
        transform.localScale = new Vector3(scale, scale, scale);
        hitEffect = null;
    }
}
