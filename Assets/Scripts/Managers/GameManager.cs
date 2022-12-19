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

        m_mountainGenerator.Initialize(Random.Range(0,1000));
        m_spawnerManager.Initialize();
        
        ExtendMountain(m_spawnChunkSize*5);
        StartCoroutine(ExtendMountainInInterval(2));
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

    IEnumerator ExtendMountainInInterval(float interval)
    {
        while (gameState != State.Over)
        {
            yield return new WaitForSeconds(interval);
            if (gameState == State.Running)
            {
                ExtendMountain(m_spawnChunkSize);
            }
        }
    }
    void ExtendMountain(float aSpawnChunkSize)
    {
        Vector3[] newPoints = m_mountainGenerator.ExtendMesh(aSpawnChunkSize);
        m_spawnerManager.PopulateArea(newPoints);
        
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

    void SpawnObstacle()
    {
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
