﻿using EntityComponent;
using EntityComponent.Components;
using System;

namespace EvershockGame.Code.Components
{
    public class HealthPickupComponent : Component, IPickupComponent
    {
        public HealthPickupComponent(Guid entity) : base(entity)
        {

        }

        //---------------------------------------------------------------------------

        public void OnPickup(IEntity collector)
        {
            if (collector != null)
            {
                if (collector is Player)
                {
                    DespawnComponent despawn = GetComponent<DespawnComponent>();
                    if (despawn != null)
                    {
                        despawn.Trigger();
                    }
                }
            }
        }
    }
}
