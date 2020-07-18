using UnityEngine;

public class DirectionChangingMovement : ForwardMovement
{

    // Variables inherited from ForwardMovement: speed, rb, lookDirection, angleCorrectionRate
    [SerializeField]
    private GameObject endPoint;
    [SerializeField]
    private float baseDirectionChangeTimer;
    private float currentDirectionChangeTimer;

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

    // Use this for initialization
    void OnEnable()
    {
        endPoint = GameObject.Find("Playing Field").transform.Find("Exit Point").gameObject;
        rb = GetComponent<Rigidbody>();
        currentDirectionChangeTimer = baseDirectionChangeTimer;
    }

    // Update is called once per frame
    void Update()
    {
        //speed here refers to the speed variable inherited from ForwardMovement
        speed = GetComponent<EnemyAttributes>().Speed;

        if (currentDirectionChangeTimer <= 0f)
        {
            //Obtains a new direction to move towards by selecting a random point in the end point
            Vector3 targetPosition = GetRandomPoint(endPoint);
            Vector3 newLookDirection = targetPosition - transform.position;
            //Strips the y-component of newLookDirection
            newLookDirection = new Vector3(newLookDirection.x, 0, newLookDirection.z);
            //Replaces lookDirection as found in ForwardMovement.cs with the new direction, in quaternion form
            lookDirection = Quaternion.LookRotation(newLookDirection);

            //Reset current direction change timer
            currentDirectionChangeTimer = baseDirectionChangeTimer;
        }
    }

    private void FixedUpdate()
    {
        if (currentDirectionChangeTimer > 0f)
        {
            currentDirectionChangeTimer -= Time.fixedDeltaTime;
        }

        //Allows the enemy to move forward at a constant speed while keeping the upwards velocity dynamic, allowing
        //the enemy to be affected by gravity.
        rb.velocity = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * speed +
            new Vector3(0, rb.velocity.y, 0);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, angleCorrectionRate * Time.fixedDeltaTime);
    }
}
