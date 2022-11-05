using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
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

            offsetFromCenter = new Vector3(0, Random.Range(-1, 1), Random.Range(-10, 10));

            position = tube.path[spawnIndex] + offsetFromCenter;

            direction =new Vector3(0,0,Random.Range(10,35));

            rotation = Quaternion.Euler(direction);

        }

        //Pipe cross
        if (index == 1)
        {
            float radiansFromPoint = Random.Range(0, 6.28f);

            position = tube.path[spawnIndex];

            direction = new Vector3(0, Mathf.Sin(radiansFromPoint), Mathf.Cos(radiansFromPoint));

            rotation = Quaternion.LookRotation(direction);

        }

        //Fence
        if (index == 2 || index == 3)
        {
            position = tube.path[spawnIndex];

            if (spawnIndex + 1 > tube.path.Length - 1)
                direction = Vector3.right;
            else
                direction = tube.path[spawnIndex] - tube.path[spawnIndex + 1];

            rotation = Quaternion.LookRotation(direction);
        }

        GameObject obj = obstacle[index].Spawn(position, rotation);

        if (index == 2 || index == 3)
        {
            if (sceneManager.distanceTravelled > 200 && Random.Range(0, 10) > 4)
            {
                if(obj.GetComponent<Rigidbody>()==false)
                {
                    obj.AddComponent<Rigidbody>();
                }

                obj.GetComponent<Rigidbody>().useGravity = false;
                obj.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(0.3f, 1.2f), 0, 0);
            }
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
