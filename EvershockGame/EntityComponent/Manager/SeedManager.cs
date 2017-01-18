using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public class SeedManager : BaseManager<SeedManager>
    {
        private Random m_RandSeed;

        //---------------------------------------------------------------------------

        protected SeedManager()
        {
            m_RandSeed = new Random();
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
    }
}
