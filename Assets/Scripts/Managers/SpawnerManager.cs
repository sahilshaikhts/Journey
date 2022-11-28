using Sahil;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]Spawner_sequence m_sequence_spawner;
    private void Start()
    {
        m_sequence_spawner.Initialize();
    }

    public void SpawnLandscapes(Vector3 aDirection)
    {
            m_sequence_spawner.SpawnProp("Landscape_Left",Vector3.right, Quaternion.LookRotation(aDirection));
            m_sequence_spawner.SpawnProp("Landscape_Right",Vector3.right,Quaternion.LookRotation(aDirection));
    }
}
