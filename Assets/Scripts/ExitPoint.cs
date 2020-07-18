using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    private List<GameObject> enemyList = new List<GameObject>();
    public List<GameObject> EnemyList { get { return enemyList; } }

    // If the enemy enters exitPoint, register that enemy
    private void OnTriggerEnter(Collider other)
    {
        enemyList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        enemyList.Remove(other.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy has been destroyed or disabled, but is somehow still in enemyList, remove that entry
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null || enemyList[i].activeSelf == false)
            {
                enemyList.RemoveAt(i);
            }
        }
    }
}
