using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Items
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

        public string Name { get; set; }
        public EItemType Type { get; set; }
        public EItemRarity Rarity { get; set; }
        public EItemCategory Category { get; set; }
        
        public Sprite Sprite { get; set; }
        public bool IsStackable { get; set; }

        public bool IsEmpty { get; set; }

        //---------------------------------------------------------------------------

        public ItemDesc(string name, EItemType type, EItemRarity rarity, Sprite sprite, bool isStackable) : this()
        {
            Name = name;
            Type = type;
            Rarity = rarity;
            Sprite = sprite;
            IsStackable = isStackable;
        }

        //---------------------------------------------------------------------------

        public static ItemDesc Empty
        {
            get { return new ItemDesc() { IsEmpty = true }; }
        }

        //---------------------------------------------------------------------------

        public static ItemDesc FromItemStoreDesc(Texture2D itemSpritesheet, int spritesheetWidth, int spritesheetHeight, ItemStoreDesc desc)
        {
            return new ItemDesc()
            {
                Name = desc.Name,
                Type = desc.Type,
                Rarity = desc.Rarity,
                Category = desc.Category,
                Sprite = new Sprite(itemSpritesheet, spritesheetWidth, spritesheetHeight, desc.SpriteX, desc.SpriteY),
                IsStackable = desc.IsStackable
            };
        }
    }

    //---------------------------------------------------------------------------

    public struct ItemStoreDesc
    {
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EItemType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EItemRarity Rarity { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EItemCategory Category { get; set; }
        public int SpriteX { get; set; }
        public int SpriteY { get; set; }
        public bool IsStackable { get; set; }

        //---------------------------------------------------------------------------

        public static ItemStoreDesc FromItemDesc(ItemDesc desc)
        {
            return new ItemStoreDesc()
            {
                Name = desc.Name,
                Type = desc.Type,
                Rarity = desc.Rarity,
                Category = desc.Category,
                SpriteX = desc.Sprite.Bounds.X / desc.Sprite.Bounds.Width,
                SpriteY = desc.Sprite.Bounds.Y / desc.Sprite.Bounds.Height,
                IsStackable = desc.IsStackable
            };
        }
    }
}
