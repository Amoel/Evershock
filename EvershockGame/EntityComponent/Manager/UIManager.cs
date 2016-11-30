using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public class UIManager : BaseManager<UIManager>
    {
        private RenderTarget2D m_Target;

        public Rectangle ScreenBounds { get; private set; }

        public bool IsUIDebugViewActive { get; set; }

        //---------------------------------------------------------------------------

        protected UIManager() { }

        //---------------------------------------------------------------------------

        public void Init(GraphicsDevice device, int width, int height)
        {
            m_Target = new RenderTarget2D(device, width, height);
            ScreenBounds = new Rectangle(0, 0, width, height);
            Console.WriteLine(width + "/" + height);
        }

        //---------------------------------------------------------------------------

        public void Render(GraphicsDevice device, SpriteBatch batch)
        {
            device.SetRenderTarget(m_Target);
            device.Clear(Color.Transparent);
            batch.Begin();
            ComponentManager.Get().DrawUIComponents(batch);
            batch.End();
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(m_Target, m_Target.Bounds, Color.White);
        }
    }
}
