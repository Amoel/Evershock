using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Particles
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

        public RandomnessDesc        Random { get; private set; }

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

        //---------------------------------------------------------------------------

        public static ParticleDesc Fire
        {
            get
            {
                return new ParticleDesc()
                {
                    LifeTime = 0.5f,
                    HasShadow = false,

                    ParticleColor = (time) => Color.Lerp(Color.Orange, Color.Red, time),
                    ParticleOpacity = (time) => time < 0.5f ? 1.0f : (1.0f - time) * 2.0f,
                    ParticleSize = (time) => new Vector2((1.5f - time) * 4, (1.5f - time) * 6),

                    LightColor = (time) => Color.Lerp(Color.Orange, Color.Red, time),
                    LightOpacity = (time) => time < 0.5f ? 1.0f : (1.0f - time) * 2.0f,
                    LightSize = (time) => new Vector2((1.5f - time) * 0.05f, (1.5f - time) * 0.05f),

                    Inertia = (time) => 0.01f,
                    Gravity = (time) => -0.3f,
                    Restitution = (time) => 0.0f
                };
            }
        }

        //---------------------------------------------------------------------------

        public static ParticleDesc Stars
        {
            get
            {
                return new ParticleDesc()
                {
                    LifeTime = 1.0f,
                    HasShadow = false,

                    ParticleColor = (time) => Color.White,
                    ParticleOpacity = (time) => (float)Math.Sin(time * Math.PI),
                    ParticleSize = (time) => Vector2.One * 2,

                    LightColor = (time) => Color.White,
                    LightOpacity = (time) => (float)Math.Sin(time * Math.PI),
                    LightSize = (time) => Vector2.One * 4,

                    Inertia = (time) => 1.0f,
                    Gravity = (time) => 0.0f,
                    Restitution = (time) => 0.0f
                };
            }
        }
    }

    //---------------------------------------------------------------------------

    public struct RandomnessDesc
    {
        public float LifeTime { get; set; }

        public float Inertia { get; set; }
        public float Gravity { get; set; }
        public float Restitution { get; set; }
    }
}
