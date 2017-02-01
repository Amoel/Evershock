using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Particles
{
    public class CircleParticleEmitter : ParticleEmitter
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        //---------------------------------------------------------------------------

        public CircleParticleEmitter(Vector3 center, float radius, ParticleDesc desc = null) : base(EEmitterType.Circle, desc)
        {
            Center = center;
            Radius = radius;
        }

        //---------------------------------------------------------------------------

        protected override Vector3 NextLocation()
        {
            float rnd = (float)(m_Rand.NextDouble() * 2.0f * Math.PI);
            return new Vector3((float)(Center.X + Math.Sin(rnd) * Radius), (float)(Center.Y + Math.Cos(rnd) * Radius), Center.Z);
        }

        //---------------------------------------------------------------------------

        protected override Vector3 NextVelocity()
        {
            float theta = (float)(m_Rand.NextDouble() * 2.0f * Math.PI);
            float r = (float)Math.Sqrt(m_Rand.NextDouble());
            float z = (float)Math.Sqrt(1.0f - r * r) * (m_Rand.NextDouble() < 0.5f ? -1.0f : 1.0f);
            return new Vector3(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta), z);
        }
    }
}
