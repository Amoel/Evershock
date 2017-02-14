using EvershockGame.Components;
using EvershockGame.Components.UI;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EvershockGame.Code.Components
{
    [Serializable]
    [RequireComponent(typeof(UITransformComponent))]
    public class HealthbarComponent : Component, IDrawableUIComponent
    {
        private Texture2D m_Hearts;
        private SpriteFont m_Font;
        private EHorizontalAlignment m_Alignment;

        private float m_MaxHealth;
        private float m_Health;
        private float m_MaxMana;
        private float m_Mana;

        private Player m_Player;

        private static int m_Factor = 50;

        //---------------------------------------------------------------------------

        public HealthbarComponent(Guid entity) : base(entity)
        {
            m_Hearts = AssetManager.Get().Find<Texture2D>(ETilesetAssets.Hearts);
            m_Font = AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont);
        }

        //---------------------------------------------------------------------------

        public void BindPlayer(Player player, EHorizontalAlignment alignment)
        {
            m_Player = player;
            m_Alignment = alignment;

            UIManager.Get().RegisterListener(player.Attributes, "CurrentHealth", (value) =>
            {
                m_Health = (float)value;
            });

            UIManager.Get().RegisterListener(player.Attributes, "MaxHealth", (value) =>
            {
                m_MaxHealth = (float)value;
            });

            UIManager.Get().RegisterListener(player.Attributes, "CurrentMana", (value) =>
            {
                m_Mana = (float)value;
            });

            UIManager.Get().RegisterListener(player.Attributes, "MaxMana", (value) =>
            {
                m_MaxMana = (float)value;
            });
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            if (m_Hearts != null)
            {
                UITransformComponent transform = GetComponent<UITransformComponent>();
                if (transform != null)
                {
                    Rectangle bounds = transform.Bounds();

                    if (m_Player != null)
                    {
                        batch.DrawString(m_Font, m_Player.Name, new Vector2(m_Alignment == EHorizontalAlignment.Left ? bounds.X + 4 : bounds.X + bounds.Width - m_Font.MeasureString(m_Player.Name).X - 4, bounds.Y), Color.White);
                    }

                    int maxHeartCount = (int)m_MaxHealth / m_Factor;
                    int heartCount = (int)m_Health / m_Factor;
                    for (int x = 0; x < maxHeartCount; x++)
                    {
                        if (x < heartCount)
                        {
                            batch.Draw(m_Hearts, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X + x * 32 : bounds.X + bounds.Width - (x + 1) * 32, bounds.Y + 32, 32, 28), new Rectangle(0, 0, 64, 56), Color.White);
                        }
                        else
                        {
                            batch.Draw(m_Hearts, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X + x * 32 : bounds.X + bounds.Width - (x + 1) * 32, bounds.Y + 32, 32, 28), new Rectangle(0, 56, 64, 56), Color.White);
                        }
                    }
                    float heartSegment = (m_Health % m_Factor) / m_Factor;
                    batch.Draw(m_Hearts, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X + heartCount * 32 : bounds.X + bounds.Width - (heartCount + 1) * 32, bounds.Y + 32, (int)(heartSegment * 32.0f), 28), new Rectangle(0, 0, (int)(heartSegment * 64), 56), Color.White * 0.4f);

                    int maxManaCount = (int)m_MaxMana / m_Factor;
                    int manaCount = (int)m_Mana / m_Factor;
                    for (int x = 0; x < maxManaCount; x++)
                    {
                        if (x < manaCount)
                        {
                            batch.Draw(m_Hearts, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X + x * 32 : bounds.X + bounds.Width - (x + 1) * 32, bounds.Y + 64, 32, 28), new Rectangle(64, 0, 64, 56), Color.White);
                        }
                        else
                        {
                            batch.Draw(m_Hearts, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X + x * 32 : bounds.X + bounds.Width - (x + 1) * 32, bounds.Y + 64, 32, 28), new Rectangle(64, 56, 64, 56), Color.White);
                        }
                    }
                    float manaSegment = (m_Mana % m_Factor) / m_Factor;
                    batch.Draw(m_Hearts, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X + manaCount * 32 : bounds.X + bounds.Width - (manaCount + 1) * 32, bounds.Y + 64, (int)(manaSegment * 32.0f), 28), new Rectangle(64, 0, (int)(manaSegment * 64), 56), Color.White * 0.4f);

                    InventoryComponent inventory = m_Player.GetComponent<InventoryComponent>();
                    if (inventory != null)
                    {
                        for (int i = 0; i < inventory.Size; i++)
                        {
                            if (i == inventory.ActiveIndex)
                            {
                                Texture2D tex = CollisionManager.Get().PointTexture;
                                batch.Draw(tex, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X : bounds.X + bounds.Width - 50, bounds.Y + 120 + i * 52, 50, 50), tex.Bounds, Color.White * 0.3f);
                            }
                            InventorySlot slot = inventory[i];
                            if (slot != null && !slot.Item.IsEmpty)
                            {
                                batch.Draw(slot.Item.Sprite.Texture, new Rectangle(m_Alignment == EHorizontalAlignment.Left ? bounds.X : bounds.X + bounds.Width - 50, bounds.Y + 120 + i * 52, 50, 50), slot.Item.Sprite.Bounds, Color.White);
                                batch.DrawString(m_Font, slot.Count.ToString(), new Vector2(m_Alignment == EHorizontalAlignment.Left ? bounds.X : bounds.X + bounds.Width - 50, bounds.Y + 120 + i * 52), Color.White);
                            }
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
