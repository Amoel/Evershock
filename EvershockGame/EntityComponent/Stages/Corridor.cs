using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Stages
{
    public enum EDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    //---------------------------------------------------------------------------

    public class Corridor
    {
        public List<Room> ConnectedRooms { get; private set; }
        public List<Corner> Corners { get; private set; }
        public List<CorridorSegment> Segments { get; private set; }

        public Point Start { get; set; }
        public Point End { get; set; }
        public int Thickness { get; set; }

        public Rectangle Bounds { get; private set; }

        //---------------------------------------------------------------------------

        public Corridor()
        {
            ConnectedRooms = new List<Room>();
            Corners = new List<Corner>();
            Segments = new List<CorridorSegment>();
        }

        //---------------------------------------------------------------------------

        public Corridor(Point start, Point end, int thickness)
        {
            ConnectedRooms = new List<Room>();
            Corners = new List<Corner>();
            Segments = new List<CorridorSegment>();

            Start = start;
            End = end;
            Thickness = thickness;

            Bounds = new Rectangle(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y), Math.Abs(Start.X - End.X), Math.Abs(Start.Y - End.Y));
        }

        //---------------------------------------------------------------------------

        public void AddCorners()
        {
            foreach (CorridorSegment segment in Segments)
            {
                Corners.Add(new Corner(new Point(segment.End.X - 2, segment.End.Y - 2), new Vector2(0.0f, -5.0f)));
                Corners.Add(new Corner(new Point(segment.End.X - 2, segment.Start.Y + 3), new Vector2(0.0f, 5.0f)));
                Corners.Add(new Corner(new Point(segment.Start.X + 3, segment.End.Y - 2), new Vector2(0.0f, -5.0f)));
                Corners.Add(new Corner(new Point(segment.Start.X + 3, segment.Start.Y + 3), new Vector2(0.0f, 5.0f)));

                Corners.Add(new Corner(new Point(segment.End.X - 2, segment.End.Y - 2), new Vector2(0.0f, 5.0f)));
                Corners.Add(new Corner(new Point(segment.End.X - 2, segment.Start.Y + 3), new Vector2(0.0f, -5.0f)));
                Corners.Add(new Corner(new Point(segment.Start.X + 3, segment.End.Y - 2), Vector2.Zero));
                Corners.Add(new Corner(new Point(segment.Start.X + 3, segment.Start.Y + 3), Vector2.Zero));
            }
        }

        //---------------------------------------------------------------------------

        public CorridorSegment AddSegment(Point start, Point end, int thickness)
        {
            if (Segments.Count > 0)
            {
                CorridorSegment segment = Segments.Last();
                if (GetDirection(segment.Start, segment.End) == GetDirection(start, end))
                {
                    segment.End = end;
                    return segment;
                }
            }
            CorridorSegment newSegment = new CorridorSegment(start, end, thickness);
            Segments.Add(newSegment);
            return newSegment;
        }

        //---------------------------------------------------------------------------

        private EDirection GetDirection(Point start, Point end)
        {
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;

            if (dx > 0) return EDirection.Right;
            else if (dx < 0) return EDirection.Left;
            else if (dy > 0) return EDirection.Down;
            else if (dy < 0) return EDirection.Up;
            else return EDirection.None;
        }
    }

    //---------------------------------------------------------------------------

    public class CorridorSegment
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public int Thickness { get; set; }
        public List<EDirection> Extensions { get; set; }

        //---------------------------------------------------------------------------

        public CorridorSegment(Point start, Point end, int thickness)
        {
            Start = start;
            End = end;
            Thickness = thickness;
            Extensions = new List<EDirection>();
        }

        //---------------------------------------------------------------------------

        public void AddExtension(EDirection extension)
        {
            Extensions.Add(extension);
        }
    }
}
