using EntityComponent;
using EntityComponent.Components;
using System;

namespace EvershockGame.Code.Components
{
    public class ManaPickupComponent : PickupComponent, IPickupComponent
    {
        public ManaPickupComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------
        
        public void OnPickup(IEntity collector)
        {
            if (collector != null && IsCollectable)
            {
                if (collector is Player)
                {
                    PlayerAttributesComponent attributes = collector.GetComponent<PlayerAttributesComponent>();
                    attributes.ReplenishMana(15);

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
