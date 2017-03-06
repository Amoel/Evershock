using EvershockGame.Components;
using EvershockGame.Manager;
using EvershockGame.Code.Factories;
using Microsoft.Xna.Framework;
using System;
using EvershockGame.Code.Manager;
using EvershockGame.Code.Items;

namespace EvershockGame.Code.Components
{
    public class PickupSpawnerComponent : SpawnerComponent
    {
        public PickupSpawnerComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Spawn()
        {
            int t_rnd = m_Rand.Next(5, 9);

            for (int i = 0; i < t_rnd; i++)
            {
                EPickups type = (EPickups)m_Rand.Next(0, Enum.GetValues(typeof(EPickups)).Length);

                if (type == EPickups.Item)
                {
                    EItemType item = ItemManager.Get().Next(EItemPool.SmallChest);
                    if (item != EItemType.None)
                    {
                        Spawn(item);
                    }
                }
                else
                {
                    Spawn(type);
                }
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
                Vector3 force = new Vector3((float)Math.Sin(rot) * dist, (float)Math.Cos(rot) * dist, SeedManager.Get().NextRandF(20, 40));
                PickupFactory.Create(pickup, transform.Location, force);
            }
        }

        //---------------------------------------------------------------------------

        public void Spawn(EItemType item)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                float rot = (SeedManager.Get().NextRandF() * (float)Math.PI * 2.0f);
                float dist = (SeedManager.Get().NextRandF() * 120.0f + 400.0f);
                Vector3 force = new Vector3((float)Math.Sin(rot) * dist, (float)Math.Cos(rot) * dist, SeedManager.Get().NextRandF(20, 40));
                PickupFactory.Create(item, transform.Location, force);
            }
        }
    }
}
