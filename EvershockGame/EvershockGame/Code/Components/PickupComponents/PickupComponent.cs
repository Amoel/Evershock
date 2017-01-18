using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public abstract class PickupComponent : Component, ITickableComponent
    {
        public bool IsCollectable { get; private set; }

        //---------------------------------------------------------------------------

        public PickupComponent(Guid entity) : base(entity)
        {
            IsCollectable = false;
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            if (!IsCollectable)
            {
                PhysicsComponent physics = GetComponent<PhysicsComponent>();
                if (physics != null)
                {
                    if (physics.HasTouchedFloor)
                    {
                        IsCollectable = true;
                    }
                }
            }
        }
    }
}
