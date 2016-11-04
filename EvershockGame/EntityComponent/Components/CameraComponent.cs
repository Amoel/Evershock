using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(TransformComponent))]
    public class CameraComponent : Component, ITickableComponent
    {
        private RenderTarget2D m_ComponentsTarget;
        private RenderTarget2D m_LightingTarget;
        private RenderTarget2D m_MainTarget;

        private Texture2D m_BackgroundTexture;

        private Dictionary<Guid, CameraTarget> m_Targets;

        private Effect m_LightingEffect;

        public bool IsInitialized { get { return m_ComponentsTarget != null; } }
        public bool IsLightingEnabled { get; set; }

        //---------------------------------------------------------------------------

        public CameraComponent(Guid entity) : base(entity)
        {
            m_Targets = new Dictionary<Guid, CameraTarget>();
            CameraManager.Get().RegisterCamera(this);

            IsLightingEnabled = true;
        }

        //---------------------------------------------------------------------------

        public void Init(GraphicsDevice device, int width, int height, Texture2D backgroundTexture = null, Effect lightingEffect = null)
        {
            m_ComponentsTarget = new RenderTarget2D(device, width, height);
            m_LightingTarget = new RenderTarget2D(device, width, height);
            m_MainTarget = new RenderTarget2D(device, width, height);

            m_BackgroundTexture = backgroundTexture;
            m_LightingEffect = lightingEffect;
        }

        //---------------------------------------------------------------------------

        public void ResizeCamera(GraphicsDevice device, int width, int height)
        {
            m_ComponentsTarget.Dispose();
            m_ComponentsTarget = new RenderTarget2D(device, width, height);

            m_LightingTarget.Dispose();
            m_LightingTarget = new RenderTarget2D(device, width, height);

            m_MainTarget.Dispose();
            m_MainTarget = new RenderTarget2D(device, width, height);
        }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                List<Vector3> locations = new List<Vector3>();
                Vector3 center = Vector3.Zero;

                foreach (CameraTarget target in m_Targets.Values)
                {
                    TransformComponent targetTransform = ComponentManager.Get().Find<TransformComponent>(target.Target);
                    if (targetTransform != null)
                    {
                        if (target.Distance <= 0.0f || Vector2.Distance(targetTransform.Location.To2D(), transform.Location.To2D()) < target.Distance)
                        {
                            locations.Add(targetTransform.Location);
                            center += targetTransform.Location;
                            
                        }
                    }
                }
                center /= (float)locations.Count;

                Vector3 distance = center - transform.Location;

                transform.MoveBy(distance / 20.0f);
            }
        }

        //---------------------------------------------------------------------------

        public RenderTarget2D Render(GraphicsDevice device, SpriteBatch batch)
        {
            device.SetRenderTarget(m_ComponentsTarget);
            device.Clear(Color.Black);

            if (m_ComponentsTarget != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (m_BackgroundTexture != null)
                {
                    Vector2 location = transform.Location.To2D();
                    batch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
                    batch.Draw(
                        m_BackgroundTexture, 
                        Vector2.Zero, 
                        new Rectangle((int)location.X % m_BackgroundTexture.Width, (int)location.Y % m_BackgroundTexture.Height, m_ComponentsTarget.Width, m_ComponentsTarget.Height), 
                        Color.White, 
                        0, 
                        Vector2.Zero, 
                        1f, 
                        SpriteEffects.None, 
                        0);
                    batch.End();
                }
                batch.Begin(SpriteSortMode.FrontToBack);
                ComponentManager.Get().DrawComponents(batch, new CameraData(transform.Location.To2D(), m_ComponentsTarget.Width, m_ComponentsTarget.Height));
                batch.End();

                if (m_LightingTarget != null && m_LightingEffect != null)
                {
                    device.SetRenderTarget(m_LightingTarget);
                    device.Clear(Color.Black);

                    batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                    ComponentManager.Get().DrawLights(batch, new CameraData(transform.Location.To2D(), m_LightingTarget.Width, m_LightingTarget.Height));
                    batch.End();

                    if (m_MainTarget != null)
                    {
                        device.SetRenderTarget(m_MainTarget);
                        device.Clear(Color.Black);

                        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                        m_LightingEffect.Parameters["lightMask"].SetValue(m_LightingTarget);
                        m_LightingEffect.CurrentTechnique.Passes[0].Apply();
                        batch.Draw(m_ComponentsTarget, new Vector2(0, 0), m_MainTarget.Bounds, Color.White);
                        batch.End();

                        return m_MainTarget;
                    }

                    return m_LightingTarget;
                }
            }
            return m_ComponentsTarget;
        }

        //---------------------------------------------------------------------------

        public void AddTarget(IEntity entity, ECameraPriority priority = ECameraPriority.VeryHigh, float distance = 0.0f)
        {
            if (entity != null)
            {
                TransformComponent transform = entity.GetComponent<TransformComponent>();
                if (transform != null && !m_Targets.ContainsKey(transform.GUID))
                {
                    m_Targets.Add(transform.GUID, new CameraTarget(transform.GUID, priority, distance));
                }
            }
        }

        //---------------------------------------------------------------------------

        public void RemoveTarget(IEntity entity)
        {
            if (entity != null)
            {
                TransformComponent transform = entity.GetComponent<TransformComponent>();
                if (transform != null && m_Targets.ContainsKey(transform.GUID))
                {
                    m_Targets.Remove(transform.GUID);
                }
            }
        }
    }

    //---------------------------------------------------------------------------

    public class CameraData
    {
        public Vector2 Center { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        //---------------------------------------------------------------------------

        public CameraData(Vector2 center, float width, float height)
        {
            Center = center;
            Width = width;
            Height = height;
        }
    }
}
