using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Stages
{
    public class Corner
    {
        public Point Location { get; set; }
        public Vector2 Normal { get; set; }

        public Vector2 AbsoluteLocation { get { return new Vector2(Location.X * 32 - Normal.X, Location.Y * 32 - Normal.Y); } }

        //---------------------------------------------------------------------------

        public Corner(Point location, Vector2 normal)
        {
            Location = location;
            Normal = normal;
        }
    }
}
