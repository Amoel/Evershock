using EntityComponent.Entities;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public delegate void PropertyChangedEventHandler(object value);

    //---------------------------------------------------------------------------

    public class UIManager : BaseManager<UIManager>
    {
        private RenderTarget2D m_Target;
        private List<Frame> m_Frames;

        public Rectangle ScreenBounds { get; private set; }

        public bool IsUIDebugViewActive { get; set; }

        private Dictionary<Guid, Dictionary<string, PropertyChangedEventHandler>> m_RegisteredProperties;

        //---------------------------------------------------------------------------

        protected UIManager()
        {
            m_RegisteredProperties = new Dictionary<Guid, Dictionary<string, PropertyChangedEventHandler>>();
        }

        //---------------------------------------------------------------------------

        public void Init(GraphicsDevice device, int width, int height)
        {
            m_Target = new RenderTarget2D(device, width, height);
            m_Frames = new List<Frame>();
            ScreenBounds = new Rectangle(0, 0, width, height);
        }

        //---------------------------------------------------------------------------

        public void AddFrame()
        {

        }

        //---------------------------------------------------------------------------

        public void Focus(UIEntity entity)
        {

        }

        //---------------------------------------------------------------------------

        public void Render(GraphicsDevice device, SpriteBatch batch)
        {
            device.SetRenderTarget(m_Target);
            device.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
            ComponentManager.Get().DrawUIComponents(batch);
            batch.End();
        }

        //---------------------------------------------------------------------------

        public void RegisterListener(IComponent component, string property, PropertyChangedEventHandler callback)
        {
            if (!m_RegisteredProperties.ContainsKey(component.GUID))
            {
                m_RegisteredProperties.Add(component.GUID, new Dictionary<string, PropertyChangedEventHandler>());
            }
            if (!m_RegisteredProperties[component.GUID].ContainsKey(property))
            {
                m_RegisteredProperties[component.GUID].Add(property, null);
            }
            m_RegisteredProperties[component.GUID][property] += callback;
            callback?.Invoke(component.GetType().GetProperty(property).GetValue(component));
        }

        //---------------------------------------------------------------------------

        public void UpdateProperty(Guid guid, string name, object value)
        {
            if (m_RegisteredProperties.ContainsKey(guid))
            {
                if (m_RegisteredProperties[guid].ContainsKey(name))
                {
                    m_RegisteredProperties[guid][name]?.Invoke(value);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(m_Target, m_Target.Bounds, Color.White);
        }
    }

    //---------------------------------------------------------------------------

    public class Frame
    {

    }
}
