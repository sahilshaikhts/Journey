using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.MaterialProperty;

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
        public new void Initialize()
        {
            base.Initialize();
        }

        public void SpawnAll()
        {
            string propType;
            for (int i = 0; i < m_props.Length; i++)
            {
                propType = m_props[i].GetPropName();
                GameObject obj = SpawnObject(propType);

                obj.transform.position = (Vector3)GetLastSpawnedPropPosition(propType) + ((Quaternion)GetLastSpawnedPropRotation(propType) * Vector3.forward);
                obj.transform.position *= m_props[i].GetSpaceBetween().z;

                obj.transform.rotation = (Quaternion)GetLastSpawnedPropRotation(propType);
            }
        }

        /// <summary>
        /// Spawns every prop next the last spawned object
        /// </summary>
        /// <param name="aSpawnDirection"> Direction of the new object from the last spawned prop.</param>
        public void SpawnAll(Vector3 aSpawnDirection)
        {
            string propType;
            for (int i = 0; i < m_props.Length; i++)
            {
                propType = m_props[i].GetPropName();
                GameObject obj = SpawnObject(propType);

                obj.transform.position = (Vector3)GetLastSpawnedPropPosition(propType) + aSpawnDirection;
                obj.transform.position *= m_props[i].GetSpaceBetween().z;

                obj.transform.rotation = (Quaternion)GetLastSpawnedPropRotation(propType);
            }
        }

        /// <summary>
        /// Spawn a prop next the last spawned object
        /// </summary>
        /// <param name="aSpawnDirection"> Direction of the new object from the last spawned prop.</param>
        /// <param name="aRotation">Rotation of the new prop.If not set ,rotation will be same as the last spawned prop.</param>
        public void SpawnProp(string aGameObjectName, Vector3 aSpawnPosition, Quaternion aRotation)
        {
            Vector3? lastSpawnPosition = GetLastSpawnedPropPosition(aGameObjectName);
            Prop prop = GetPropByName(aGameObjectName);

            if (lastSpawnPosition.HasValue)
            {
                if (Mathf.Abs(lastSpawnPosition.Value.x - aSpawnPosition.x)<prop.GetSpaceBetween().z) return;
            }

            GameObject obj = SpawnObject(aGameObjectName);

            if (obj == null) Debug.Log("No available objects in pool");
            
            obj.transform.position = aSpawnPosition + prop.GetOffset();
            obj.transform.rotation = aRotation;
        }
    }
}

