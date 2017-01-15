using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Stages
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

        public List<Exit> Exits { get; private set; }

        public Corner TopLeft()
        {
            return new Corner(new Point(Bounds.X, Bounds.Y), new Vector2(1, 1));
        }

        public Corner TopRight()
        {
            return new Corner(new Point(Bounds.X + Bounds.Width, Bounds.Y), new Vector2(-1, 1));
        }

        public Corner BottomLeft()
        {
            return new Corner(new Point(Bounds.X, Bounds.Y + Bounds.Height), new Vector2(1, -1));
        }

        public Corner BottomRight()
        {
            return new Corner(new Point(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height), new Vector2(-1, -1));
        }

        //---------------------------------------------------------------------------

        public Room(Rectangle bounds)
        {
            Bounds = bounds;
            RoomType = ERoomType.Normal;

            Exits = new List<Exit>()
            {
                new Exit(this, EDirection.Left),
                new Exit(this, EDirection.Right),
                new Exit(this, EDirection.Up),
                new Exit(this, EDirection.Down)
            };
        }

        //---------------------------------------------------------------------------

        public Room(int x, int y, int width, int height)
        {
            Bounds = new Rectangle(x, y, width, height);
            RoomType = ERoomType.Normal;

            Exits = new List<Exit>()
            {
                new Exit(this, EDirection.Left),
                new Exit(this, EDirection.Right),
                new Exit(this, EDirection.Up),
                new Exit(this, EDirection.Down)
            };
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

        //---------------------------------------------------------------------------

        public static void GetClosestExits(Room left, Room right, out Exit leftExit, out Exit rightExit)
        {
            leftExit = null;
            rightExit = null;

            float minDistance = float.MaxValue;
            foreach (Exit l in left.Exits)
            {
                foreach (Exit r in right.Exits)
                {
                    float distance = Vector2.Distance(l.GetLocation().ToVector2(), r.GetLocation().ToVector2());
                    if (distance < minDistance)
                    {
                        leftExit = l;
                        rightExit = r;
                        minDistance = distance;
                    }
                }
            }
        }
    }
}
