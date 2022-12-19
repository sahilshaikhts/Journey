using UnityEngine;

public class ObjectPool
{
    private GameObject m_prefab;

    private GameObject[] m_spawned_objects;
    
    int lastIndex;//points to the element that was last pulled and activated(end index for active objects (0..lastIndex))

    public void Initialize(GameObject aPrefab, int aCount)
    {
        m_prefab = aPrefab;

        m_spawned_objects = new GameObject[aCount];

        lastIndex = -1;

        for (int i = 0; i < aCount; i++)
        {
            GameObject obj = MonoBehaviour.Instantiate(aPrefab);
            obj.SetActive(false);
            m_spawned_objects[i] = obj;
        }
    }

    public GameObject PullObjectFromPool()
    {
        //Shift the lastIndex by one and return that element in the array to activate the gameObject
        if (lastIndex < m_spawned_objects.Length - 1)
        {
            lastIndex++;
         
            return m_spawned_objects[lastIndex];
        }
        else
            return null;

    }

    public GameObject[] DeSpawnObjects(int count)        //Sort the pool_object and point to the new avaiblable object
    {
        if (count > 0)
        {
            GameObject[] requestedObjects = new GameObject[count];

            //store the list of objects in a temporary array of GOs
            for (int i = 0; i < count; i++)
            {
                requestedObjects[i] = m_spawned_objects[i];
                requestedObjects[i].SetActive(false);
            }
          
            //Shift the GOs that were not requested to be deactivated to the top 
            for (int i = count, k = 0; i < m_spawned_objects.Length; i++, k++)
            {
                m_spawned_objects[k] = m_spawned_objects[i];
            }
            //Move the stored GOs that were requested at the bottom of the array.
            for (int i = m_spawned_objects.Length-count, k = 0; i < m_spawned_objects.Length; i++, k++)
            {
                m_spawned_objects[i] = requestedObjects[k];
            }

            lastIndex -= count;
            return requestedObjects;
        }
        return null;
    }

    public void DeSpawnObject(GameObject aObject)        //Find the requested object in the active region of the array and deactivates it 
    {
        for (int i = 0; i < lastIndex; i++)
        {
            //find the requested object and swap it with the object before the lastindex and then move the lastIndex by -1

            if (m_spawned_objects[i] == aObject)
            {
                GameObject swapObj = m_spawned_objects[i];

                swapObj.SetActive(false);

                m_spawned_objects[i] = m_spawned_objects[lastIndex];
                m_spawned_objects[lastIndex] = swapObj;

                lastIndex--;
             
                break;
            }

        
        }
    }
    
    public GameObject Spawn(Vector3 aPosition, Quaternion aRotation)
    {
        GameObject obj = PullObjectFromPool();
        
        if (obj == null)
            return null;
        
        obj.transform.position = aPosition;
        obj.transform.rotation = aRotation;

        obj.SetActive(true);


        return obj;
    }

    public GameObject GetLastSpawnObject()
    {
        if (lastIndex!=-1)
            return m_spawned_objects[lastIndex].gameObject;
        else
            return null;
    }
    public GameObject[] GetActiveObjects()
    {
        GameObject[] enabledObj=new GameObject[lastIndex+1];
        for (int i = 0; i <= lastIndex; i++)
        {
            enabledObj[i] = m_spawned_objects[i];
        }

        return enabledObj;
    }

    public GameObject[] GetObjects()
    {
        GameObject[] enabledObj = new GameObject[m_spawned_objects.Length];
        for (int i = 0; i < m_spawned_objects.Length; i++)
        {
            enabledObj[i] = m_spawned_objects[i];
        }

        return enabledObj;
    }

    public int GetTotalCount()
    {
        return m_spawned_objects.Length;
    }

    public int GetActiveObjectCount()
    {
        return lastIndex;
    }

    public bool IsObjectAvailableToSpawn() => lastIndex != m_spawned_objects.Length - 1;
}
