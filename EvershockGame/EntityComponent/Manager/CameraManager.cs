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
        private List<CameraFuseData> m_FusedCameras;
        private List<RenderTarget2D> m_Targets;

        //---------------------------------------------------------------------------
         
        protected CameraManager() { }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Cameras = new List<Guid>();
            m_FusedCameras = new List<CameraFuseData>();
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

        public void Tick()
        {
            foreach (CameraFuseData data in m_FusedCameras)
            {
                CameraComponent first = ComponentManager.Get().Find<CameraComponent>(data.First);
                CameraComponent second = ComponentManager.Get().Find<CameraComponent>(data.Second);

                if (first != null && second != null)
                {
                    if (Vector3.Distance(first.Center, second.Center) < data.Distance)
                    {
                        TransformComponent firstTransform = first.GetComponent<TransformComponent>();
                        TransformComponent secondTransform = second.GetComponent<TransformComponent>();

                        if (firstTransform != null && secondTransform != null)
                        {
                            bool firstIsLeft = (firstTransform.AbsoluteLocation.X < secondTransform.AbsoluteLocation.X);
                            first.Fuse(second, firstIsLeft ? ECameraMode.FusedLeft : ECameraMode.FusedRight);
                            second.Fuse(first, firstIsLeft ? ECameraMode.FusedRight : ECameraMode.FusedLeft);
                        }
                    }
                    else
                    {
                        first.Split();
                        second.Split();
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Render(GraphicsDevice device, SpriteBatch batch)
        {
            if (m_Targets == null) m_Targets = new List<RenderTarget2D>();
            else m_Targets.Clear();

            //List<RenderTarget2D> targets = new List<RenderTarget2D>();
            foreach (Guid guid in m_Cameras)
            {
                CameraComponent camera = ComponentManager.Get().Find<CameraComponent>(guid);
                if (camera != null && camera.IsInitialized)
                {
                    m_Targets.Add(camera.Render(device, batch));
                }
            }

            //device.SetRenderTarget(null);
            //device.Clear(Color.CornflowerBlue);
            //batch.Begin();
            //for (int i = 0; i < targets.Count; i++)
            //{
            //    batch.Draw(targets[i], new Rectangle(i * device.PresentationParameters.BackBufferWidth / targets.Count, 0, device.PresentationParameters.BackBufferWidth / targets.Count, device.PresentationParameters.BackBufferHeight), Color.White);
            //}
            //batch.End();
        }

        //---------------------------------------------------------------------------

        public void Draw(GraphicsDevice device, SpriteBatch batch)
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                batch.Draw(m_Targets[i], new Rectangle(i * device.PresentationParameters.BackBufferWidth / m_Targets.Count, 0, device.PresentationParameters.BackBufferWidth / m_Targets.Count, device.PresentationParameters.BackBufferHeight), Color.White);
            }
        }

        //---------------------------------------------------------------------------

        public void FuseCameras(CameraComponent first, CameraComponent second, float distance)
        {
            m_FusedCameras.Add(new CameraFuseData(first.GUID, second.GUID, distance));
        }

        //---------------------------------------------------------------------------

        public void SplitCameras(CameraComponent first, CameraComponent second)
        {
            m_FusedCameras.RemoveAll(x => x.First == first.GUID && x.Second == second.GUID);
        }

        //---------------------------------------------------------------------------

        struct CameraFuseData
        {
            public Guid First { get; private set; }
            public Guid Second { get; private set; }
            public float Distance { get; private set; }

            //---------------------------------------------------------------------------

            public CameraFuseData(Guid first, Guid second, float distance)
            {
                First = first;
                Second = second;
                Distance = distance;
            }
        }
    }
}
