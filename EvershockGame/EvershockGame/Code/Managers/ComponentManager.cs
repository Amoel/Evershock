using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EvershockGame.Components;
using System.Reflection;
using EvershockGame.Code.Components;

namespace EvershockGame.Code.Manager
{
    [Serializable]
    [SerializeManager(true, "data", "Managers")]
    public class ComponentManager : BaseManager<ComponentManager>
    {
        [JsonIgnore]
        private Dictionary<Guid, SmartContainer<IComponent>> m_Components;
        [JsonIgnore]
        private Dictionary<Guid, SmartContainer<ITickableComponent>> m_TickableComponents;
        [JsonIgnore]
        private Dictionary<Guid, SmartContainer<IDrawableComponent>> m_DrawableComponents;
        [JsonIgnore]
        private Dictionary<Guid, SmartContainer<ILightingComponent>> m_LightingComponents;
        [JsonIgnore]
        private Dictionary<Guid, SmartContainer<IDrawableUIComponent>> m_DrawableUIComponents;

        [JsonIgnore]
        private Queue<IComponent> m_RegisterQueue;
        [JsonIgnore]
        private Queue<Guid> m_UnregisterQueue;

        //---------------------------------------------------------------------------

        protected ComponentManager() { GlobalManager.Get().Register(this); }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Components = new Dictionary<Guid, SmartContainer<IComponent>>();
            m_TickableComponents = new Dictionary<Guid, SmartContainer<ITickableComponent>>();
            m_DrawableComponents = new Dictionary<Guid, SmartContainer<IDrawableComponent>>();
            m_LightingComponents = new Dictionary<Guid, SmartContainer<ILightingComponent>>();
            m_DrawableUIComponents = new Dictionary<Guid, SmartContainer<IDrawableUIComponent>>();

            m_RegisterQueue = new Queue<IComponent>();
            m_UnregisterQueue = new Queue<Guid>();
        }

        //---------------------------------------------------------------------------

        protected override void InitAfterDeserialize()
        {
            if (m_Components == null)
            {
                m_Components = new Dictionary<Guid, SmartContainer<IComponent>>();
            }

            if (m_TickableComponents == null)
            {
                m_TickableComponents = new Dictionary<Guid, SmartContainer<ITickableComponent>>();
            }

            if (m_DrawableComponents == null)
            {
                m_DrawableComponents = new Dictionary<Guid, SmartContainer<IDrawableComponent>>();
            }

            if (m_LightingComponents == null)
            {
                m_LightingComponents = new Dictionary<Guid, SmartContainer<ILightingComponent>>();
            }

            if (m_DrawableUIComponents == null)
            {
                m_DrawableUIComponents = new Dictionary<Guid, SmartContainer<IDrawableUIComponent>>();
            }
        }

        //---------------------------------------------------------------------------

        public void Register(IComponent component)
        {
            if (component != null) m_RegisterQueue.Enqueue(component);
        }

        //---------------------------------------------------------------------------

