using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Items
{
    public enum EItemType
    {
        None,
        HealthPotion,
        ManaPotion,
        Axe,
        SmallKey
    }

    public enum EItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum EItemCategory
    {
        Usable,
        MeleeWeapon,
        RangeWeapon
    }

    //---------------------------------------------------------------------------

    public struct ItemDesc
    {
        public static int MaxStack = 5;

        public Guid ID { get; private set; }
        public string Name { get; private set; }
        public EItemType Type { get; private set; }
        public EItemRarity Rarity { get; private set; }
        public EItemCategory Category { get; private set; }

        public Sprite Sprite { get; private set; }
        public bool IsStackable { get; private set; }

        public bool IsValid { get; private set; }

        //---------------------------------------------------------------------------

        public ItemDesc(string name, EItemType type, EItemRarity rarity, Sprite sprite, bool isStackable) : this()
        {
            ID = Guid.NewGuid();
            Name = name;
            Type = type;
            Rarity = rarity;
            Sprite = sprite;
            IsStackable = isStackable;
            IsValid = true;
        }

        //---------------------------------------------------------------------------

        public static ItemDesc Empty
        {
            get { return new ItemDesc(); }
        }
    }
}
