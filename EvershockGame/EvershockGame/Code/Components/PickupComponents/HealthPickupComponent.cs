﻿using EntityComponent;
using EntityComponent.Components;
using System;

namespace EvershockGame.Code.Components
{
    public class HealthPickupComponent : PickupComponent, IPickupComponent
    {
        public HealthPickupComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void OnPickup(IEntity collector)
        {
            if (collector != null && IsCollectable)
            {
                if (collector is Player)
                {
                    AttributesComponent attributes = collector.GetComponent<AttributesComponent>();
                    attributes.ReplenishHealth(15);

                    DespawnComponent despawn = GetComponent<DespawnComponent>();
                    if (despawn != null)
                    {
                        despawn.Trigger();
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}