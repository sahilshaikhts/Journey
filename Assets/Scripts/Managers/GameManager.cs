using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Spawner_environment m_spawner_enviornment;
    [SerializeField] MountainGenerator m_mountainGenerator;
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


        m_mountainGenerator.Initialize(150);
        //  m_spawner_enviornment.Initialize(15);

        StartCoroutine(ExtendMountain(.4f));
        //StartCoroutine(CropMountainMesh(5));
        //StartCoroutine(RecenterMountain(15));
        //StartCoroutine(SpawnEnvironment(.4f));

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


    IEnumerator ExtendMountain(float interval)
    {

        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if (gameState == State.Running)
                m_mountainGenerator.ExtendMesh();
        }
    }

    IEnumerator CropMountainMesh(float interval)
    {

        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if (gameState == State.Running)
            {
                int pointsRemoved = m_mountainGenerator.CropMesh(14);
            }
        }
    }

    IEnumerator RecenterMountain(float interval)
    {
        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if (gameState == State.Running)
            {
                Vector3 distance = m_mountainGenerator.RecenterMesh();
                RecenterObstacles(distance);
            }
        }
    }

    IEnumerator SpawnEnvironment(float interval)
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
                //m_spawner_enviornment.Spawn();
                timesSpawned++;
            }
        }
    }

    IEnumerator DeSpawnObstacle()
    {
        if (gameState == State.Running)
        {
            m_spawner_enviornment.DeSpawnObstacle();
        }

        yield return new WaitForEndOfFrame();
    }

    void RecenterObstacles(Vector3 distance)
    {
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
