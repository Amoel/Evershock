using EntityComponent.Components;
using EntityComponent.Factory;
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

        public void RegisterCamera(Camera camera)
        {
            if (camera != null && !m_Cameras.Contains(camera.Properties.GUID))
            {
                m_Cameras.Add(camera.Properties.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public void UnregisterCamera(Camera camera)
        {
            if (camera != null && m_Cameras.Contains(camera.Properties.GUID))
            {
                m_Cameras.Remove(camera.Properties.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public void ResizeCameras(int width, int height)
        {
            foreach (Guid guid in m_Cameras)
            {
                CameraComponent camera = ComponentManager.Get().Find<CameraComponent>(guid);
                if (camera != null && camera.IsInitialized)
                {
                    camera.ResizeCamera(width, height);
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
                    Camera camera1 = EntityManager.Get().Find<Camera>(first.Entity);
                    Camera camera2 = EntityManager.Get().Find<Camera>(second.Entity);
                    if (camera1 != null && camera2 != null)
                    {
                        switch (data.Mode)
                        {
                            case ECameraMode.Split:
                                if (Vector3.Distance(first.GetCenter(ECameraTargetGroup.One), second.GetCenter(ECameraTargetGroup.One)) < data.Distance)
                                {
                                    data.UpdateMode(ECameraMode.Fused);
                                    first.SetCameraMode(ECameraMode.Fused, second);
                                    second.SetCameraMode(ECameraMode.Fused, first);
                                }
                                break;
                            case ECameraMode.Fused:
                                if (Vector3.Distance(first.GetCenter(ECameraTargetGroup.One), second.GetCenter(ECameraTargetGroup.One)) > data.Distance)
                                {
                                    data.UpdateMode(ECameraMode.Split);
                                    first.SetCameraMode(ECameraMode.Split);
                                    second.SetCameraMode(ECameraMode.Split);
                                }
                                else
                                {
                                    Vector3 firstLocation = camera1.Transform.Location;
                                    Vector3 secondLocation = camera2.Transform.Location;
                                    if (Math.Abs(firstLocation.X - secondLocation.X) <= (first.Width + second.Width) / 2 && Math.Abs(firstLocation.Y - secondLocation.Y) <= 0.0f)
                                    {
                                        MergeCameras(camera1, camera2);
                                        data.UpdateMode(ECameraMode.Merged);
                                        first.SetCameraMode(ECameraMode.Merged);
                                        second.SetCameraMode(ECameraMode.Merged);
                                    }
                                }
                                break;
                            case ECameraMode.Merged:
                                if (Vector3.Distance(first.GetCenter(ECameraTargetGroup.One), second.GetCenter(ECameraTargetGroup.One)) > data.Distance)
                                {
                                    SplitCameras(camera1, camera2);
                                    data.UpdateMode(ECameraMode.Split);
                                    first.SetCameraMode(ECameraMode.Split);
                                    second.SetCameraMode(ECameraMode.Split);
                                }
                                break;
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Render(SpriteBatch batch, float deltaTime)
        {
            if (m_Targets == null) m_Targets = new List<RenderTarget2D>();
            else m_Targets.Clear();
            
            foreach (Guid guid in m_Cameras)
            {
                CameraComponent camera = ComponentManager.Get().Find<CameraComponent>(guid);
                if (camera != null && camera.IsInitialized && EntityManager.Get().Find<Camera>(camera.Entity).IsEnabled)
                {
                    m_Targets.Add(camera.Render(batch, deltaTime));
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Draw(GraphicsDevice device, SpriteBatch batch)
        {
            foreach (Guid guid in m_Cameras)
            {
                CameraComponent camera = ComponentManager.Get().Find<CameraComponent>(guid);
                if (camera != null && camera.IsInitialized && EntityManager.Get().Find<Camera>(camera.Entity).IsEnabled)
                {
                    camera.Draw(batch);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void FuseCameras(Camera first, Camera second, float distance)
        {
            m_FusedCameras.Add(new CameraFuseData(first.Properties.GUID, second.Properties.GUID, distance));
        }

        //---------------------------------------------------------------------------

        private void MergeCameras(Camera left, Camera right)
        {
            left.Properties.ResizeCamera(left.Properties.Width + right.Properties.Width, left.Properties.Height);
            left.Properties.Viewport = new Rectangle(0, 0, left.Properties.Viewport.Width + right.Properties.Viewport.Width, left.Properties.Viewport.Height);
            foreach (CameraTarget target in right.Properties.GetTargets())
            {
                left.Properties.AddTarget(new CameraTarget(target.Target, ECameraTargetGroup.Two, target.Distance));
            }
            left.Transform.MoveTo(left.Properties.GetCenter());
            right.Disable();
            left.Properties.SetCameraMode(ECameraMode.Merged, right.Properties);
            right.Properties.SetCameraMode(ECameraMode.Merged, left.Properties);
        }

        //---------------------------------------------------------------------------

        private void SplitCameras(Camera left, Camera right)
        {
            left.Properties.ResizeCamera(left.Properties.Width - right.Properties.Width, left.Properties.Height);

            if (left.Properties.GetCenter(ECameraTargetGroup.One).X < left.Properties.GetCenter(ECameraTargetGroup.Two).X)
            {
                left.Properties.Viewport = new Rectangle(0, 0, Width / 2, Height);
                right.Properties.Viewport = new Rectangle(Width / 2, 0, Width / 2, Height);

                left.Transform.MoveTo(left.Properties.GetCenter() - new Vector3(left.Properties.Width / 2, 0, 0));
                right.Transform.MoveTo(left.Properties.GetCenter() + new Vector3(right.Properties.Width / 2, 0, 0));
            }
            else
            {
                right.Properties.Viewport = new Rectangle(0, 0, Width / 2, Height);
                left.Properties.Viewport = new Rectangle(Width / 2, 0, Width / 2, Height);

                left.Transform.MoveTo(left.Properties.GetCenter() + new Vector3(left.Properties.Width / 2, 0, 0));
                right.Transform.MoveTo(left.Properties.GetCenter() - new Vector3(right.Properties.Width / 2, 0, 0));
            }
            
            foreach (CameraTarget target in right.Properties.GetTargets())
            {
                left.Properties.RemoveTarget(target.Target);
            }
            right.Enable();
            left.Properties.SetCameraMode(ECameraMode.Fused, right.Properties);
            right.Properties.SetCameraMode(ECameraMode.Fused, left.Properties);
        }

        //---------------------------------------------------------------------------

        class CameraFuseData
        {
            public Guid First { get; private set; }
            public Guid Second { get; private set; }
            public float Distance { get; private set; }
            public ECameraMode Mode { get; private set; }

            //---------------------------------------------------------------------------

            public CameraFuseData(Guid first, Guid second, float distance)
            {
                First = first;
                Second = second;
                Distance = distance;
                Mode = ECameraMode.Split;
            }

            //---------------------------------------------------------------------------

            public void UpdateMode(ECameraMode mode)
            {
                Mode = mode;
            }
        }
    }
}
