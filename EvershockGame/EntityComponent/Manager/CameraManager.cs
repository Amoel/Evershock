using EntityComponent.Components;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EntityComponent.Manager
{
    public class CameraManager : BaseManager<CameraManager>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private List<Guid> m_Cameras;

        //---------------------------------------------------------------------------
         
        protected CameraManager() { }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Cameras = new List<Guid>();
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height)
        {
            Width = width;
            Height = height;
        }

        //---------------------------------------------------------------------------

        public void RegisterCamera(CameraComponent camera)
        {
            if (camera != null && !m_Cameras.Contains(camera.GUID))
            {
                m_Cameras.Add(camera.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public void UnregisterCamera(CameraComponent camera)
        {
            if (camera != null && m_Cameras.Contains(camera.GUID))
            {
                m_Cameras.Remove(camera.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public void ResizeCameras(GraphicsDevice device, int width, int height)
        {
            foreach (Guid guid in m_Cameras)
            {
                CameraComponent camera = ComponentManager.Get().Find<CameraComponent>(guid);
                if (camera != null && camera.IsInitialized)
                {
                    camera.ResizeCamera(device, width, height);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Render(GraphicsDevice device, SpriteBatch batch)
        {
            List<RenderTarget2D> targets = new List<RenderTarget2D>();
            foreach (Guid guid in m_Cameras)
            {
                CameraComponent camera = ComponentManager.Get().Find<CameraComponent>(guid);
                if (camera != null && camera.IsInitialized)
                {
                    targets.Add(camera.Render(device, batch));
                }
            }

            device.SetRenderTarget(null);
            device.Clear(Color.CornflowerBlue);
            batch.Begin();
            for (int i = 0; i < targets.Count; i++)
            {
                batch.Draw(targets[i], new Rectangle(i * device.PresentationParameters.BackBufferWidth / 2, 0, device.PresentationParameters.BackBufferWidth / 2, device.PresentationParameters.BackBufferHeight), Color.White);
            }
            batch.End();
        }
    }
}
