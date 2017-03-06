using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvershockGame.Code.Manager
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

#if DEBUG
            ConsoleManager.Get().Tick();
#endif
        }

        //---------------------------------------------------------------------------

        public void Render(GraphicsDevice device, SpriteBatch batch, float deltaTime)
        {
            CameraManager.Get().Render(batch, deltaTime);
            UIManager.Get().Render(device, batch, deltaTime);

            device.SetRenderTarget(null);
            device.Clear(Color.Transparent);
            batch.Begin();
            CameraManager.Get().Draw(device, batch);
            UIManager.Get().Draw(batch);

#if DEBUG
            ConsoleManager.Get().Draw(batch, deltaTime);
#endif

            batch.End();
        }
    }
}
