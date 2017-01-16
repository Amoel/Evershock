using EntityComponent.Components;
using EvershockGame.Code.Factories;
using System;

namespace EvershockGame.Code.Components
{
    public class PickupSpawnerComponent : SpawnerComponent
    {
        public PickupSpawnerComponent (Guid entity) : base (entity) { }

        public void Spawn()
        {
            int tempRand = m_Rand.Next(0, Enum.GetValues(typeof(EPickups)).Length);
            Spawn((EPickups)tempRand);
        }

        public void Spawn(EPickups pickup)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                PickupFactory.Create(pickup, transform.Location + new Microsoft.Xna.Framework.Vector3(60,0,0));
            }
            
        }
    }
}
