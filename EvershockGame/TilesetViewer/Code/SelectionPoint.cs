using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public class SelectionPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        //---------------------------------------------------------------------------

        public SelectionPoint() { }

        //---------------------------------------------------------------------------

        public SelectionPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        //---------------------------------------------------------------------------

        public static SelectionPoint operator+(SelectionPoint first, SelectionPoint second)
        {
            return new SelectionPoint(first.X + second.X, first.Y + second.Y);
        }

        //---------------------------------------------------------------------------

        public static SelectionPoint operator-(SelectionPoint first, SelectionPoint second)
        {
            return new SelectionPoint(first.X - second.X, first.Y - second.Y);
        }

        //---------------------------------------------------------------------------

        public override bool Equals(object obj)
        {
            return ((SelectionPoint)obj).X == X && ((SelectionPoint)obj).Y == Y;
        }
    }
}
