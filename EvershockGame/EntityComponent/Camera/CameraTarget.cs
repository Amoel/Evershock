using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent
{
    public enum ECameraPriority
    {
        VeryHigh,
        High,
        Medium,
        Low
    }

    //---------------------------------------------------------------------------

    public class CameraTarget
    {
        public Guid Target { get; private set; }
        public ECameraPriority Priority { get; private set; }
        public float Distance { get; private set; }

        //---------------------------------------------------------------------------

        public CameraTarget(Guid target, ECameraPriority priority, float distance)
        {
            Target = target;
            Priority = priority;
            Distance = distance;
        }
    }
}
