using EvershockGame.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Items
{
    public enum EItemPool
    {
        None,
        SmallChest
    }

    //---------------------------------------------------------------------------

    public class ItemPool
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EItemPool Type { get; private set; }
        
        public Dictionary<EItemType, float> Types { get; private set; }
        [JsonIgnore]
        private float m_MaxProbability;
        [JsonIgnore]
        private Random m_Rand;

        //---------------------------------------------------------------------------

        public ItemPool(EItemPool type)
        {
            Type = type;

            Types = new Dictionary<EItemType, float>();
            m_Rand = new Random(SeedManager.Get().NextSeed());
        }

        //---------------------------------------------------------------------------

        public void ResetMaxProbability()
        {
            m_MaxProbability = Types.Values.Sum(probability => probability);
        }

        //---------------------------------------------------------------------------

        public void Add(EItemType type, float probability)
        {
            if (!Types.ContainsKey(type))
            {
                Types.Add(type, probability);
                m_MaxProbability += probability;
            }
        }

        //---------------------------------------------------------------------------

        public void Remove(EItemType type)
        {
            if (Types.ContainsKey(type))
            {
                m_MaxProbability -= Types[type];
                Types.Remove(type);
            }
        }

        //---------------------------------------------------------------------------

        public EItemType Next()
        {
            float rnd = (float)m_Rand.NextDouble() * m_MaxProbability;
            float sum = 0.0f;

            foreach (KeyValuePair<EItemType, float> kvp in Types)
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
