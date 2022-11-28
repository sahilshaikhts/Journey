using UnityEngine;

namespace Sahil
{
    //No collision checking
    //Just spaced accoriding to the scale of mesh
    [System.Serializable]
    public class Spawner_sequence : Spawner
    {
        /// <summary>
        /// Origin of each prop in the prop list.
        /// </summary>
        [SerializeField] Vector3[] m_propOrigin;
        Vector3[] m_lastSpawnPosition;//Array of positions of the last spawned gameobject of each type.

        Quaternion[] m_lastSpawnRotation;//Array of rotations of the last spawned gameobject of each type.

        public new void Initialize()
        {
            if (m_propOrigin.Length != m_props.Length) Debug.LogError("Missing prop spawn origin.(Need to enter atleast default Vector3(0,0,0))");
            m_lastSpawnPosition = m_propOrigin;
            m_lastSpawnRotation = new Quaternion[m_props.Length];

            base.Initialize();
        }

        public void SpawnAll()
        {
            for (int i = 0; i < m_props.Length; i++)
            {
                GameObject obj = SpawnObject(i);
                obj.transform.position = m_lastSpawnPosition[i] + m_lastSpawnRotation[i] * Vector3.forward * m_props[i].GetSpaceBetween().z;
                obj.transform.rotation = m_lastSpawnRotation[i];

                m_lastSpawnPosition[i] = obj.transform.position;
                m_lastSpawnRotation[i] = obj.transform.rotation;
            }
        }

        /// <summary>
        /// Spawns every prop next the last spawned object
        /// </summary>
        /// <param name="aSpawnDirection"> Direction of the new object from the last spawned prop.</param>
        public void SpawnAll(Vector3 aSpawnDirection)
        {
            for (int i = 0; i < m_props.Length; i++)
            {
                GameObject obj = SpawnObject(i);
                obj.transform.position = m_lastSpawnPosition[i] + aSpawnDirection * m_props[i].GetSpaceBetween().z;
                obj.transform.rotation = m_lastSpawnRotation[i];

                m_lastSpawnPosition[i] = obj.transform.position;
                m_lastSpawnRotation[i] = obj.transform.rotation;
            }
        }

        /// <summary>
        /// Spawn a prop next the last spawned object
        /// </summary>
        /// <param name="aSpawnDirection"> Direction of the new object from the last spawned prop.</param>
        /// <param name="aRotation">Rotation of the new prop.If not set ,rotation will be same as the last spawned prop.</param>
        public void SpawnProp(string aGameObjectName, Vector3 aSpawnDirection, Quaternion aRotation)
        {
            for (int i = 0; i < m_props.Length; i++)
            {
                GameObject obj = SpawnObject(i);
                obj.transform.position = m_lastSpawnPosition[i] + aSpawnDirection * m_props[i].GetSpaceBetween().z;
                obj.transform.rotation = aRotation;

                m_lastSpawnPosition[i] = obj.transform.position;
                m_lastSpawnRotation[i] = obj.transform.rotation;
            }
        }
    }
}

