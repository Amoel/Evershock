using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Manager;
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

        protected bool m_IsCollected;

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
            if (m_IsCollected)
            {
                LightingComponent light = GetComponent<LightingComponent>();
                if (light != null)
                {
                    light.Scale *= 1.2f;
                    light.Brightness -= 0.05f;
                }
                SpriteComponent sprite = GetComponent<SpriteComponent>();
                if (sprite != null)
                {
                    sprite.Opacity -= 0.05f;
                    if (sprite.Opacity <= 0.0f)
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
}
