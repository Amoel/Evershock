using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components
{
    public class AreaSpriteComponent : Component, IDrawableComponent
    {
        public Texture2D Tileset { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private int[,] m_Map;

        //---------------------------------------------------------------------------

        public AreaSpriteComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Init(Texture2D tileset, int tileWidth, int tileHeight, int[,] map)
        {
            Tileset = tileset;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Width = (Tileset.Width / TileWidth);
            Height = (Tileset.Height / TileHeight);

            m_Map = map;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (Tileset != null && m_Map != null)
            {
                for (int y = 0; y < m_Map.GetLength(1); ++y)
                {
                    for (int x = 0; x < m_Map.GetLength(0); ++x)
                    {
                        int tileX = m_Map[y, x] % Width;
                        int tileY = m_Map[y, x] / Width;
                        batch.Draw(Tileset, 
                            new Rectangle(32 * x - (int)data.Center.X, 32 * y - (int)data.Center.Y, 32, 32), 
                            new Rectangle(TileWidth * tileX, TileHeight * tileY, TileWidth, TileHeight), 
                            Color.White);
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
