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
    public class GameManager : BaseManager<GameManager>
    {
        protected GameManager() { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            ComponentManager.Get().TickComponents(deltaTime);
            PhysicsManager.Get().Step(deltaTime);
            CameraManager.Get().Tick();
        }

        //---------------------------------------------------------------------------

        public void Render(GraphicsDevice device, SpriteBatch batch)
        {
            CameraManager.Get().Render(batch);
            UIManager.Get().Render(device, batch);

            device.SetRenderTarget(null);
            device.Clear(Color.Transparent);
            batch.Begin();
            CameraManager.Get().Draw(device, batch);
            UIManager.Get().Draw(batch);
            batch.End();
        }
    }
}
