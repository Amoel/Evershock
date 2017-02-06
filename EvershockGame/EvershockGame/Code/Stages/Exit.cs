using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Stages
{
    public class Exit
    {
        public Room Room { get; private set; }
        public EDirection Direction { get; private set; }

        public bool IsUsed { get; set; }

        //---------------------------------------------------------------------------

        public Exit(Room room, EDirection direction)
        {
            Room = room;
            Direction = direction;
        }

        //---------------------------------------------------------------------------

        public Point GetLocation()
        {
            switch (Direction)
            {
                case EDirection.Left: return new Point(Room.Bounds.Left - 1, Room.Bounds.Center.Y);
                case EDirection.Right: return new Point(Room.Bounds.Right, Room.Bounds.Center.Y);
                case EDirection.Up: return new Point(Room.Bounds.Center.X, Room.Bounds.Top - 1);
                case EDirection.Down: return new Point(Room.Bounds.Center.X, Room.Bounds.Bottom);
            }
            return Point.Zero;
        }
    }
}
