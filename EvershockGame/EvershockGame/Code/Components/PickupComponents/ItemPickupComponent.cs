using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Items;
using EvershockGame.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class ItemPickupComponent : PickupComponent, IPickupComponent
    {
        public EItemType Type { get; set; }

        //---------------------------------------------------------------------------

        public ItemPickupComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void OnPickup(IEntity collector)
        {
            if (collector != null && IsCollectable)
            {
                if (collector is Player)
                {
                    InventoryComponent inventory = collector.GetComponent<InventoryComponent>();
                    if (inventory != null && inventory.TryAdd(Type, 1) == 0)
                    {
                        m_IsCollected = true;
                        //DespawnComponent despawn = GetComponent<DespawnComponent>();
                        //if (despawn != null)
                        //{
                        //    despawn.Trigger();
                        //}
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
