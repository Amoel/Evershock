using EntityComponent.Items;
using Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public class ItemManager : BaseManager<ItemManager>
    {
        private Dictionary<EItemRarity, Color> m_RarityColors = new Dictionary<EItemRarity, Color>()
        {
            { EItemRarity.Common, Color.White },
            { EItemRarity.Uncommon, Color.Cyan },
            { EItemRarity.Rare, Color.Green },
            { EItemRarity.Epic, Color.Purple },
            { EItemRarity.Legendary, Color.Gold }
        };

        private Dictionary<EItemPool, ItemPool> m_ItemPools;
        private Dictionary<EItemType, ItemDesc> m_Items;

        //---------------------------------------------------------------------------

        protected ItemManager()
        {
            m_ItemPools = new Dictionary<EItemPool, ItemPool>();
            m_Items = new Dictionary<EItemType, ItemDesc>();
        }

        //---------------------------------------------------------------------------

        public void RegisterItemPool(ItemPool pool)
        {
            if (!m_ItemPools.ContainsKey(pool.Type))
            {
                m_ItemPools.Add(pool.Type, pool);
            }
        }

        //---------------------------------------------------------------------------

        public void RegisterItem(ItemDesc item)
        {
            if (!m_Items.ContainsKey(item.Type))
            {
                m_Items.Add(item.Type, item);
            }
        }

        //---------------------------------------------------------------------------

        public ItemPool Find(EItemPool type)
        {
            if (m_ItemPools.ContainsKey(type))
            {
                return m_ItemPools[type];
            }
            return null;
        }

        //---------------------------------------------------------------------------

        public ItemDesc Find(EItemType type)
        {
            if (m_Items.ContainsKey(type))
            {
                return m_Items[type];
            }
            return ItemDesc.Empty;
        }

        //---------------------------------------------------------------------------

        public EItemType Next(EItemPool type)
        {
            ItemPool pool = Find(type);
            if (pool != null)
            {
                return pool.Next();
            }
            return EItemType.None;
        }

        //---------------------------------------------------------------------------

        public Color GetColorByRarity(EItemRarity rarity)
        {
            if (m_RarityColors.ContainsKey(rarity))
            {
                return m_RarityColors[rarity];
            }
            return Color.White;
        }

        //---------------------------------------------------------------------------

        public Color GetColorByType(EItemType type)
        {
            ItemDesc desc = Find(type);
            if (desc.IsValid)
            {
                return GetColorByRarity(desc.Rarity);
            }
            return Color.White;
        }
    }
}
