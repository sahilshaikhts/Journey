using UnityEngine;

public class Spawner_environment : MonoBehaviour
{
    [SerializeField] Prop[] m_props;

    ObjectPool[] obstacle;

    public void Initialize(int initSpawnCount)
    {
    }

    void Spawn()
    {

    }

    public void DeSpawnObstacle()
    {
        //Get list on enabled object,check from bottom for which are behind player,from that index to 0 request to push and disable;
        for (int i = 0; i < obstacle.Length; i++)
        {
            GameObject[] activeObst = obstacle[i].GetActiveObjects();

            for (int k = 0; k < activeObst.Length; k++)
            {
                //obstacle[i].DeactivateObject(activeObst[k]);
            }
        }
    }

}
[System.Serializable]
public struct Prop
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3Range offset;
    [SerializeField] IntRange quantity;

    [Header("______________________________________________________________________________")]
    [Header("SPAWN IN SERIES\n")]
    [SerializeField] bool spawnInSeries;
    [SerializeField] Vector3 distanceBetween;
}

[System.Serializable]
public struct Vector3Range
{
    [SerializeField] Vector3 min,max;

    public Vector3 GetRandomValue()=>new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.y, max.y));
}

[System.Serializable]
public struct IntRange
{
    [SerializeField] int min, max;

    public int GetRandomValue() => Random.Range(min, max);
}

