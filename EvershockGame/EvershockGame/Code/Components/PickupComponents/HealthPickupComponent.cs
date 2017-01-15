using EntityComponent;
using EntityComponent.Components;
using System;

namespace EvershockGame.Code.Components
{
    public class HealthPickupComponent : Component, IPickupComponent
    {
        public HealthPickupComponent(Guid entity) : base(entity)
        {

        }

        public void OnPickup(IEntity collector)
        {
            if (collector is Player)
            {
                GetComponent<DespawnComponent>().Trigger();
            }
        }
    }
}
