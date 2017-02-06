using EvershockGame.Components;
using EvershockGame.Stages;
using Level;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Manager
{
    public delegate void StageChangedEventHandler();

    //---------------------------------------------------------------------------

    public class StageManager : BaseManager<StageManager>
    {
        public Stage Stage { get; set; }

        private Chunk[,] m_Chunks;

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

            Left = 0;
            Right = horizontalChunks * Chunk.Width;
            Top = 0;
            Bottom = verticalChunks * Chunk.Height;

            Width = Right - Left;
            Height = Bottom - Top;

            for (int y = 0; y < verticalChunks; y++)
            {
                for (int x = 0; x < horizontalChunks; x++)
                {
                    AddChunk(x, y);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Create(byte[,] map)
        {
            int horizontalChunks = (int)Math.Ceiling((float)map.GetLength(0) / Chunk.Width);
            int verticalChunks = (int)Math.Ceiling((float)map.GetLength(1) / Chunk.Height);
            m_Chunks = new Chunk[horizontalChunks, verticalChunks];

            Left = 0;
            Right = horizontalChunks * Chunk.Width;
            Top = 0;
            Bottom = verticalChunks * Chunk.Height;

            Width = Right - Left;
            Height = Bottom - Top;

            for (int y = 0; y < verticalChunks; y++)
            {
                for (int x = 0; x < horizontalChunks; x++)
                {
                    AddChunk(x, y);
                }
            }

            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            Random r = new Random(SeedManager.Get().NextSeed());
            int tileSize = 24;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    SetIsBlocked(x, y, map[x, y] < 255);
                    if (map[x, y] < 255)
                    {
                        int sumSide = CalcTileIndexSide(
                            x > 0 && map[x - 1, y] == map[x, y],
                            x < mapWidth - 1 && map[x + 1, y] == map[x, y]);
                        SetTextureBounds(x, y, ELayerMode.First, new Rectangle(tileSize * (sumSide + (sumSide > 2 ? r.Next(0, 4) : 0)), tileSize + tileSize * 3 * map[x, y], tileSize, tileSize));
                        int sumTop = CalcTileIndexFull(
                            y > 0 && map[x, y - 1] == map[x, y],
                            x > 0 && map[x - 1, y] == map[x, y],
                            y < mapHeight - 1 && map[x, y + 1] == map[x, y],
                            x < mapWidth - 1 && map[x + 1, y] == map[x, y]);
                        SetTextureBounds(x, y - 1, ELayerMode.Third, new Rectangle(tileSize * sumTop, tileSize * 3 * map[x, y], tileSize, tileSize));
                    }
                    else
                    {
                        SetTextureBounds(x, y, ELayerMode.First, new Rectangle(tileSize * r.Next(9), tileSize * 2, tileSize, tileSize));
                    }
                }
            }           

            foreach (Chunk chunk in m_Chunks)
            {
                chunk.CreateCollision();
            }
        }

        //---------------------------------------------------------------------------

        private void AddChunk(int x, int y)
        {
            m_Chunks[x, y] = new Chunk(x, y);
            if (x > 0 && m_Chunks[x - 1, y] != null)
            {
                m_Chunks[x, y].LeftChunk = m_Chunks[x - 1, y];
                m_Chunks[x - 1, y].RightChunk = m_Chunks[x, y];
            }
            if (x < m_Chunks.GetLength(0) - 1 && m_Chunks[x + 1, y] != null)
            {
                m_Chunks[x, y].RightChunk = m_Chunks[x + 1, y];
                m_Chunks[x + 1, y].LeftChunk = m_Chunks[x, y];
            }
            if (y > 0 && m_Chunks[x, y - 1] != null)
            {
                m_Chunks[x, y].TopChunk = m_Chunks[x, y - 1];
                m_Chunks[x, y - 1].BottomChunk = m_Chunks[x, y];
            }
            if (y < m_Chunks.GetLength(1) - 1 && m_Chunks[x, y + 1] != null)
            {
                m_Chunks[x, y].BottomChunk = m_Chunks[x, y + 1];
                m_Chunks[x, y + 1].TopChunk = m_Chunks[x, y];
            }
        }

        //---------------------------------------------------------------------------

        private int CalcTileIndexFull(bool isAboveSame, bool isLeftSame, bool isBelowSame, bool isRightSame)
        {
            int sum = 0;
            if (isAboveSame) sum += 1;
            if (isLeftSame) sum += 2;
            if (isBelowSame) sum += 4;
            if (isRightSame) sum += 8;
            return sum;
        }

        //---------------------------------------------------------------------------

        private int CalcTileIndexSide(bool isLeftSame, bool isRightSame)
        {
            int sum = 0;
            if (isLeftSame) sum += 1;
            if (isRightSame) sum += 2;
            return sum;
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

        public List<Corner> GetCorners()
        {
            if (Stage != null)
            {
                return Stage.Corners;
            }
            return new List<Corner>();
        }

        //---------------------------------------------------------------------------

        private void OnStageChanged()
        {
            StageChanged?.Invoke();
        }
    }
}
