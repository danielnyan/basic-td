using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{
    public GameObject trackedEnemy;
    public Vector3 offset;
    private float baseHealth;
    private float currentHealth;
    private Image filler;

    // Use this for initialization
    private void OnEnable()
    {
        //Gets the image component in the child gameObject named "Fill"
        filler = transform.Find("Fill").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the tracked enemy isn't destroyed or disabled, reflect the enemy's health.
        if (trackedEnemy != null && trackedEnemy.activeSelf)
        {
            baseHealth = trackedEnemy.GetComponent<EnemyAttributes>().BaseHealth;
            currentHealth = trackedEnemy.GetComponent<EnemyAttributes>().CurrentHealth;

            //Sets the fill amount to the percentage of enemy health left
            filler.fillAmount = currentHealth / baseHealth;

            //Positions the healthbar on top of the enemy
            transform.position = Camera.main.WorldToScreenPoint(trackedEnemy.transform.position + offset);
        }
        else
        {
            //Else, disable the healthbar and return it to the object pool
            transform.SetParent(ObjectPooler.SharedInstance.transform);
            gameObject.SetActive(false);
        }
    }
}
