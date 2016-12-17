using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Stage
{
    public class Stage
    {
        private Random m_Rand;

        public List<Room> Rooms { get; private set; }
        public List<Corridor> Corridors { get; private set; }
        public Rectangle Bounds { get; private set; }

        public Room SpawnRoom { get; private set; }

        private byte[,] m_Map; 

        //---------------------------------------------------------------------------

        public Stage(int seed)
        {
            m_Rand = (seed == 0 ? new Random() : new Random(seed));
            Rooms = new List<Room>();
            Corridors = new List<Corridor>();

            for (int i = 0; i < 30; i++)
            {
                Point p = GetPointInCircle(15);
                Rooms.Add(new Room(p.X, p.Y, m_Rand.Next(5, 20), m_Rand.Next(5, 20)));
            }
            SpreadRooms(Rooms);
            ConnectRooms(Rooms);
        }

        //---------------------------------------------------------------------------

        private void SpreadRooms(List<Room> rooms)
        {
            int padding = 5;
            Room a, b;
            int dx, dxa, dxb, dy, dya, dyb;
            bool touching;
            do
            {
                touching = false;
                for (int i = 0; i < rooms.Count(); i++)
                {
                    a = rooms[i];
                    for (int j = i + 1; j < rooms.Count(); j++)
                    {
                        b = rooms[j];
                        if (a.Intersects(b, padding))
                        {
                            touching = true;
                            dx = Math.Min(a.Bounds.Right - b.Bounds.Left + padding, a.Bounds.Left - b.Bounds.Right - padding);
                            dy = Math.Min(a.Bounds.Bottom - b.Bounds.Top + padding, a.Bounds.Top - b.Bounds.Bottom - padding);
                            if (Math.Abs(dx) < Math.Abs(dy))
                            {
                                dy = 0;
                            }
                            else
                            {
                                dx = 0;
                            }
                            dxa = -dx / 2;
                            dxb = dx + dxa;
                            dya = -dy / 2;
                            dyb = dy + dya;
                            a.Shift(dxa, dya);
                            b.Shift(dxb, dyb);
                        }
                    }
                }
            }
            while (touching);
            UpdateBounds(rooms);
        }

        //---------------------------------------------------------------------------

        private void ConnectRooms(List<Room> rooms)
        {
            rooms.RemoveAll(room => room.Bounds.Width * room.Bounds.Height < 130);

            Dictionary<Room, List<Room>> graph = new Dictionary<Room, List<Room>>();
            Room a, b, c;
            double abDist, acDist, bcDist;
            bool skip;
            for (int i = 0; i < rooms.Count(); i++)
            {
                a = rooms[i];
                for (int j = i + 1; j < rooms.Count(); j++)
                {
                    skip = false;
                    b = rooms[j];
                    abDist = Math.Pow(a.Bounds.Center.X - b.Bounds.Center.X, 2) + Math.Pow(a.Bounds.Center.Y - b.Bounds.Center.Y, 2);
                    for (int k = 0; k < rooms.Count(); k++)
                    {
                        if (k == i || k == j)
                        {
                            continue;
                        }
                        c = rooms[k];
                        acDist = Math.Pow(a.Bounds.Center.X - c.Bounds.Center.X, 2) + Math.Pow(a.Bounds.Center.Y - c.Bounds.Center.Y, 2);
                        bcDist = Math.Pow(b.Bounds.Center.X - c.Bounds.Center.X, 2) + Math.Pow(b.Bounds.Center.Y - c.Bounds.Center.Y, 2);
                        if (acDist < abDist && bcDist < abDist)
                        {
                            skip = true;
                        }
                        if (skip)
                        {
                            break;
                        }
                    }
                    if (!skip)
                    {
                        if (!graph.ContainsKey(a))
                        {
                            graph.Add(a, new List<Room>());
                        }
                        graph[a].Add(b);
                    }
                }
            }
            Corridors = GenerateCorridors(graph);
            SetSpawnRoom(Rooms[m_Rand.Next(Rooms.Count)]);
            //StageManager.Get().Create(map);
        }

        //---------------------------------------------------------------------------

        private List<Corridor> GenerateCorridors(Dictionary<Room, List<Room>> graph)
        {
            List<Corridor> corridors = new List<Corridor>();

            int dx, dy, x, y;
            Room a, b;

            foreach (KeyValuePair<Room, List<Room>> kvp in graph)
            {
                foreach (Room room in kvp.Value)
                {
                    if (kvp.Key.Bounds.Center.X < room.Bounds.Center.X)
                    {
                        a = kvp.Key;
                        b = room;
                    }
                    else
                    {
                        a = room;
                        b = kvp.Key;
                    }
                    x = a.Bounds.Center.X;
                    y = a.Bounds.Center.Y;
                    dx = b.Bounds.Center.X - x;
                    dy = b.Bounds.Center.Y - y;

                    corridors.Add(new Corridor(new Point(x, y), new Point(x + dx, y + dy)));
                }
            }
            return corridors;
        }

        //---------------------------------------------------------------------------

        private void SetSpawnRoom(Room room)
        {
            room.RoomType = ERoomType.Spawn;
            SpawnRoom = room;
        }

        //---------------------------------------------------------------------------

        private void CreateMap()
        {
            bool[,] map = new bool[Bounds.Width, Bounds.Height];
            foreach (Room room in Rooms)
            {
                for (int x = room.Bounds.X - Bounds.X; x < room.Bounds.X - Bounds.X + room.Bounds.Width; x++)
                {
                    for (int y = room.Bounds.Y - Bounds.Y; y < room.Bounds.Y - Bounds.Y + room.Bounds.Height; y++)
                    {
                        map[x, y] = true;
                    }
                }
            }
            foreach (Corridor corridor in Corridors)
            {
                for (int x = Math.Min(corridor.Start.X, corridor.Center.X) - Bounds.X - 1; x <= Math.Max(corridor.Start.X, corridor.Center.X) - Bounds.X + 1; x++)
                {
                    for (int y = Math.Min(corridor.Start.Y, corridor.Center.Y) - Bounds.Y - 1; y <= Math.Max(corridor.Start.Y, corridor.Center.Y) - Bounds.Y + 1; y++)
                    {
                        map[x, y] = true;
                    }
                }
                for (int x = Math.Min(corridor.End.X, corridor.Center.X) - Bounds.X - 1; x <= Math.Max(corridor.End.X, corridor.Center.X) - Bounds.X + 1; x++)
                {
                    for (int y = Math.Min(corridor.End.Y, corridor.Center.Y) - Bounds.Y - 1; y <= Math.Max(corridor.End.Y, corridor.Center.Y) - Bounds.Y + 1; y++)
                    {
                        if (y < Bounds.Height) map[x, y] = true;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void SaveStageAsImage(string path)
        {
            int size = 10;
            System.Drawing.Bitmap image =
                new System.Drawing.Bitmap(Bounds.Width * size, Bounds.Height * size);
            foreach (Room room in Rooms)
            {
                System.Drawing.Color color;
                switch (room.RoomType)
                {
                    case ERoomType.Spawn:
                        color = System.Drawing.Color.FromArgb(185, 219, 48);
                        break;
                    case ERoomType.Boss:
                        color = System.Drawing.Color.FromArgb(219, 85, 48);
                        break;
                    default:
                        color = System.Drawing.Color.FromArgb(48, 159, 219);
                        break;
                }
                for (int x = (room.Bounds.X - Bounds.X) * size; x < (room.Bounds.X - Bounds.X + room.Bounds.Width) * size; x++)
                {
                    for (int y = (room.Bounds.Y - Bounds.Y) * size; y < (room.Bounds.Y - Bounds.Y + room.Bounds.Height) * size; y++)
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }
            foreach (Corridor corridor in Corridors)
            {
                for (int x = (Math.Min(corridor.Start.X, corridor.Center.X) - Bounds.X) * size; x < (Math.Max(corridor.Start.X, corridor.Center.X) - Bounds.X + 1) * size; x++)
                {
                    for (int y = (Math.Min(corridor.Start.Y, corridor.Center.Y) - Bounds.Y) * size; y < (Math.Max(corridor.Start.Y, corridor.Center.Y) - Bounds.Y + 1) * size; y++)
                    {
                        image.SetPixel(x, y, System.Drawing.Color.Black);
                    }
                }
                for (int x = (Math.Min(corridor.End.X, corridor.Center.X) - Bounds.X) * size; x < (Math.Max(corridor.End.X, corridor.Center.X) - Bounds.X + 1) * size; x++)
                {
                    for (int y = (Math.Min(corridor.End.Y, corridor.Center.Y) - Bounds.Y) * size; y < (Math.Max(corridor.End.Y, corridor.Center.Y) - Bounds.Y + 1) * size; y++)
                    {
                        image.SetPixel(x, y, System.Drawing.Color.Black);
                    }
                }
            }
            image.Save("C:/Users/Max/Desktop/Dungeon.png");
        }

        //---------------------------------------------------------------------------

        private void CheckMap(int x, int y)
        {
            if (m_Map == null) return;
        }

        //---------------------------------------------------------------------------

        private void UpdateBounds(List<Room> rooms)
        {
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;
            foreach (Room room in rooms)
            {
                minX = Math.Min(minX, room.Bounds.Left);
                maxX = Math.Max(maxX, room.Bounds.Right);
                minY = Math.Min(minY, room.Bounds.Top);
                maxY = Math.Max(maxY, room.Bounds.Bottom);
            }
            Bounds = new Rectangle(minX - 10, minY - 10, (maxX - minX) + 20, (maxY - minY) + 20);
            m_Map = new byte[Bounds.Width, Bounds.Height];
        }

        //---------------------------------------------------------------------------

        private Point GetPointInCircle(float radius)
        {
            float t = (float)(2.0f * Math.PI * m_Rand.NextDouble());
            float u = (float)(m_Rand.NextDouble() + m_Rand.NextDouble());
            float r = (u > 1.0f ? 2.0f - u : u);
            return new Point((int)(radius * r * Math.Cos(t)), (int)(radius * r * Math.Sin(t)));
        }
    }
}
