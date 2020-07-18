using System.Collections.Generic;
using UnityEngine;

//Credits to Mark Placzek: https://www.raywenderlich.com/136091/object-pooling-unity
//This version finds the object by tag and returns it.

//To use the object pooler, specify the number of distinct items to pool under "Items to pool", then add
//the gameObject objectToPool, specify amountToPool, and whether the pool should expand if the number drawn
//exceeds the number of objects in the pool. Don't touch the "Pooled objects" row; it's there purely for debugging
//purposes to see which objects are already in the pool.

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand;
}

public class ObjectPooler : MonoBehaviour
{
    //Allows you to get the objects to pool by using ObjectPooler.SharedInstance, rather than using
    //GameObject.Find() to find the scene's object pooler
    public static ObjectPooler SharedInstance;
    private List<GameObject> pooledObjects;
    [SerializeField]
    private List<ObjectPoolItem> itemsToPool;

    void Awake()
    {
        if (SharedInstance == null) SharedInstance = this;
        else if (SharedInstance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        if (transform.childCount == 0)
        {
            //Initialises pooledObjects as an empty list of GameObjects.
            pooledObjects = new List<GameObject>();

            //Instantiates every object that's supposed to be pooled, disables them, and makes them a 
            //child of the gameObject containing this script to keep the hierarchy view neat.
            foreach (ObjectPoolItem item in itemsToPool)
            {
                for (int i = 0; i < item.amountToPool; i++)
                {
                    GameObject obj = Instantiate(item.objectToPool);
                    obj.transform.SetParent(transform);
                    obj.SetActive(false);

                    //After the object is instantiated, tells pooledObjects that this object is available
                    pooledObjects.Add(obj);
                }
            }
        }
    }

    //Searches through all the items in pooledObjects, a list of objects that are pooled but dormant. 
    //If the object is disabled (i.e. not currently active) and has a certain tag, that object is returned.
    public GameObject GetPooledObject(string tag)
    {
        for (int i = pooledObjects.Count - 1; i >= 0; i--)
        {
            if (pooledObjects[i] == null)
            {
                pooledObjects.RemoveAt(i);
            }
            else if (!pooledObjects[i].activeInHierarchy)
            {
                if (pooledObjects[i].CompareTag(tag))
                {
                    //If the object is disabled and the tag checks out, the object is returned and the function breaks
                    return pooledObjects[i];
                }
            }
        }

        //If there are no objects that are inactive and matches the tag, this bit is ran
        //Searches through all the unique items that are pooled and check if the tag matches
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                //If the tag matches, and the object pool should expand (since all the pooled objects are already 
                //currently being used), then a new object with the tag is instantiated, and that object is returned
                if (item.shouldExpand)
                {
                    GameObject obj = Instantiate(item.objectToPool);
                    obj.transform.SetParent(SharedInstance.transform);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }

        //If there are no objects with the specified tag, or if the object pool for that object should not expand,
        //then nothing is returned.
        return null;
    }

    public void DisableAllObjects()
    {
        foreach (Transform child in SharedInstance.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}