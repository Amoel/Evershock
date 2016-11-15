using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public class SelectionRect
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        //---------------------------------------------------------------------------

        public SelectionRect(int x, int y) : this(x, y, 0, 0) { }

        //---------------------------------------------------------------------------

        public SelectionRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        //---------------------------------------------------------------------------

        public SelectionRect(int x, int y, int width, int height, SelectionRect bounds)
        {
            Update(x, y, width, height, bounds);
        }

        //---------------------------------------------------------------------------

        public void Update(int x, int y, int width, int height, SelectionRect bounds)
        {
            X = (bounds != null ? Math.Min(bounds.X + bounds.Width - 1, Math.Max(x, bounds.X)) : x);
            Y = (bounds != null ? Math.Min(bounds.Y + bounds.Height - 1, Math.Max(y, bounds.Y)) : y);
            Width = (bounds != null ? Math.Min(Math.Max(width, 1), bounds.X + bounds.Width - X) : width);
            Height = (bounds != null ? Math.Min(Math.Max(height, 1), bounds.Y + bounds.Height - Y) : height);
        }

        //---------------------------------------------------------------------------

        public SelectionRect Within(SelectionRect bounds)
        {
            int x = (bounds != null ? Math.Min(bounds.X + bounds.Width - 1, Math.Max(X, bounds.X)) : X);
            int y = (bounds != null ? Math.Min(bounds.Y + bounds.Height - 1, Math.Max(Y, bounds.Y)) : Y);
            int width = (bounds != null ? Math.Min(Math.Max(Width, 1), bounds.X + bounds.Width - X) : Width);
            int height = (bounds != null ? Math.Min(Math.Max(Height, 1), bounds.Y + bounds.Height - Y) : Height);
            return new SelectionRect(x, y, width, height);
        }
    }
}
