using Sahil.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Sahil
{
    public class Spawner
    {
        [Tooltip("Define how many sets of all objects you want the objectpool to spawn.\r\nNote: Each set is the max amount set for each prop.(Prop.Max *SetsOfObjectInPool).")]
        /// <summary>
        /// Define how many sets of all objects you want the objectpool to spawn.
        /// Note: Each set is the max amount set for each prop.(Prop.Max *SetsOfObjectInPool).
        /// </summary>
        [SerializeField] int m_setOfObjectsInPool;
        [SerializeField] protected Prop[] m_props;

        Dictionary<string, Prop> m_propsDict;

        private Dictionary<string, ObjectPool> m_pool_props;

        protected void Initialize()
        {
            m_pool_props = new Dictionary<string, ObjectPool>();
            m_propsDict = new Dictionary<string, Prop>();
            for (int i = 0; i < m_props.Length; i++)
            {
                ObjectPool objectPool = new ObjectPool();
                objectPool.Initialize(m_props[i].GetPrefab(), m_props[i].GetMaxSpawnQuantity() * m_setOfObjectsInPool);

                m_pool_props.Add(m_props[i].GetPropName(), objectPool);
                m_propsDict.Add(m_props[i].GetPropName(), m_props[i]);
            }
        }

        protected GameObject SpawnObject(string aPropName,bool aDespawnIfNeeded=true)
        {
            return SpawnObject(aPropName,Vector3.zero, Quaternion.identity, aDespawnIfNeeded);
        }

        protected GameObject SpawnObject(string aPropName, Vector3 aPosition, bool aDespawnIfNeeded = true)
        {
            return SpawnObject(aPropName, aPosition, Quaternion.identity, aDespawnIfNeeded);
        }

        protected GameObject SpawnObject(string aPropName, Vector3 aPosition, Quaternion aRotation, bool aDespawnIfNeeded = true)
        {
            if (m_pool_props[aPropName].IsObjectAvailableToSpawn() == false)
            {
                DeSpawnObject(aPropName);
            }
            return m_pool_props[aPropName].Spawn(aPosition, aRotation);
        }

        protected void DeSpawnObject(string aPropName,int aCount=1)
        {
            m_pool_props[aPropName].DeSpawnObjects(aCount);
        }

        protected Prop GetPropByName(string aGameObjectName) => m_propsDict[aGameObjectName];

        protected Vector3? GetLastSpawnedPropPosition(string aPropName)
        {
            GameObject obj=m_pool_props[aPropName].GetLastSpawnObject();

            if(obj != null)
            {
               return obj.transform.position;
            }
            return null;
        }
        protected Quaternion? GetLastSpawnedPropRotation(string aPropName)
        {
            GameObject obj = m_pool_props[aPropName].GetLastSpawnObject();

            if (obj != null)
            {
                return obj.transform.rotation;
            }
            return null;
        }
    }


    [System.Serializable]
    public struct Prop
    {
        [SerializeField] string m_name;
        [SerializeField] GameObject m_prefab;
        [SerializeField] Vector3 m_spaceBetween;
        [SerializeField] Vector3 m_offset;
        [SerializeField] IntRange m_quantity;

        public string GetPropName() => m_name;
        public GameObject GetPrefab() => m_prefab;
        public int GetMaxSpawnQuantity() => m_quantity.GetMax();
        public Vector3 GetSpaceBetween()=>m_spaceBetween;
        public Vector3 GetOffset()=> m_offset;

    }
}

