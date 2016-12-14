using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Stage
{
    public class Corridor
    {
        public List<CorridorSegment> Segments { get; private set; }

        public Point Start { get; set; }
        public Point End { get; set; }

        public Point Center { get { return new Point(End.X, Start.Y); } }
        public Rectangle Bounds { get { return new Rectangle(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y), Math.Abs(Start.X - End.X), Math.Abs(Start.Y - End.Y)); } }

        //---------------------------------------------------------------------------

        public Corridor(Point start, Point end)
        {
            Segments = new List<CorridorSegment>();
            Start = start;
            End = end;
        }

        //---------------------------------------------------------------------------

        public void AddSegment(Point start, Point end)
        {
            Segments.Add(new CorridorSegment(start, end));
        }
    }

    //---------------------------------------------------------------------------

    public class CorridorSegment
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        //---------------------------------------------------------------------------

        public CorridorSegment(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }
}
