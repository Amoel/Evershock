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
        private float m_Time = 0.0f;
        private bool m_IsTriggered = false;

        private List<Func<bool>> m_CustomTrigger;
        private List<Func<float, bool>> m_TimeTrigger;

        //---------------------------------------------------------------------------

        public DespawnComponent(Guid entity) : base(entity)
        {
            m_TimeTrigger = new List<Func<float, bool>>();
            m_CustomTrigger = new List<Func<bool>>();
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            m_Time += deltaTime;

            if (!m_IsTriggered)
            {
                if (m_TimeTrigger != null)
                {
                    foreach (Func<float, bool> trigger in m_TimeTrigger)
                    {
                        if (trigger(m_Time)) Trigger();
                    }
                }
                if (m_CustomTrigger != null)
                {
                    foreach (Func<bool> trigger in m_CustomTrigger)
                    {
                        if (trigger()) Trigger();
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void AddTimeTrigger(Func<float, bool> trigger)
        {
            m_TimeTrigger.Add(trigger);
        }

        //---------------------------------------------------------------------------

        public void AddCustomTrigger(Func<bool> trigger)
        {
            m_CustomTrigger.Add(trigger);
        }

        //---------------------------------------------------------------------------

        public void Trigger()
        {
            m_IsTriggered = true;
            EntityManager.Get().Unregister(Entity);
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
