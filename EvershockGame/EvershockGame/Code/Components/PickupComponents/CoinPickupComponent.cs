using EvershockGame;
using EvershockGame.Components;
using System;

namespace EvershockGame.Code.Components
{
    public class CoinPickupComponent : PickupComponent, IPickupComponent
    {
        public CoinPickupComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        //TODO_lukas: Think about what Coins could be good for

        public void OnPickup(IEntity collector)
        {
            if (collector != null && IsCollectable)
            {
                if (collector is Player)
                {
                    PlayerAttributesComponent attributes = collector.GetComponent<PlayerAttributesComponent>();
                    attributes.AddCoins();

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