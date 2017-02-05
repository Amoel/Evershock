using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Items
{
    public enum EItemPool
    {
        None,
        SmallChest
    }

    //---------------------------------------------------------------------------

    public class ItemPool
    {
        public EItemPool Type { get; private set; }

        private Dictionary<EItemType, float> m_Types;
        private float m_MaxProbability;

        private Random m_Rand;

        //---------------------------------------------------------------------------

        public ItemPool(EItemPool type)
        {
            Type = type;

            m_Types = new Dictionary<EItemType, float>();
            m_Rand = new Random(SeedManager.Get().NextSeed());
        }

        //---------------------------------------------------------------------------

        public void Add(EItemType type, float probability)
        {
            if (!m_Types.ContainsKey(type))
            {
                m_Types.Add(type, probability);
                m_MaxProbability += probability;
            }
        }

        //---------------------------------------------------------------------------

        public void Remove(EItemType type)
        {
            if (m_Types.ContainsKey(type))
            {
                m_MaxProbability -= m_Types[type];
                m_Types.Remove(type);
            }
        }

        //---------------------------------------------------------------------------

        public EItemType Next()
        {
            float rnd = (float)m_Rand.NextDouble() * m_MaxProbability;
            float sum = 0.0f;

            foreach (KeyValuePair<EItemType, float> kvp in m_Types)
            {
                sum += kvp.Value;
                if (rnd < sum)
                {
                    return kvp.Key;
                }
            }
            return EItemType.None;
        }
    }
}
