using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Stage
{
    public enum ERoomType
    {
        Normal,
        Spawn,
        Boss
    }

    //---------------------------------------------------------------------------

    public class Room
    {
        public Rectangle Bounds { get; set; }
        public ERoomType RoomType { get; set; }

        //---------------------------------------------------------------------------

        public Room(Rectangle bounds)
        {
            Bounds = bounds;
            RoomType = ERoomType.Normal;
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
}
