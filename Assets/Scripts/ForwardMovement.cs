using UnityEngine;

public class ForwardMovement : MonoBehaviour
{
    //Simple forward movement for Type A enemies
    protected Rigidbody rb;
    protected float speed;
    protected Quaternion lookDirection;
    [SerializeField]
    protected float angleCorrectionRate; //The rate of rotation when the Type A enemy goes off course

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        
        //Memorises the direction in which the enemy was spawned. This is because for Type A enemies, the
        //enemy is spawned looking towards the target.
        lookDirection = transform.rotation;
    }

    private void Update()
    {
        //Speed is updated constantly because buffs and debuffs may change the value
        speed = GetComponent<EnemyAttributes>().Speed;
    }

    private void FixedUpdate()
    {
        //Allows the enemy to move forward at a constant speed while keeping the upwards velocity dynamic, allowing
        //the enemy to be affected by gravity.
        rb.velocity = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * speed + 
            new Vector3(0, rb.velocity.y, 0);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, angleCorrectionRate * Time.fixedDeltaTime);
    }

    //By the way, the enemy doesn't navigate around towers and whatnot. I know that NavMesh and the 
    //NavMeshAgent library is a thing, but due to the scope of the project, I've decided not to implement it
}
