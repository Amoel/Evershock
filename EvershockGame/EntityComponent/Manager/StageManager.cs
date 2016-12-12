using EntityComponent.Components;
using EntityComponent.Stage;
using Level;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public void Create(bool[,] map)
        {
            int horizontalChunks = (int)Math.Ceiling((float)map.GetLength(0) / Chunk.Width);
            int verticalChunks = (int)Math.Ceiling((float)map.GetLength(1) / Chunk.Height);
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

            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            Random r = new Random();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    SetIsBlocked(x, y, !map[x, y]);
                    if (!map[x, y])
                    {
                        int sumSide = CalcTileIndexSide(
                            x > 0 && !map[x - 1, y],
                            x < mapWidth - 1 && !map[x + 1, y]);
                        SetTextureBounds(x, y, ELayerMode.First, new Rectangle(24 * (sumSide + (sumSide > 2 ? r.Next(0, 4) : 0)), 24, 24, 24));
                        int sumTop = CalcTileIndexFull(
                            y > 0 && !map[x, y - 1],
                            x > 0 && !map[x - 1, y],
                            y < mapHeight - 1 && !map[x, y + 1],
                            x < mapWidth - 1 && !map[x + 1, y]);
                        SetTextureBounds(x, y - 1, ELayerMode.Third, new Rectangle(24 * sumTop, 0, 24, 24));
                    }
                    else
                    {
                        SetTextureBounds(x, y, ELayerMode.First, new Rectangle(24 * r.Next(9), 48, 24, 24));
                    }
                }
            }           

            foreach (Chunk chunk in m_Chunks)
            {
                chunk.CreateCollision();
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

        public void Draw(SpriteBatch batch, CameraData data)
        {
            if (m_Chunks != null && CollisionManager.Get().IsDebugViewActive)
            {
                Texture2D tex = CollisionManager.Get().PointTexture;
                for (int y = 0; y < m_Chunks.GetLength(1); y++)
                {
                    for (int x = 0; x < m_Chunks.GetLength(0); x++)
                    {
                        Vector2 location = new Vector2(x * Chunk.Width * 32, y * Chunk.Height * 32).ToLocal(data);
                        Rectangle bounds = new Rectangle((int)location.X, (int)location.Y, Chunk.Width * 32, Chunk.Height * 32);
                        if (bounds.Intersects(data.Bounds))
                        {
                            batch.Draw(tex, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), tex.Bounds, Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                            batch.Draw(tex, new Rectangle(bounds.X, bounds.Y, 1, bounds.Height), tex.Bounds, Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                            batch.Draw(tex, new Rectangle(bounds.X + bounds.Width, bounds.Y, 1, bounds.Height), tex.Bounds, Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                            batch.Draw(tex, new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, 1), tex.Bounds, Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        private void OnStageChanged()
        {
            StageChanged?.Invoke();
        }
    }
}
