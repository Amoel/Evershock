using EntityComponent.Stage;
using Level;
using Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public delegate void StageChangedEventHandler();

    //---------------------------------------------------------------------------

    public class StageManager : BaseManager<StageManager>
    {
        Chunk[,] m_Chunks;

        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public event StageChangedEventHandler StageChanged;

        //---------------------------------------------------------------------------

        protected StageManager() { }

        //---------------------------------------------------------------------------

        public void Create(int width, int height)
        {
            int horizontalChunks = (int)Math.Ceiling((float)width / Chunk.Width);
            int verticalChunks = (int)Math.Ceiling((float)height / Chunk.Height);
            m_Chunks = new Chunk[horizontalChunks, verticalChunks];

            for (int y = 0; y < verticalChunks; y++)
            {
                for (int x = 0; x < horizontalChunks; x++)
                {
                    m_Chunks[x, y] = new Chunk(x, y);
                }
            }

            Left = 0;
            Right = horizontalChunks * Chunk.Width;
            Top = 0;
            Bottom = verticalChunks * Chunk.Height;

            Width = Right - Left;
            Height = Bottom - Top;
        }

        //---------------------------------------------------------------------------

        public bool IsBlocked(int x, int y)
        {
            if (x < Left || x >= Right || y < Top || y >= Bottom) return true;
            return m_Chunks[x / Chunk.Width, y / Chunk.Height].IsBlocked(x, y);
        }

        //---------------------------------------------------------------------------

        public void SetIsBlocked(int x, int y, bool isBlocked)
        {
            if (x < Left || x >= Right || y < Top || y >= Bottom) return;
            m_Chunks[x / Chunk.Width, y / Chunk.Height].SetIsBlocked(x, y, isBlocked);
        }

        //---------------------------------------------------------------------------

        public Rectangle GetTextureBounds(int x, int y, ELayerMode layer)
        {
            if (x < Left || x >= Right || y < Top || y >= Bottom) return new Rectangle();
            return m_Chunks[x / Chunk.Width, y / Chunk.Height].GetTextureBounds(x, y, layer);
        }

        //---------------------------------------------------------------------------

        public void SetTextureBounds(int x, int y, ELayerMode layer, Rectangle bounds)
        {
            if (x < Left || x >= Right || y < Top || y >= Bottom) return;
            m_Chunks[x / Chunk.Width, y / Chunk.Height].SetTextureBounds(x, y, layer, bounds);
        }

        //---------------------------------------------------------------------------

        public void Load(Map map, int x, int y)
        {
            for (int _y = 0; _y < map.Height; _y++)
            {
                for (int _x = 0; _x < map.Width; _x++)
                {
                    SetIsBlocked(x + _x, y + _y, map[_x, _y].IsBlocked);
                    foreach (KeyValuePair<ELayerMode, Layer> kvp in map[_x, _y].Layers)
                    {
                        SetTextureBounds(x + _x, y + _y, kvp.Key, new Rectangle(kvp.Value.TargetX * 32, kvp.Value.TargetY * 32, 32, 32));
                    }
                }
            }

            foreach (Chunk chunk in m_Chunks)
            {
                chunk.CreateCollision();
            }
        }

        //---------------------------------------------------------------------------

        private void OnStageChanged()
        {
            StageChanged?.Invoke();
        }
    }
}
