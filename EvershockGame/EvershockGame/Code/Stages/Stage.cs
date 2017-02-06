using EvershockGame.Factory;
using EvershockGame.Manager;
using EvershockGame.Pathfinding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Stages
{
    public class Stage
    {
        private Random m_Rand;

        public static readonly int Size = 5;

        public List<Room> Rooms { get; private set; }
        public List<Corridor> Corridors { get; private set; }
        public List<Corner> Corners { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Room SpawnRoom { get; private set; }

        private byte[,] m_Map;

        private CorridorGenerator m_CorridorGenerator;

        //---------------------------------------------------------------------------

        public Stage(int seed)
        {
            m_Rand = (seed == 0 ? new Random() : new Random(seed));
            Rooms = new List<Room>();
            Corridors = new List<Corridor>();
            Corners = new List<Corner>();

            for (int i = 0; i < 40; i++)
            {
                Rooms.Add(CreateRoom(Size));
            }
            SpreadRooms(Rooms);
            ConnectRooms(Rooms);
            FindCorners(Rooms, Corridors);

            AreaManager.Get().Reset(Bounds.Width, Bounds.Height);
            foreach (Room room in Rooms)
            {
                Area area = EntityFactory.Create<Area>("Area");
                area.Collider.AddRect(room.Bounds.X, room.Bounds.Y, room.Bounds.Width, room.Bounds.Height);
            }
        }

        //---------------------------------------------------------------------------

        private Room CreateRoom(int size)
        {
            Point p = GetPointInCircle(50);
            return new Room(p.X, p.Y, m_Rand.Next(3, 10) * size, m_Rand.Next(3, 10) * size);
        }

        //---------------------------------------------------------------------------

        private void SpreadRooms(List<Room> rooms)
        {
            int padding = Size;
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
                            a.Shift(dxa + (dxa > 0 ? (Size - (dxa % Size)) : -(Size + (dxa % Size))), dya + (dya > 0 ? (Size - (dya % Size)) : -(Size + (dya % Size))));
                            b.Shift(dxb + (dxb > 0 ? (Size - (dxb % Size)) : -(Size + (dxb % Size))), dyb + (dyb > 0 ? (Size - (dyb % Size)) : -(Size + (dyb % Size))));
                            //a.Shift(dxa, dya);
                            //b.Shift(dxb, dyb);
                        }
                    }
                }
            }
            while (touching);

            UpdateBounds(rooms);
            foreach (Room room in rooms)
            {
                room.Shift(-Bounds.X, -Bounds.Y);
            }
        }

        //---------------------------------------------------------------------------

        private void ConnectRooms(List<Room> rooms)
        {
            rooms.RemoveAll(room => room.Bounds.Width * room.Bounds.Height < 130);
            rooms.Add(new Room(new Rectangle(Size, Size, Size * 3, Size * 3)));

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
        }

        //---------------------------------------------------------------------------

        private List<Corridor> GenerateCorridors(Dictionary<Room, List<Room>> graph)
        {
            List<Corridor> corridors = new List<Corridor>();

            m_CorridorGenerator = new CorridorGenerator(CreateMap(Size), Size);
            //m_CorridorGenerator = new CorridorGenerator(CreateMap(1), 1);

            foreach (KeyValuePair<Room, List<Room>> kvp in graph)
            {
                foreach (Room room in kvp.Value)
                {
                    Exit leftExit = null;
                    Exit rightExit = null;
                    Room.GetClosestExits(kvp.Key, room, out leftExit, out rightExit);

                    Corridor corridor = m_CorridorGenerator.Execute(leftExit, rightExit);
                    if (corridor != null) corridors.Add(corridor);
                }
            }
            return corridors;
        }

        //---------------------------------------------------------------------------

        private Corridor GenerateCorridor(Room first, Room second)
        {
            int dx, dy, x, y;
            Room a, b;

            if (first.Bounds.Center.X < first.Bounds.Center.X)
            {
                a = first;
                b = second;
            }
            else
            {
                a = second;
                b = first;
            }
            x = a.Bounds.Center.X;
            y = a.Bounds.Center.Y;
            dx = b.Bounds.Center.X - x;
            dy = b.Bounds.Center.Y - y;

            return new Corridor(new Point(x, y), new Point(x + dx, y + dy), 2);
        }

        //---------------------------------------------------------------------------

        private void FindCorners(List<Room> rooms, List<Corridor> corridors)
        {
            foreach (Room room in rooms)
            {
                Corners.Add(room.TopLeft());
                Corners.Add(room.TopRight());
                Corners.Add(room.BottomLeft());
                Corners.Add(room.BottomRight());
            }
            foreach (Corridor corridor in corridors)
            {
                Corners.AddRange(corridor.Corners);
            }
        }

        //---------------------------------------------------------------------------

        private void SetSpawnRoom(Room room)
        {
            room.RoomType = ERoomType.Spawn;
            SpawnRoom = room;
        }

        //---------------------------------------------------------------------------

        public byte[,] CreateMap(int size = 1)
        {
            byte[,] map = new byte[Bounds.Width / size, Bounds.Height / size];

            for (int x = 0; x < Bounds.Width / size; x++)
            {
                for (int y = 0; y < Bounds.Height / size; y++)
                {

                    map[x, y] = SeedManager.Get().NextRand(0, 2) == 0 ? (byte)0 : (byte)2;
                }
            }

            foreach (Room room in Rooms)
            {
                for (int x = room.Bounds.X; x < room.Bounds.X + room.Bounds.Width; x++)
                {
                    for (int y = room.Bounds.Y; y < room.Bounds.Y + room.Bounds.Height; y++)
                    {
                        map[x / size, y / size] = 255;
                    }
                }
            }
            foreach (Corridor corridor in Corridors)
            {
                foreach (CorridorSegment segment in corridor.Segments)
                {
                    int thickness = (segment.Thickness - 1) / 2;
                    for (int x = Math.Min(segment.Start.X, segment.End.X) - thickness; x <= Math.Max(segment.Start.X, segment.End.X) + thickness; x++)
                    {
                        for (int y = Math.Min(segment.Start.Y, segment.End.Y) - thickness; y <= Math.Max(segment.Start.Y, segment.End.Y) + thickness; y++)
                        {
                            if (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1)) map[x, y] = 255;
                        }
                    }
                }
            }
            return map;
        }

        //---------------------------------------------------------------------------

        public void SaveStageAsImage(string path)
        {
            int size = 5;
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
                for (int x = room.Bounds.X * size; x < (room.Bounds.X + room.Bounds.Width) * size; x++)
                {
                    for (int y = room.Bounds.Y * size; y < (room.Bounds.Y + room.Bounds.Height) * size; y++)
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }
            
            foreach (Corridor corridor in Corridors)
            {
                foreach (CorridorSegment segment in corridor.Segments)
                {
                    int thickness = (segment.Thickness - 1) / 2;
                    for (int x = (Math.Min(segment.Start.X, segment.End.X) - thickness) * size; x < (Math.Max(segment.Start.X, segment.End.X) + 1 + thickness) * size; x++)
                    {
                        for (int y = (Math.Min(segment.Start.Y, segment.End.Y) - thickness) * size; y < (Math.Max(segment.Start.Y, segment.End.Y) + 1 + thickness) * size; y++)
                        {
                            if (x >= 0 && x < image.Width && y >= 0 && y < image.Height) image.SetPixel(x, y, System.Drawing.Color.Black);
                        }
                    }
                    foreach (EDirection extension in segment.Extensions)
                    {
                        Rectangle bounds = new Rectangle();
                        switch (extension)
                        {
                            case EDirection.Left: bounds = new Rectangle(segment.Start.X - 2, segment.Start.Y - 1, 1, 3); break;
                            case EDirection.Right: bounds = new Rectangle(segment.Start.X + 2, segment.Start.Y - 1, 1, 3); break;
                            case EDirection.Up: bounds = new Rectangle(segment.Start.X - 1, segment.Start.Y - 2, 3, 1); break;
                            case EDirection.Down: bounds = new Rectangle(segment.Start.X - 1, segment.Start.Y + 2, 3, 1); break;
                        }
                        DrawRect(image, bounds, Size, System.Drawing.Color.Black);
                    }

                }
            }

            for (int x = 0; x < Bounds.Width * size; x++)
            {
                for (int y = 0; y < Bounds.Height * size; y++)
                {
                    if (x % (size * Size) == 0 || y % (size * Size) == 0) image.SetPixel(x, y, System.Drawing.Color.DarkGray);
                }
            }
            image.Save(path);
        }

        //---------------------------------------------------------------------------

        private void DrawRect(System.Drawing.Bitmap image, Rectangle bounds, int size, System.Drawing.Color color)
        {
            for (int x = bounds.X * size; x < (bounds.X + bounds.Width) * size; x++)
            {
                for (int y = bounds.Y * size; y < (bounds.Y + bounds.Height) * size; y++)
                {
                    if (x >= 0 && x < image.Width && y >= 0 && y < image.Height) image.SetPixel(x, y, color);
                }
            }
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

            Point temp = new Point((int)(radius * r * Math.Cos(t)), (int)(radius * r * Math.Sin(t)));
            return new Point(temp.X + (temp.X > 0 ? (5 - (temp.X % 5)) : -(5 + (temp.X % 5))), temp.Y + (temp.Y > 0 ? (5 - (temp.Y % 5)) : -(5 + (temp.Y % 5))));
        }
    }
}
