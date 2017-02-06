using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvershockGame
{
    public enum ECameraTargetGroup
    {
        None,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten
    }

    //---------------------------------------------------------------------------

    public class CameraTarget
    {
        public Guid Target { get; private set; }
        public ECameraTargetGroup Group { get; private set; }
        public float Distance { get; private set; }

        //---------------------------------------------------------------------------

        public CameraTarget(Guid target, ECameraTargetGroup group, float distance)
        {
            Target = target;
            Group = group;
            Distance = distance;
        }
    }
}
