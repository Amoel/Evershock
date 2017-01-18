using EntityComponent.Components;
using EntityComponent.Manager;
using EvershockGame.Code.Factories;
using Microsoft.Xna.Framework;
using System;

namespace EvershockGame.Code.Components
{
    public class PickupSpawnerComponent : SpawnerComponent
    {
        private Random m_Rand;

        public PickupSpawnerComponent (Guid entity) : base (entity)
        {
            m_Rand = new Random(SeedManager.Get().NextSeed());
        }

        //---------------------------------------------------------------------------

        public void Spawn()
        {
            int t_rnd = m_Rand.Next(5, 9);

            for (int i = 0; i < t_rnd; i++)
            {
                Spawn((EPickups)m_Rand.Next(0, Enum.GetValues(typeof(EPickups)).Length));
            }
        }

        //---------------------------------------------------------------------------

        public void Spawn(EPickups pickup)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                float rot = (SeedManager.Get().NextRandF() * (float)Math.PI * 2.0f);
                float dist = (SeedManager.Get().NextRandF() * 120.0f + 400.0f);
                Vector3 force = new Vector3((float)Math.Sin(rot) * dist, (float)Math.Cos(rot) * dist, SeedManager.Get().NextRandF(30f,46f));
                PickupFactory.Create(pickup, transform.Location, force);
            }
            
        }
    }
}
