using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityComponent.Components
{
    public enum ECameraMode
    {
        None,
        FusedLeft,
        FusedRight,
        Split
    };

    //---------------------------------------------------------------------------

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
        private ECameraMode m_CameraMode;
        private CameraComponent m_Other;

        public bool IsInitialized { get { return m_ComponentsTarget != null; } }
        public bool IsLightingEnabled { get; set; }

        public Vector3 Center { get; private set; }

        //---------------------------------------------------------------------------

        public CameraComponent(Guid entity) : base(entity)
        {
            m_Targets = new Dictionary<Guid, CameraTarget>();
            m_CameraMode = ECameraMode.Split;
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
                CalculateCenter(transform.Location);
                
                Vector3 distance = Vector3.Zero;
                
                switch (m_CameraMode)
                {
                    case ECameraMode.Split:
                        distance = Center - transform.Location;
                        transform.MoveBy(distance / 20.0f);
                        break;
                    case ECameraMode.FusedLeft:
                    case ECameraMode.FusedRight:
                        if (m_Other != null)
                        {
                            Vector3 fusedCenter = Vector3.Zero;
                            if (m_CameraMode == ECameraMode.FusedLeft)
                            {
                                fusedCenter = (Center + m_Other.Center) / 2.0f - new Vector3(m_MainTarget.Width / 2, 0, 0);
                            }
                            else
                            {
                                fusedCenter = (Center + m_Other.Center) / 2.0f + new Vector3(m_MainTarget.Width / 2, 0, 0);
                            }
                            distance = fusedCenter - transform.Location;
                            if (distance.Length() < 1.0f)
                            {
                                transform.MoveTo(fusedCenter);
                            }
                            else
                            {
                                transform.MoveBy(distance / 10.0f);
                            }
                        }
                        break;
                }
            }
        }

        //---------------------------------------------------------------------------

        private void CalculateCenter(Vector3 location)
        {
            List<Vector3> locations = new List<Vector3>();
            Center = Vector3.Zero;
            foreach (CameraTarget target in m_Targets.Values)
            {
                TransformComponent targetTransform = ComponentManager.Get().Find<TransformComponent>(target.Target);
                if (targetTransform != null)
                {
                    if (target.Distance <= 0.0f || Vector2.Distance(targetTransform.Location.To2D(), location.To2D()) < target.Distance)
                    {
                        locations.Add(targetTransform.Location);
                        Center += targetTransform.Location;

                    }
                }
            }
            Center /= (float)locations.Count;
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
                        new Rectangle((int)(location.X % m_BackgroundTexture.Width), (int)(location.Y % m_BackgroundTexture.Height), m_ComponentsTarget.Width, m_ComponentsTarget.Height), 
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
                    CalculateCenter(transform.Location);
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
                    CalculateCenter(transform.Location);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Fuse(CameraComponent other, ECameraMode mode)
        {
            m_Other = other;
            m_CameraMode = mode;
        }

        //---------------------------------------------------------------------------

        public void Split()
        {
            m_CameraMode = ECameraMode.Split;
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
