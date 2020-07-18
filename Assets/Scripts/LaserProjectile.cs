using UnityEngine;

public class LaserProjectile : HitboxScript {

    [SerializeField]
    private GameObject trackedEnemy;

	public void SetupProjectile(float damage, GameObject trackedEnemy)
    {
        this.damage = damage;
        this.trackedEnemy = trackedEnemy;
    }

    private void OnEnable()
    {
        hasBuffs = false;

        //LaserProjectile.cs is attached to the child named "Offset" containing the mesh. Now we need to reference
        //the parent
        Transform laser = transform.parent;

        if (trackedEnemy != null)
        {
            //Strips the y component so that the laser is parallel to the ground.
            Vector3 laserDirection = trackedEnemy.transform.position - laser.position;
            laserDirection = new Vector3(laserDirection.x, 0f, laserDirection.z);
            laser.rotation = Quaternion.LookRotation(laserDirection);
        }
    }
}
