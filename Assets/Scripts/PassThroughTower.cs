using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughTower : MonoBehaviour
{
    private Collider collide;

    private void OnCollisionEnter(Collision collision)
    {
        //If the object hits another object and that object happens to be a tower, ignore collision between
        //this object and the tower
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tower"))
        {
            Physics.IgnoreCollision(collide, collision.collider);
        }
        //Also ignores collisions with walls
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            Physics.IgnoreCollision(collide, collision.collider);
        }
    }

    private void OnEnable()
    {
        collide = GetComponent<Collider>();
    }
}
