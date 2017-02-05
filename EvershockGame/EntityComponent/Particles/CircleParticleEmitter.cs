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
        public float InnerRadius { get; set; }
        public float OuterRadius { get; set; }

        //---------------------------------------------------------------------------

        public CircleParticleEmitter(float innerRadius, float outerRadius, ParticleDesc desc = null) : base(EEmitterType.Circle, desc)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }

        //---------------------------------------------------------------------------

        protected override Vector3 NextLocation()
        {
            float rnd = (float)(m_Rand.NextDouble() * 2.0f * Math.PI);
            float radius = (float)(InnerRadius + m_Rand.NextDouble() * (OuterRadius - InnerRadius));
            return new Vector3((float)(Center.X + Math.Sin(rnd) * radius), (float)(Center.Y + Math.Cos(rnd) * radius), Center.Z);
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
