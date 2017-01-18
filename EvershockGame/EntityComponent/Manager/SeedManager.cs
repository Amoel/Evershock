using Managers;
using System;

namespace EntityComponent.Manager
{
    public class SeedManager : BaseManager<SeedManager>
    {
        private Random m_RandSeed;
        private Random m_Rand;

        //---------------------------------------------------------------------------

        protected SeedManager()
        {
            m_RandSeed = new Random();
            m_Rand = new Random();
        }

        //---------------------------------------------------------------------------

        public void ResetBaseSeed(int seed)
        {
            m_RandSeed = new Random(seed);
        }

        //---------------------------------------------------------------------------

        public int NextSeed()
        {
            return m_RandSeed.Next();
        }

        //---------------------------------------------------------------------------

        public int NextSeed(int min, int max)
        {
            return m_RandSeed.Next(min, max);
        }

        //---------------------------------------------------------------------------

        public int NextRand()
        {
            return m_Rand.Next();
        }

        //---------------------------------------------------------------------------

        public float NextRandF()
        {
            return (float)m_Rand.NextDouble();
        }

        //---------------------------------------------------------------------------

        public int NextRand(int min, int max)
        {
            return m_Rand.Next(min, max);
        }

        //---------------------------------------------------------------------------

        public float NextRandF(float min, float max)
        {
            return min + (float)m_Rand.NextDouble()* (max- min);
        }
    }
}
