using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public struct BulletDesc
    {
        public float Velocity { get; set; }
        public float Damage { get; set; }
        public float Gravity { get; set; }

        //---------------------------------------------------------------------------

        public static BulletDesc Default
        {
            get
            {
                return new BulletDesc()
                {
                    Velocity = 600.0f,
                    Damage = 10.0f,
                    Gravity = 0.0f
                };
            }
        }
    }
}
