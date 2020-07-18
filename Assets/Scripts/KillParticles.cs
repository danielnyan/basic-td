using UnityEngine;

public class KillParticles : MonoBehaviour
{

    [SerializeField]
    private float defaultLifetime = 1f;
    private float remainingLifetime = 0f;

    void OnEnable()
    {
        remainingLifetime = defaultLifetime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Disables the particle after some time has elapsed to save space
        if (remainingLifetime < 0f)
        {
            gameObject.SetActive(false);
        }
        else remainingLifetime -= Time.fixedDeltaTime;
    }
}
