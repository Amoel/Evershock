using EntityComponent.Components;
using EvershockGame.Code.Factories;
using Microsoft.Xna.Framework;
using System;

namespace EvershockGame.Code.Components
{
    public class PickupSpawnerComponent : SpawnerComponent
    {
        Random rnd = new Random();

        public PickupSpawnerComponent (Guid entity) : base (entity) { }

        //---------------------------------------------------------------------------

        public void Spawn()
        {
            for (int i = 0; i < 12; i++)
            {
                int tempRand = m_Rand.Next(0, Enum.GetValues(typeof(EPickups)).Length);
                Spawn((EPickups)tempRand);
            }
        }

        //---------------------------------------------------------------------------

        public void Spawn(EPickups pickup)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                float rot = (float)(rnd.NextDouble() * Math.PI * 2.0f);
                float dist = (float)(rnd.NextDouble() * 60.0f + 200.0f);
                Vector3 force = new Vector3((float)Math.Sin(rot) * dist, (float)Math.Cos(rot) * dist, (float)(15.0f + rnd.NextDouble() * 8.0f));
                PickupFactory.Create(pickup, transform.Location, force);
            }
            
        }
    }
}
