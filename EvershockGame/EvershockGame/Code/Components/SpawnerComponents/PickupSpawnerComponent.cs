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
            int t_rnd = rnd.Next(7, 21);

            for (int i = 0; i < t_rnd; i++)
            {
                Spawn((EPickups)rnd.Next(0, Enum.GetValues(typeof(EPickups)).Length));
            }
        }

        //---------------------------------------------------------------------------

        public void Spawn(EPickups pickup)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                float rot = (float)(rnd.NextDouble() * Math.PI * 2.0f);
                float dist = (float)(rnd.NextDouble() * 120.0f + 400.0f);
                Vector3 force = new Vector3((float)Math.Sin(rot) * dist, (float)Math.Cos(rot) * dist, (float)(30.0f + rnd.NextDouble() * 16.0f));
                PickupFactory.Create(pickup, transform.Location, force);
            }
            
        }
    }
}
