using EntityComponent;
using EntityComponent.Components;
using System;

namespace EvershockGame.Code.Components
{
    public class CoinPickupComponent : PickupComponent, IPickupComponent
    {
        public CoinPickupComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        //of course, this needs some content. Currently works exactly as the HealthPickupComponent

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