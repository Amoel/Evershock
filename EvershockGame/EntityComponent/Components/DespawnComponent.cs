using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Components
{
    [Serializable]
    public class DespawnComponent : Component, ITickableComponent
    {
        private float m_Timer = 0.0f;

        //---------------------------------------------------------------------------

        public DespawnComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            //m_Timer += deltaTime;
            //if (m_Timer > 1.0f) Trigger();
        }

        //---------------------------------------------------------------------------

        public void Trigger()
        {
            EntityManager.Get().Unregister(Entity);
        }
    }
}
