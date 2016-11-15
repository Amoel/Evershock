using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level
{
    [Serializable]
    public class ViewRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        //---------------------------------------------------------------------------

        public ViewRect() { }

        //---------------------------------------------------------------------------

        public ViewRect(double x, double y)
            : this(x, y, 0, 0) { }

        //---------------------------------------------------------------------------

        public ViewRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
