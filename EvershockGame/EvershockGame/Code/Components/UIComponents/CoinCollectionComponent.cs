using System;
using EvershockGame.Components.UI;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace EvershockGame.Code.Components
{
    [Serializable]
    [RequireComponent(typeof(UITransformComponent))]
    public class CoinCollectionComponent : Component, IDrawableUIComponent
    {
        Texture2D m_CoinTexture;
        SpriteFont m_Font;
        
        short m_CurrentCoins;

        private Dictionary<Player,short> m_PlayerCoins;

        public CoinCollectionComponent(Guid entity) : base(entity)
        {
            m_CoinTexture = AssetManager.Get().Find<Texture2D>(ESpriteAssets.CoinAnimation);
            m_Font = AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont);
            m_PlayerCoins = new Dictionary<Player, short>();
        }

        public void Bind(params Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                m_PlayerCoins.Add(player, 0);

                UIManager.Get().RegisterListener(player.Attributes, "CurrentCoins", (value) =>
                {
                    m_PlayerCoins[player] = (short)value;
                    m_CurrentCoins = (short)m_PlayerCoins.Values.Sum(coin => coin);
                });
            }
        }

        public void Draw(SpriteBatch batch)
        {
            UITransformComponent transform = GetComponent<UITransformComponent>();
            if (transform != null)
            {
                Rectangle bounds = transform.Bounds();
                batch.DrawString(m_Font, m_CurrentCoins.ToString(), new Vector2(bounds.Center.X - m_Font.MeasureString("Coins").X, bounds.Y), Color.White);
            }
        }

        public override void OnCleanup() { }
    }
}
