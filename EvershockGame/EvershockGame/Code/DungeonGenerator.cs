using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class DungeonGenerator
    {
        private Random m_Rand;

        //---------------------------------------------------------------------------

        public DungeonGenerator() { }

        //---------------------------------------------------------------------------

        public void Run(int seed = 0)
        {
            m_Rand = (seed != 0 ? new Random(seed) : new Random());

            List<Room> rooms = new List<Room>();
            for (int i = 0; i < 40; i++)
            {
                Point p = GetPointInCircle(15);
                rooms.Add(new Room(p.X, p.Y, m_Rand.Next(5, 20), m_Rand.Next(5, 20)));
            }
            SpreadRooms(rooms);
            ConnectRooms(rooms);
        }

        //---------------------------------------------------------------------------

        private void SpreadRooms(List<Room> rooms)
        {
            int padding = 2;
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
        }

        //---------------------------------------------------------------------------

        private void ConnectRooms(List<Room> rooms)
        {
            rooms.RemoveAll(room => !room.IsMajorRoom);
            Rectangle bounds = GetDungeonBounds(rooms);
            rooms.Add(new Room(bounds.X + 10, bounds.Y + 10, 9, 9));

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

            List<Corridor> corridors = GenerateCorridors(graph);

            Rectangle dungeonBounds = GetDungeonBounds(rooms);

            bool[,] map = new bool[dungeonBounds.Width, dungeonBounds.Height];
            foreach (Room room in rooms)
            {
                for (int x = room.Bounds.X - dungeonBounds.X; x < room.Bounds.X - dungeonBounds.X + room.Bounds.Width; x++)
                {
                    for (int y = room.Bounds.Y - dungeonBounds.Y; y < room.Bounds.Y - dungeonBounds.Y + room.Bounds.Height; y++)
                    {
                        map[x, y] = true;
                    }
                }
            }
            foreach (Corridor corridor in corridors)
            {
                for (int x = Math.Min(corridor.Start.X, corridor.Center.X) - dungeonBounds.X - 1; x <= Math.Max(corridor.Start.X, corridor.Center.X) - dungeonBounds.X + 1; x++)
                {
                    for (int y = Math.Min(corridor.Start.Y, corridor.Center.Y) - dungeonBounds.Y - 1; y <= Math.Max(corridor.Start.Y, corridor.Center.Y) - dungeonBounds.Y + 1; y++)
                    {
                        map[x, y] = true;
                    }
                }
                for (int x = Math.Min(corridor.End.X, corridor.Center.X) - dungeonBounds.X - 1; x <= Math.Max(corridor.End.X, corridor.Center.X) - dungeonBounds.X + 1; x++)
                {
                    for (int y = Math.Min(corridor.End.Y, corridor.Center.Y) - dungeonBounds.Y - 1; y <= Math.Max(corridor.End.Y, corridor.Center.Y) - dungeonBounds.Y + 1; y++)
                    {
                        if (y < dungeonBounds.Height) map[x, y] = true;
                    }
                }
            }

            StageManager.Get().Create(map);
            //System.Drawing.Bitmap image =
            //    new System.Drawing.Bitmap(dungeonBounds.Width * 10, dungeonBounds.Height * 10);
            //foreach (Room room in rooms)
            //{
            //    System.Drawing.Color color = System.Drawing.Color.FromArgb(0, 150, 80);
            //    for (int x = (room.Bounds.X - dungeonBounds.X) * 10; x < (room.Bounds.X - dungeonBounds.X + room.Bounds.Width) * 10; x++)
            //    {
            //        for (int y = (room.Bounds.Y - dungeonBounds.Y) * 10; y < (room.Bounds.Y - dungeonBounds.Y + room.Bounds.Height) * 10; y++)
            //        {
            //            image.SetPixel(x, y, color);
            //        }
            //    }
            //}
            //image.Save("C:/Users/Max/Desktop/Dungeon.png");
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
                    
                    if (dx >= 0)
                    {
                        if (dy >= 0)
                        {
                            corridors.Add(new Corridor(new Point(x, y), new Point(x + dx, y + dy)));
                        }
                        else
                        {
                            corridors.Add(new Corridor(new Point(x, y + dy), new Point(x + dx, y - dy)));
                        }
                    }
                    else
                    {
                        if (dy >= 0)
                        {
                            corridors.Add(new Corridor(new Point(x + dx, y), new Point(x - dx, y + dy)));
                        }
                        else
                        {
                            corridors.Add(new Corridor(new Point(x + dx, y + dy), new Point(x - dx, y - dy)));
                        }
                    }
                }
            }
            return corridors;
        }

        //---------------------------------------------------------------------------

        private Point GetPointInCircle(float radius)
        {
            float t = (float)(2.0f * Math.PI * m_Rand.NextDouble());
            float u = (float)(m_Rand.NextDouble() + m_Rand.NextDouble());
            float r = (u > 1.0f ? 2.0f - u : u);
            return new Point((int)(radius * r * Math.Cos(t)), (int)(radius * r * Math.Sin(t)));
        }

        //---------------------------------------------------------------------------

        private Rectangle GetDungeonBounds(List<Room> rooms)
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
            return new Rectangle(minX - 10, minY - 10, (maxX - minX) + 20, (maxY - minY) + 20);
        }

        //---------------------------------------------------------------------------

        class Room
        {
            public bool IsMajorRoom { get { return Bounds.Width * Bounds.Height >= 130; } }
            public Rectangle Bounds { get; set; }

            //---------------------------------------------------------------------------

            public Room(Rectangle bounds)
            {
                Bounds = bounds;
            }

            //---------------------------------------------------------------------------

            public Room(int x, int y, int width, int height)
            {
                Bounds = new Rectangle(x, y, width, height);
            }

            //---------------------------------------------------------------------------

            public void Shift(int x, int y)
            {
                Bounds = new Rectangle(Bounds.X + x, Bounds.Y + y, Bounds.Width, Bounds.Height);
            }

            //---------------------------------------------------------------------------

            public bool Intersects(Room other, int padding)
            {
                return new Rectangle(Bounds.X - padding, Bounds.Y - padding, Bounds.Width + padding * 2, Bounds.Height + padding * 2).Intersects(other.Bounds);
            }
        }

        //---------------------------------------------------------------------------

        class Corridor
        {
            public Point Start { get; set; }
            public Point End { get; set; }

            public Point Center { get { return new Point(End.X, Start.Y); } }
            public Rectangle Bounds { get { return new Rectangle(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y), Math.Abs(Start.X - End.X), Math.Abs(Start.Y - End.Y)); } }

            //---------------------------------------------------------------------------

            public Corridor(Point start, Point end)
            {
                Start = start;
                End = end;
            }
        }
    }
}
