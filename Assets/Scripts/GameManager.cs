using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    ObstacleSpawner obstacleSpawner;

    [SerializeField]
    TubeGenerator tubeGenerator;

    [SerializeField]
    UIManager uiManager;

    [SerializeField]
    GameObject player;

    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float rotSpeed;

    public enum State
    {
        Initializing = 0,
        Running,
        Pause,
        Over,
        Count
    }

    public State gameState;

    public float distanceTravelled = 0;
    
    public int highScore;
    public int timesSpawned=0;

    float timeSinceLastWave;
    void Start()
    {
        gameState = State.Running;
        
        if (player)
        {
            player.GetComponent<player>().speed = moveSpeed;
            player.GetComponent<player>().rotSpeed = rotSpeed;
        }

        tubeGenerator.Initialize(25);
        obstacleSpawner.Initialize(15);

        StartCoroutine(ExtendTube(1));
        StartCoroutine(CropTube(20));
        StartCoroutine(RecenterTube(25));

        StartCoroutine(SpawnObstacle(1));
    }

    private void FixedUpdate()
    {
        if (gameState == State.Running)
        {
            distanceTravelled += 5 * Time.fixedDeltaTime;
            uiManager.txt_distance.text = ((int)distanceTravelled).ToString();
        }
        else
        {
            if (player)
            {
                player.GetComponent<player>().speed = 0;
                player.GetComponent<player>().rotSpeed = 0;
            }
        }
        if (moveSpeed < 6.5f)
        {
            if (timeSinceLastWave + 8 < Time.time)
            {
                moveSpeed += 0.2f;
                rotSpeed += 0.02f;

                player.GetComponent<player>().speed = moveSpeed;
                player.GetComponent<player>().rotSpeed = rotSpeed;

                timeSinceLastWave = Time.time;
            }
        }
    }


    IEnumerator ExtendTube(float interval)
    {

        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if(gameState==State.Running)
            tubeGenerator.ExtendMesh();
        }
    }

    IEnumerator CropTube(float interval)
    {

        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if (gameState == State.Running)
            {
                int pointsRemoved = tubeGenerator.CropMesh(14);

                obstacleSpawner.OnCurvePointsRemoved(pointsRemoved);
            }
        }
    }

    IEnumerator RecenterTube(float interval)
    {
        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if (gameState == State.Running)
            {
                Vector3 distance = tubeGenerator.RecenterMesh();
                RecenterObstacles(distance);
            }
        }
    }

    IEnumerator SpawnObstacle(float interval)
    {

        while (gameState == State.Running)
        {
            yield return new WaitForSeconds(interval);

            if (gameState == State.Running)
            {
                if(timesSpawned>=3)
                {
                    yield return StartCoroutine(DeSpawnObstacle());
                    timesSpawned = 0;
                }
                obstacleSpawner.SpawnObstacle();
                timesSpawned++;
            }
        }
    }

    IEnumerator DeSpawnObstacle()
    {
        if (gameState == State.Running)
        {
            obstacleSpawner.DeSpawnObstacle();
        }

        yield return new WaitForEndOfFrame();
    }

    void RecenterObstacles(Vector3 distance)
    {
        obstacleSpawner.ReCenter(distance);
    }

    void GameOver()
    {
        gameState = State.Over;
        uiManager.GameOver();
    }

    public void UnPause()
    {
        if (player)
        {
            player.GetComponent<player>().speed = moveSpeed;
            player.GetComponent<player>().rotSpeed = rotSpeed;
        }
    }

    public void PlayerDied()
    {
        GameOver();
    }
}
