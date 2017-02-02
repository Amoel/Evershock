using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityComponent.Particles
{
    public class PointParticleEmitter : ParticleEmitter
    {
        public PointParticleEmitter(Vector3 center, ParticleDesc desc = null) : base(EEmitterType.Point, desc)
        {
            Center = center;
        }

        //---------------------------------------------------------------------------

        protected override Vector3 NextLocation()
        {
            return Center;
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
