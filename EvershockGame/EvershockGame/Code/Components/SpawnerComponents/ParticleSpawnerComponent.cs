using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using EvershockGame.Code.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class ParticleSpawnerComponent : SpawnerComponent, ITickableComponent
    {
        private float m_Time = 0;

        //---------------------------------------------------------------------------

        public ParticleSpawnerComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            Spawn(EParticles.Spark);
            //m_Time += deltaTime;
            //if (m_Time >= 1.0f)
            //{
            //    Spawn(EParticles.Spark);
            //    m_Time -= 1.0f;
            //}
        }

        //---------------------------------------------------------------------------

        public void Spawn()
        {
            int t_rnd = m_Rand.Next(5, 9);

            for (int i = 0; i < t_rnd; i++)
            {
                Spawn((EParticles)m_Rand.Next(0, Enum.GetValues(typeof(EParticles)).Length));
            }
        }

        //---------------------------------------------------------------------------

        public void Spawn(EParticles particle)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                float rot = (SeedManager.Get().NextRandF() * (float)Math.PI * 2.0f);
                float dist = (SeedManager.Get().NextRandF() * 4.0f + 2.0f);
                Vector3 force = new Vector3((float)Math.Sin(rot) * dist + 10, (float)Math.Cos(rot) * dist, SeedManager.Get().NextRandF(30f, 46f));
                ParticleFactory.Create(particle, transform.Location, force);
            }

        }
    }
}
