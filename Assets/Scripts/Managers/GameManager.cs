using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] SpawnerManager m_spawnerManager;
    [SerializeField] MountainGenerator m_mountainGenerator;
    [SerializeField] UIManager uiManager;

    [SerializeField] float m_spawnChunkSize;//size of terrain to be generated every time

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

        m_mountainGenerator.Initialize(150, m_spawnChunkSize);

        StartCoroutine(ExtendMountain(1));
        StartCoroutine(SpawnEnvironment(.4f));
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
            {
                m_mountainGenerator.ExtendMesh(m_spawnChunkSize);
            }
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

                //m_spawnerManager.SpawnLandscapes();
            }
        }
    }

    IEnumerator DeSpawnObstacle()
    {
        if (gameState == State.Running)
        {
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
