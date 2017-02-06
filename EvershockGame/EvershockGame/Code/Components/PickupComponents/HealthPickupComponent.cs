using EvershockGame;
using EvershockGame.Components;
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
                    PlayerAttributesComponent attributes = collector.GetComponent<PlayerAttributesComponent>();
                    attributes.ReplenishHealth(25);

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
