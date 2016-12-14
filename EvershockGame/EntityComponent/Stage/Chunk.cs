using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Manager;
using Level;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Stage
{
    public class Chunk
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public static readonly int Width = 9;
        public static readonly int Height = 9;

        private ChunkCell[,] m_Cells;
        private Guid m_CollisionEntity;

        public Chunk LeftChunk { get; set; }
        public Chunk RightChunk { get; set; }
        public Chunk TopChunk { get; set; }
        public Chunk BottomChunk { get; set; }

        //---------------------------------------------------------------------------

        public int GlobalX(int x) { return X * Width + x; }
        public int GlobalY(int y) { return Y * Height + y; }

        //---------------------------------------------------------------------------

        public Chunk(int x, int y)
        {
            X = x;
            Y = y;
            m_Cells = new ChunkCell[Width, Height];
            for (int _y = 0; _y < Height; _y++)
            {
                for (int _x = 0; _x < Width; _x++)
                {
                    m_Cells[_x, _y] = new ChunkCell();
                }
            }
        }

        //---------------------------------------------------------------------------

        public ChunkCell this[int x, int y]
        {
            get { return m_Cells[x, y]; }
        }

        //---------------------------------------------------------------------------

        public bool IsBlocked(int x, int y)
        {
            return m_Cells[x % Width, y % Height].IsBlocked;
        }

        //---------------------------------------------------------------------------

        public void SetIsBlocked(int x, int y, bool isBlocked)
        {
            m_Cells[x % Width, y % Height].IsBlocked = isBlocked;
        }

        //---------------------------------------------------------------------------

        public Rectangle GetTextureBounds(int x, int y, ELayerMode layer)
        {
            switch (layer)
            {
                case ELayerMode.First:
                    return m_Cells[x % Width, y % Height].Layer1;
                case ELayerMode.Second:
                    return m_Cells[x % Width, y % Height].Layer2;
                case ELayerMode.Third:
                    return m_Cells[x % Width, y % Height].Layer3;
            }
            return new Rectangle();
        }

        //---------------------------------------------------------------------------

        public void SetTextureBounds(int x, int y, ELayerMode layer, Rectangle bounds)
        {
            switch (layer)
            {
                case ELayerMode.First:
                    m_Cells[x % Width, y % Height].Layer1 = bounds;
                    break;
                case ELayerMode.Second:
                    m_Cells[x % Width, y % Height].Layer2 = bounds;
                    break;
                case ELayerMode.Third:
                    m_Cells[x % Width, y % Height].Layer3 = bounds;
                    break;
            }
        }

        //---------------------------------------------------------------------------

        //public IEntity CreateCollision()
        //{
        //    IEntity entity = EntityManager.Get().Find(m_CollisionEntity);

        //    if (entity == null)
        //    {
        //        entity = EntityFactory.Create<Entity>(string.Format("Chunk[{0}|{0}]", X, Y));
        //        entity.AddComponent<TransformComponent>();
        //        entity.AddComponent<PhysicsComponent>();
        //        entity.AddComponent<MultiPathColliderComponent>().Init();
        //    }
        //    MultiPathColliderComponent path = entity.GetComponent<MultiPathColliderComponent>();

        //    if (path != null)
        //    {
        //        path.Reset();
        //        for (int x = 0; x < Width - 1; x++)
        //        {
        //            Vector2 start = new Vector2((GlobalX(x) + 1) * 32, GlobalY(0) * 32);
        //            int length = 0;
        //            for (int y = 0; y < Height; y++)
        //            {
        //                if (m_Cells[x, y].IsBlocked != m_Cells[x + 1, y].IsBlocked)
        //                {
        //                    length++;
        //                }
        //                else
        //                {
        //                    if (length > 0)
        //                    {
        //                        path.AddPath(start, new Vector2((GlobalX(x) + 1) * 32, GlobalY(y) * 32));
        //                        length = 0;
        //                    }
        //                    start = new Vector2((GlobalX(x) + 1) * 32, (GlobalY(y) + 1) * 32);
        //                }
        //            }
        //            if (length > 0)
        //            {
        //                path.AddPath(start, new Vector2((GlobalX(x) + 1) * 32, GlobalY(Height) * 32));
        //            }
        //        }

        //        for (int y = 0; y < Height - 1; y++)
        //        {
        //            Vector2 start = new Vector2(GlobalX(0) * 32, (GlobalY(y) + 1) * 32);
        //            int length = 0;
        //            for (int x = 0; x < Width; x++)
        //            {
        //                if (m_Cells[x, y].IsBlocked != m_Cells[x, y + 1].IsBlocked)
        //                {
        //                    length++;
        //                }
        //                else
        //                {
        //                    if (length > 0)
        //                    {
        //                        path.AddPath(start, new Vector2(GlobalX(x) * 32, (GlobalY(y) + 1) * 32));
        //                        length = 0;
        //                    }
        //                    start = new Vector2((GlobalX(x) + 1) * 32, (GlobalY(y) + 1) * 32);
        //                }
        //            }
        //            if (length > 0)
        //            {
        //                path.AddPath(start, new Vector2(GlobalX(Width) * 32, (GlobalY(y) + 1) * 32));
        //            }
        //        }
        //    }
        //    return entity;
        //}

        public IEntity CreateCollision()
        {
            IEntity entity = EntityManager.Get().Find(m_CollisionEntity);

            if (entity == null)
            {
                entity = EntityFactory.Create<Entity>(string.Format("Chunk[{0}|{0}]", X, Y));
                entity.AddComponent<TransformComponent>();
                entity.AddComponent<PhysicsComponent>();
                entity.AddComponent<MultiPathColliderComponent>().Init();
            }
            MultiPathColliderComponent path = entity.GetComponent<MultiPathColliderComponent>();

            if (path != null)
            {
                path.Reset();
                for (int x = -1; x < Width - 1; x++)
                {
                    Vector2 start = new Vector2((GlobalX(x) + 1) * 32, GlobalY(0) * 32);
                    int length = 0;
                    for (int y = 0; y < Height; y++)
                    {
                        ChunkCell cell = null;
                        if (x < 0 && LeftChunk != null) cell = LeftChunk[Width + x, y];
                        else if (x >= 0) cell = m_Cells[x, y];

                        if (cell != null)
                        {
                            if (cell.IsBlocked != m_Cells[x + 1, y].IsBlocked)
                            {
                                length++;
                            }
                            else
                            {
                                if (length > 0)
                                {
                                    path.AddPath(start, new Vector2((GlobalX(x) + 1) * 32, GlobalY(y) * 32));
                                    length = 0;
                                }
                                start = new Vector2((GlobalX(x) + 1) * 32, (GlobalY(y) + 1) * 32);
                            }
                        }
                    }
                    if (length > 0)
                    {
                        path.AddPath(start, new Vector2((GlobalX(x) + 1) * 32, GlobalY(Height) * 32));
                    }
                }

                for (int y = -1; y < Height - 1; y++)
                {
                    Vector2 start = new Vector2(GlobalX(0) * 32, (GlobalY(y) + 1) * 32);
                    int length = 0;
                    for (int x = 0; x < Width; x++)
                    {
                        ChunkCell cell = null;
                        if (y < 0 && TopChunk != null) cell = TopChunk[x, Height + y];
                        else if (y >= 0) cell = m_Cells[x, y];

                        if (cell != null)
                        {
                            if (cell.IsBlocked != m_Cells[x, y + 1].IsBlocked)
                            {
                                length++;
                            }
                            else
                            {
                                if (length > 0)
                                {
                                    path.AddPath(start, new Vector2(GlobalX(x) * 32, (GlobalY(y) + 1) * 32));
                                    length = 0;
                                }
                                start = new Vector2((GlobalX(x) + 1) * 32, (GlobalY(y) + 1) * 32);
                            }
                        }
                    }
                    if (length > 0)
                    {
                        path.AddPath(start, new Vector2(GlobalX(Width) * 32, (GlobalY(y) + 1) * 32));
                    }
                }
            }
            return entity;
        }
    }

    //---------------------------------------------------------------------------

    public class ChunkCell
    {
        public bool IsBlocked { get; set; }
        public Rectangle Layer1 { get; set; }
        public Rectangle Layer2 { get; set; }
        public Rectangle Layer3 { get; set; }
    }
}
