using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Particles
{
    public class Particle
    {
        public Vector3 Location { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 Drift { get; private set; }
        public float LifeTime { get; private set; }
        public float MaxLifeTime { get; private set; }
        public float RelativeLifeTime { get; private set; }

        //---------------------------------------------------------------------------

        public Particle(Vector3 location, Vector3 velocity, float maxLifeTime)
        {
            Location = location;
            Velocity = velocity;
            MaxLifeTime = maxLifeTime;
        }

        //---------------------------------------------------------------------------

        public bool Update(float deltaTime, ParticleDesc desc)
        {
            LifeTime += deltaTime;
            RelativeLifeTime = (LifeTime / MaxLifeTime);
            if (LifeTime >= MaxLifeTime) return true;

            Velocity = (Velocity - Vector3.UnitZ * desc.Gravity(RelativeLifeTime)) * (1.0f - desc.Inertia(RelativeLifeTime));

            if (Drift.Length() > 0)
            {
                Matrix rotMatrix = Matrix.CreateFromAxisAngle(Vector3.Normalize(Drift), MathHelper.ToRadians(Drift.Length()));
                Velocity = Vector3.Transform(Velocity, rotMatrix);
            }

            Location += Velocity;
            if (Location.Z < 0)
            {
                Location = new Vector3(Location.X, Location.Y, 0);
                Velocity = new Vector3(Velocity.X, Velocity.Y, -Velocity.Z * desc.Restitution(RelativeLifeTime));
            }

            return false;
        }
    }
}
