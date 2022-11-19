using UnityEngine;
namespace Sahil.Perlin
{
    [System.Serializable]
    class PerlinNoiseGenerator
    {
        [SerializeField] Octave[] m_octaves;
        int m_initialSeed;
        
        public void SetSeed(int aInitSeed) =>m_initialSeed = aInitSeed;
        
        public PerlinNoiseGenerator(Octave[] aOctaves, int aInitSeed)
        {
            m_initialSeed =aInitSeed;
            
            m_octaves = aOctaves;
        }

        /// <summary>
        /// Returns a PerlinNoise value between -1 and 1.
        /// </summary>
        /// <param name="aPerlinMapPosition"></param>
        /// <returns></returns>
        public float GenerateValue(Vector3 aPerlinMapPosition)
        {
            float value = 0;
            float totalAmplitutde=0;

            for(int i = 0; i < m_octaves.Length; i++)
            {
                Random.seed = m_initialSeed+i;

                value = Mathf.PerlinNoise(aPerlinMapPosition.x * m_octaves[i].Frequency, aPerlinMapPosition.y * m_octaves[i].Frequency);

                value *= m_octaves[i].Amplitude;
                
                totalAmplitutde+= m_octaves[i].Amplitude;
            }

            value/=totalAmplitutde;
            value = (value * 2) - 1;   

            return value;
        }

        [System.Serializable]
        public struct Octave
        {
            public float Amplitude, Frequency;
        }
    }
}
