using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarSpawn : MonoBehaviour {

    public static HealthbarSpawn instance;
    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private Vector3 offset;

	// Use this for initialization
	void Awake () {
        instance = this;
	}
	
	public void AllocateHealthbar(GameObject enemy)
    {
        //Get a healthbar and set it as a child of the healthbar canvas.
        GameObject healthBarInstance = ObjectPooler.SharedInstance.GetPooledObject(healthBar.tag);
        healthBarInstance.transform.SetParent(transform);

        //Setup some of the public variables of the health bar, such as offset and tracked enemy
        healthBarInstance.GetComponent<HealthbarScript>().trackedEnemy = enemy;
        healthBarInstance.GetComponent<HealthbarScript>().offset = offset;
        healthBarInstance.SetActive(true);
    }

    public void DeallocateHealthbar()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
