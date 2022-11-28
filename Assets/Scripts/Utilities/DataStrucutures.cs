using UnityEngine;

namespace Sahil.DataStructures
{
    [System.Serializable]
    public struct Vector3Range
    {
        [SerializeField] Vector3 min, max;

        public Vector3 GetRandomValue() => new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.y, max.y));
        public Vector3 GetMin => min;
        public Vector3 GetMax => max;
    }

    [System.Serializable]
    public struct IntRange
    {
        [SerializeField] int min, max;

        public int GetRandomValue() => Random.Range(min, max);

        public int GetMin() => min;
        public int GetMax() => max;
    }
}