        private void ExecuteRegister(IComponent component)
        {
            if (component != null)
            {
                if (component is ITickableComponent && !m_TickableComponents.ContainsKey(component.GUID))
                {
                    m_TickableComponents.Add(component.GUID, new SmartContainer<ITickableComponent>(component as ITickableComponent));
                }
                if (component is IDrawableComponent && !m_DrawableComponents.ContainsKey(component.GUID))
                {
                    m_DrawableComponents.Add(component.GUID, new SmartContainer<IDrawableComponent>(component as IDrawableComponent));
                }
                if (component is ILightingComponent && !m_LightingComponents.ContainsKey(component.GUID))
                {
                    m_LightingComponents.Add(component.GUID, new SmartContainer<ILightingComponent>(component as ILightingComponent));
                }
                if (component is IDrawableUIComponent && !m_DrawableUIComponents.ContainsKey(component.GUID))
                {
                    m_DrawableUIComponents.Add(component.GUID, new SmartContainer<IDrawableUIComponent>(component as IDrawableUIComponent));
                }
                if (!m_Components.ContainsKey(component.GUID))
                {
                    m_Components.Add(component.GUID, new SmartContainer<IComponent>(component));
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Unregister(Guid guid)
        {
            m_UnregisterQueue.Enqueue(guid);
        }

        //---------------------------------------------------------------------------

        public void Unregister(IComponent component)
        {
            if (component != null) m_UnregisterQueue.Enqueue(component.GUID);
        }

        //---------------------------------------------------------------------------

        private void ExecuteUnregister(IComponent component)
        {
            if (component != null)
            {
                component.OnCleanup();

                if (component is ITickableComponent && m_TickableComponents.ContainsKey(component.GUID)) m_TickableComponents.Remove(component.GUID);
                if (component is IDrawableComponent && m_DrawableComponents.ContainsKey(component.GUID)) m_DrawableComponents.Remove(component.GUID);
                if (component is ILightingComponent && m_LightingComponents.ContainsKey(component.GUID)) m_LightingComponents.Remove(component.GUID);
                if (component is IDrawableUIComponent && m_DrawableUIComponents.ContainsKey(component.GUID)) m_DrawableUIComponents.Remove(component.GUID);
                if (m_Components.ContainsKey(component.GUID)) m_Components.Remove(component.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public IComponent Find(Guid guid)
        {
            if (m_Components.ContainsKey(guid))
            {
                return m_Components[guid].Data;
            }
            return m_RegisterQueue.FirstOrDefault(component => component.GUID == guid);
        }

        //---------------------------------------------------------------------------

        public T Find<T>(Guid guid) where T : IComponent
        {
            if (m_Components.ContainsKey(guid) && typeof(T).IsAssignableFrom(m_Components[guid].Data.GetType()))
            {
                return (T)m_Components[guid].Data;
            }
            IComponent newComponent = m_RegisterQueue.FirstOrDefault(component => component.GUID == guid);
            return ((newComponent != null && typeof(T).IsAssignableFrom(newComponent.GetType())) ? (T)newComponent : default(T));
        }

        //---------------------------------------------------------------------------

        public void TickComponents(float deltaTime)
        {
            while (m_RegisterQueue.Count > 0)
            {
                ExecuteRegister(m_RegisterQueue.Dequeue());
            };

            while (m_UnregisterQueue.Count > 0)
            {
                Guid guid = m_UnregisterQueue.Dequeue();
                ExecuteUnregister(Find(guid));
            };

            foreach (SmartContainer<ITickableComponent> container in m_TickableComponents.Values)
            {
                if (!((IComponent)container.Data).IsEnabled) continue;
                container.Data.PreTick(deltaTime);
            }

            foreach (SmartContainer<ITickableComponent> container in m_TickableComponents.Values)
            {
                if (!((IComponent)container.Data).IsEnabled) continue;
                container.Data.Tick(deltaTime);
            }

            foreach (SmartContainer<ITickableComponent> container in m_TickableComponents.Values)
            {
                if (!((IComponent)container.Data).IsEnabled) continue;
                container.Data.PostTick(deltaTime);
            }
        }

        //---------------------------------------------------------------------------

        public void DrawComponents(SpriteBatch batch, CameraData data, float deltaTime)
        {
            foreach (SmartContainer<IDrawableComponent> container in m_DrawableComponents.Values)
            {
                if (!((IComponent)container.Data).IsEnabled) continue;
                container.Data.Draw(batch, data, deltaTime);
            }
        }

        //---------------------------------------------------------------------------

        public void DrawLights(SpriteBatch batch, CameraData data, float deltaTime)
        {
            foreach (SmartContainer<ILightingComponent> container in m_LightingComponents.Values)
            {
                if (!((IComponent)container.Data).IsEnabled) continue;
                container.Data.DrawLight(batch, data, deltaTime);
            }
        }

        //---------------------------------------------------------------------------

        public void DrawUIComponents(SpriteBatch batch, float deltaTime)
        {
            foreach (SmartContainer<IDrawableUIComponent> container in m_DrawableUIComponents.Values)
            {
                if (!((IComponent)container.Data).IsEnabled) continue;
                container.Data.Draw(batch,deltaTime);
            }
        }
    }
}
