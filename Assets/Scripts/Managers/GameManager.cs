using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] ObstacleSpawner obstacleSpawner;
    [SerializeField] MountainGenerator tubeGenerator;
    [SerializeField] UIManager uiManager;

    [SerializeField] float moveSpeed;

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
    public int timesSpawned = 0;

    float timeSinceLastWave;
    void Start()
    {
        gameState = State.Running;


        tubeGenerator.Initialize(150);
        obstacleSpawner.Initialize(10);

        StartCoroutine(ExtendTube(.4f));
        //StartCoroutine(CropTube(5));
        //StartCoroutine(RecenterTube(15));

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
        }
        if (moveSpeed < 6.5f)
        {
            if (timeSinceLastWave + 8 < Time.time)
            {
                moveSpeed += 0.2f;
                timeSinceLastWave = Time.time;
            }
        }
    }


    IEnumerator ExtendTube(float interval)
    {

        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if (gameState == State.Running)
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
                if (timesSpawned >= 3)
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
    }

    public void PlayerDied()
    {
        GameOver();
    }
}
