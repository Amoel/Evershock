using Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TilesetViewer
{
    public static class Extensions
    {
        public static Rect ToRect(this ViewRect view)
        {
            return new Rect(view.X, view.Y, view.Width, view.Height);
        }

        //---------------------------------------------------------------------------

        public static ViewRect ToView(this Rect rect)
        {
            return new ViewRect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
