using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Particles
{
    public class ParticleDesc
    {
        public float                 LifeTime { get; set; }
        public bool                  HasShadow { get; set; }

        public Func<float, Vector3>  Location { get; set; }
        public Func<float, Vector3>  Velocity { get; set; }
        public Func<float, Vector3>  Drift { get; set; }

        public Func<float, Color>    ParticleColor { get; set; }
        public Func<float, float>    ParticleOpacity { get; set; }
        public Func<float, Vector2>  ParticleSize { get; set; }

        public Func<float, Color>    LightColor { get; set; }
        public Func<float, float>    LightOpacity { get; set; }
        public Func<float, Vector2>  LightSize { get; set; }

        public Func<float, float>    Inertia { get; set; }
        public Func<float, float>    Gravity { get; set; }
        public Func<float, float>    Restitution { get; set; }

        //---------------------------------------------------------------------------

        public static ParticleDesc Default
        {
            get
            {
                return new ParticleDesc()
                {
                    LifeTime = 4.0f,
                    HasShadow = true,

                    ParticleColor = (time) => Color.Lerp(Color.Yellow, Color.Red, time),
                    ParticleOpacity = (time) => 1.0f - time,
                    ParticleSize = (time) => Vector2.One,
                    
                    LightColor = (time) => Color.Lerp(Color.Yellow, Color.Red, time),
                    LightOpacity = (time) => 1.0f - time,
                    LightSize = (time) => Vector2.One,

                    Inertia = (time) => 0.01f,
                    Gravity = (time) => 1.0f,
                    Restitution = (time) => 0.6f
                };
            }
        }
    }
}
