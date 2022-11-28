using Sahil.DataStructures;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

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
        

        protected ObjectPool[] m_pool_props;

        protected void Initialize()
        {
            m_pool_props = new ObjectPool[m_props.Length];
            m_propsDict = new Dictionary<string, Prop>();
            for (int i = 0; i < m_pool_props.Length; i++)
            {
                ObjectPool objectPool = new ObjectPool();
                objectPool.Initialize(m_props[i].GetPrefab(), m_props[i].GetMaxSpawnQuantity() * m_setOfObjectsInPool);
                m_pool_props[i] = objectPool;

                m_propsDict.Add(m_props[i].GetPrefab().name, m_props[i]);
            }
        }

        protected GameObject SpawnObject(int aPropIndex)
        {
            return m_pool_props[aPropIndex].Spawn(Vector3.zero, Quaternion.identity);
        }
        protected GameObject SpawnObject(int aPropIndex,Vector3 aPosition)
        {
            return m_pool_props[aPropIndex].Spawn(aPosition, Quaternion.identity);
        }

        protected GameObject SpawnObject(int aPropIndex,Vector3 aPosition, Quaternion aRotation)
        {
            return m_pool_props[aPropIndex].Spawn(aPosition, aRotation);
        }

        protected void DeSpawn()
        {
            //Get list on enabled object,check from bottom for which are behind player,from that index to 0 request to push and disable;
            for (int i = 0; i < m_pool_props.Length; i++)
            {
                GameObject[] activeObst = m_pool_props[i].GetActiveObjects();

                for (int k = 0; k < activeObst.Length; k++)
                {
                    //obstacle[i].DeactivateObject(activeObst[k]);
                }
            }
        }


        protected Prop GetProp(string aGameObjectName) => m_propsDict[aGameObjectName];
    }


    [System.Serializable]
    public struct Prop
    {
        [SerializeField] GameObject m_prefab;
        [SerializeField] Vector3 m_spaceBetween;
        [SerializeField] IntRange m_quantity;

        public GameObject GetPrefab() => m_prefab;
        public int GetMaxSpawnQuantity() => m_quantity.GetMax();
        public Vector3 GetSpaceBetween()=>m_spaceBetween;
        //[Header("______________________________________________________________________________")]
        //[Header("SPAWN IN SERIES\n")]
        //[SerializeField] bool spawnInSeries;
        //[SerializeField] Vector3 distanceBetween;
    }
}

