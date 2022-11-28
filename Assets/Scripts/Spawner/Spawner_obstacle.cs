using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_obstacle : MonoBehaviour
{
    [SerializeField]
     GameManager sceneManager;

    [SerializeField]
    GameObject[] prfb_obstacle;

    [SerializeField]
    ObjectPool[] obstacle;

    [SerializeField]
    int[] obstacleSpawnWeights;//Weights in increasing order

    [SerializeField]
    Tube tube;

    [SerializeField]
    GameObject player;

    int spawnIndex = 0;

    public void Initialize(int initSpawnCount)
    {
        tube = GameObject.FindGameObjectWithTag("Tube").GetComponent<Tube>();

        if (prfb_obstacle.Length > 0)
            obstacle = new ObjectPool[prfb_obstacle.Length];

        for (int i = 0; i < prfb_obstacle.Length; i++)
        {
            obstacle[i].Initialize(prfb_obstacle[i], 30);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            SpawnObstacle();
        }

    }

    void SpawnObstacle(int index)
    {
        int distanceFromLastSpawnIndex = Random.Range(9, 20);

        if (spawnIndex + distanceFromLastSpawnIndex > tube.path.Length - 1)
        {
            return;
        }
        else
            spawnIndex += distanceFromLastSpawnIndex;

        Vector3 position = new Vector3(), direction;
        Quaternion rotation = new Quaternion();

        //Pipe 
        if (index == 0)
        {
            Vector3 offsetFromCenter;

            offsetFromCenter = new Vector3(0, Random.Range(-2, -1.5f), Random.Range(-10, 10));

            position = tube.path[spawnIndex] + offsetFromCenter;

            direction = new Vector3(0, 0, Random.Range(10, 35));

            rotation = Quaternion.Euler(direction);

        }

        //Pipe cross
        if (index == 1)
        {
            position = tube.path[spawnIndex] + new Vector3(Random.Range(55, 45), Random.Range(-25, -15), 0);

            rotation = Quaternion.LookRotation((tube.path[spawnIndex + 1] - tube.path[spawnIndex]).normalized);

            obstacle[index].Spawn(position, rotation);

            position = tube.path[spawnIndex] + new Vector3(Random.Range(55, 45), Random.Range(-25, -15), 0);

            rotation = Quaternion.LookRotation((tube.path[spawnIndex + 1] - tube.path[spawnIndex]).normalized);

            obstacle[index].Spawn(position, rotation);
        }

    }

    public void DeSpawnObstacle()
    {
        //Get list on enabled object,check from bottom for which are behind player,from that index to 0 request to push and disable;
        for (int i = 0; i < obstacle.Length; i++)
        {
            GameObject[] activeObst = obstacle[i].GetActiveObjects();

            for (int k = 0; k < activeObst.Length; k++)
            {
                if (activeObst[k].transform.position.x < player.transform.position.x - 2)
                {
                   obstacle[i].DeactivateObject(activeObst[k]);
                }

            }
        }
    }

    public void SpawnObstacle()
    {
        int index = GetWeightedRandomIndex(obstacleSpawnWeights);

        if(index!=-1)
        SpawnObstacle(index);
    }

    public void ReCenter(Vector3 distance)
    {
        for (int i = 0; i < obstacle.Length; i++)
        {
            GameObject[] objects = obstacle[i].GetObjects();
            for (int j = 0; j < objects.Length; j++)
            {
                objects[j].transform.position += distance;
            }
        }
    }

    public void OnCurvePointsRemoved(int pointsRemoved)
    {
        spawnIndex -= pointsRemoved;
    }
    
    int GetWeightedRandomIndex(int[] aWeights)//aWeights in increasing order
    {
        int totalWeight = 0;
        for (int i = 0; i < aWeights.Length; i++)
        {
            totalWeight += aWeights[i];
        }

        int RandValue = Random.Range(0, totalWeight);
        for (int i = 0; i < aWeights.Length; i++)
        {
            if(RandValue<aWeights[i])
            {
                return i;
            }
            RandValue -= aWeights[i];
        }

        return -1;
    }

}
