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

        int m_CurrentCoins;
        float m_PastCoins;
        float m_CombinedDeltaTime;

        private Dictionary<Player,int> m_PlayerCoins;

        public CoinCollectionComponent(Guid entity) : base(entity)
        {
            m_CoinTexture = AssetManager.Get().Find<Texture2D>(ESpriteAssets.CoinAnimation);
            m_Font = AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont);
            m_PlayerCoins = new Dictionary<Player, int>();
        }

        //---------------------------------------------------------------------------

        int InterpolateDisplay(int value, float deltaTime, float durationInSeconds = 0.5f)
        {
            m_CombinedDeltaTime += deltaTime;

            if (m_CombinedDeltaTime >= durationInSeconds)
                return m_CurrentCoins;
            else
            {
                float temp = m_CurrentCoins - m_PastCoins;
                return (int)(m_PastCoins + (temp * m_CombinedDeltaTime/durationInSeconds));
            }
        }

        //---------------------------------------------------------------------------

        public void Bind(params Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                m_PlayerCoins.Add(player, 0);

                UIManager.Get().RegisterListener(player.Attributes, "CurrentCoins", (value) =>
                {
                    m_PastCoins = m_CurrentCoins;
                    m_PlayerCoins[player] = (int)value;
                    m_CurrentCoins = m_PlayerCoins.Values.Sum(coin => coin);
                    m_CombinedDeltaTime = 0; //noch unschön :P
                });
            }
        }

        public void Draw(SpriteBatch batch, float deltaTime)
        {
            UITransformComponent transform = GetComponent<UITransformComponent>();
            if (transform != null)
            {
                Rectangle bounds = transform.Bounds();
                batch.DrawString(m_Font, InterpolateDisplay(m_CurrentCoins,deltaTime).ToString(), new Vector2(bounds.Center.X - m_Font.MeasureString("Coins").X, bounds.Y), Color.White);
            }


        }

        public override void OnCleanup() { }
    }
}
