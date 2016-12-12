using EntityComponent.Manager;
using Level;
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
        Split,
        Fused,
        Merged
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
        private Vector3 m_Center;

        private Effect m_LightingEffect;
        private List<Effect> m_PostEffects;
        private RenderTarget2D[] m_EffectTargets;

        private ECameraMode m_CameraMode;
        private CameraComponent m_Other;

        public Rectangle Viewport { get; set; }
        public bool IsInitialized { get { return m_ComponentsTarget != null; } }
        public bool IsLightingEnabled { get; set; }

        public int Width { get { return m_MainTarget != null ? m_MainTarget.Width : 0; } }
        public int Height { get { return m_MainTarget != null ? m_MainTarget.Height : 0; } }

        public GraphicsDevice Device { get; private set; }

        //---------------------------------------------------------------------------

        public CameraComponent(Guid entity) : base(entity)
        {
            m_Targets = new Dictionary<Guid, CameraTarget>();
            m_PostEffects = new List<Effect>();
            m_EffectTargets = new RenderTarget2D[2];
            m_CameraMode = ECameraMode.Split;
            IsLightingEnabled = true;
        }

        //---------------------------------------------------------------------------

        public void Init(GraphicsDevice device, int width, int height, Texture2D backgroundTexture = null, Effect lightingEffect = null)
        {
            Device = device;
            m_ComponentsTarget = new RenderTarget2D(device, width, height);
            m_LightingTarget = new RenderTarget2D(device, width, height);
            m_MainTarget = new RenderTarget2D(device, width, height);

            m_EffectTargets[0] = new RenderTarget2D(device, width, height);
            m_EffectTargets[1] = new RenderTarget2D(device, width, height);

            m_BackgroundTexture = backgroundTexture;
            m_LightingEffect = lightingEffect;

            if (lightingEffect != null) AddEffect(lightingEffect);

            Viewport = new Rectangle(0, 0, width, height);
        }

        //---------------------------------------------------------------------------

        public void ResizeCamera(int width, int height)
        {
            m_ComponentsTarget.Dispose();
            m_ComponentsTarget = new RenderTarget2D(Device, width, height);

            m_LightingTarget.Dispose();
            m_LightingTarget = new RenderTarget2D(Device, width, height);

            m_MainTarget.Dispose();
            m_MainTarget = new RenderTarget2D(Device, width, height);

            m_EffectTargets[0].Dispose();
            m_EffectTargets[0] = new RenderTarget2D(Device, width, height);

            m_EffectTargets[1].Dispose();
            m_EffectTargets[1] = new RenderTarget2D(Device, width, height);
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                m_Center = CalculateCenter(transform.Location);

                Vector3 distance = Vector3.Zero;
                
                switch (m_CameraMode)
                {
                    case ECameraMode.Split:
                    case ECameraMode.Merged:
                        distance = m_Center - transform.Location;
                        transform.MoveBy(distance / 20.0f);
                        break;
                    case ECameraMode.Fused:
                        if (m_Other != null)
                        {
                            Vector3 fusedCenter = Vector3.Zero;
                            if (m_Other.GetCenter().X < GetCenter().X)
                            {
                                fusedCenter = (m_Center + m_Other.GetCenter()) / 2.0f + new Vector3(m_MainTarget.Width / 2, 0, 0);
                            }
                            else
                            {
                                fusedCenter = (m_Center + m_Other.GetCenter()) / 2.0f - new Vector3(m_MainTarget.Width / 2, 0, 0);
                            }
                            distance = fusedCenter - transform.Location;

                            if (distance.Length() > 0.0f)
                            {
                                Vector3 speed = (distance / 10.0f).Length() < 2.0f ? Vector3.Normalize(distance) * 2 : distance / 10.0f;
                                if (speed.Length() > distance.Length())
                                {
                                    transform.MoveBy(distance);
                                }
                                else
                                {
                                    transform.MoveBy(speed);
                                }
                            }
                        }
                        break;
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void AddEffect(Effect effect)
        {
            m_PostEffects.Add(effect);
        }

        //---------------------------------------------------------------------------

        public Vector3 GetCenter(ECameraTargetGroup group = ECameraTargetGroup.None)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                return CalculateCenter(transform.Location, group);
            }
            return Vector3.Zero;
        }

        //---------------------------------------------------------------------------

        private Vector3 CalculateCenter(Vector3 location, ECameraTargetGroup group = ECameraTargetGroup.None)
        {
            Vector3 center = Vector3.Zero;
            List<Vector3> locations = new List<Vector3>();
            foreach (CameraTarget target in m_Targets.Values)
            {
                if (group != ECameraTargetGroup.None && group != target.Group) continue;
                TransformComponent targetTransform = ComponentManager.Get().Find<TransformComponent>(target.Target);
                if (targetTransform != null)
                {
                    if (target.Distance <= 0.0f || Vector2.Distance(targetTransform.Location.To2D(), location.To2D()) < target.Distance)
                    {
                        locations.Add(targetTransform.Location);
                        center += targetTransform.Location;

                    }
                }
            }
            center /= (float)locations.Count;
            return center;
        }

        //---------------------------------------------------------------------------

        public RenderTarget2D Render(SpriteBatch batch)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                CameraData data = new CameraData(transform.Location.To2D(), m_ComponentsTarget.Width, m_ComponentsTarget.Height);

                Device.SetRenderTarget(m_ComponentsTarget);
                Device.Clear(Color.Black);

                if (m_ComponentsTarget != null)
                {
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
                    DrawStage(batch, data);
                    ComponentManager.Get().DrawComponents(batch, data);
                    StageManager.Get().Draw(batch, data);
                    batch.End();

                    if (m_LightingTarget != null && m_LightingEffect != null)
                    {
                        Device.SetRenderTarget(m_LightingTarget);
                        Device.Clear(Color.Black);

                        batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                        ComponentManager.Get().DrawLights(batch, new CameraData(transform.Location.To2D(), m_LightingTarget.Width, m_LightingTarget.Height));
                        batch.End();

                        if (m_MainTarget != null)
                        {
                            m_LightingEffect.Parameters["lightMask"].SetValue(m_LightingTarget);
                            for (int i = 0; i < m_PostEffects.Count; ++i)
                            {
                                Device.SetRenderTarget(m_EffectTargets[i % 2]);
                                Device.Clear(Color.Transparent);
                                batch.Begin(SpriteSortMode.Deferred, null, null, null, null, m_PostEffects[i], null);
                                batch.Draw(i == 0 ? m_ComponentsTarget : m_EffectTargets[(i + 1) % 2], m_EffectTargets[(i + 1) % 2].Bounds, Color.White);
                                batch.End();
                            }

                            Device.SetRenderTarget(m_MainTarget);
                            batch.Begin();
                            batch.Draw((m_PostEffects.Count > 0) ? m_EffectTargets[(m_PostEffects.Count - 1) % 2] : m_ComponentsTarget, Vector2.Zero, Color.White);
                            batch.End();

                            return m_MainTarget;
                        }

                        return m_LightingTarget;
                    }
                }
            }
            return m_ComponentsTarget;
        }

        //---------------------------------------------------------------------------

        private void DrawStage(SpriteBatch batch, CameraData data)
        {
            Texture2D tileset = AssetManager.Get().Find<Texture2D>("BasicTileset");
            if (tileset != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    Vector2 location = new Vector2(Width / 2 - data.Center.X, Height / 2 - data.Center.Y);

                    for (int x = 0; x < Width / 32 + 2; x++)
                    {
                        for (int y = 0; y < Height / 32 + 2; y++)
                        {
                            int xPos = (x + (int)(data.Center.X - Width / 2) / 32);
                            int yPos = (y + (int)(data.Center.Y - Height / 2) / 32);

                            Rectangle layer1 = StageManager.Get().GetTextureBounds(xPos, yPos, ELayerMode.First);
                            if (layer1.Width > 0 && layer1.Height > 0) batch.Draw(tileset, new Rectangle((int)location.X + xPos * 32, (int)location.Y + yPos * 32, 32, 32), layer1, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.00001f);

                            Rectangle layer2 = StageManager.Get().GetTextureBounds(xPos, yPos, ELayerMode.Second);
                            if (layer2.Width > 0 && layer2.Height > 0) batch.Draw(tileset, new Rectangle((int)location.X + xPos * 32, (int)location.Y + yPos * 32, 32, 32), layer2, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.00002f);

                            Rectangle layer3 = StageManager.Get().GetTextureBounds(xPos, yPos, ELayerMode.Third);
                            if (layer3.Width > 0 && layer3.Height > 0) batch.Draw(tileset, new Rectangle((int)location.X + xPos * 32, (int)location.Y + yPos * 32, 32, 32), layer3, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(m_MainTarget, Viewport, Color.White);
        }

        //---------------------------------------------------------------------------

        public void AddTarget(IEntity entity, float distance = 0.0f)
        {
            if (entity != null)
            {
                TransformComponent transform = entity.GetComponent<TransformComponent>();
                if (transform != null && !m_Targets.ContainsKey(transform.GUID))
                {
                    m_Targets.Add(transform.GUID, new CameraTarget(transform.GUID, ECameraTargetGroup.One, distance));
                    CalculateCenter(transform.Location);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void AddTarget(CameraTarget target)
        {
            if (target != null)
            {
                TransformComponent transform = ComponentManager.Get().Find<TransformComponent>(target.Target);
                if (transform != null && !m_Targets.ContainsKey(transform.GUID))
                {
                    m_Targets.Add(target.Target, target);
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

        public void RemoveTarget(Guid target)
        {
            if (target != null)
            {
                TransformComponent transform = ComponentManager.Get().Find<TransformComponent>(target);
                if (transform != null && m_Targets.ContainsKey(transform.GUID))
                {
                    m_Targets.Remove(target);
                    CalculateCenter(transform.Location);
                }
            }
        }

        //---------------------------------------------------------------------------

        public List<CameraTarget> GetTargets()
        {
            return m_Targets.Values.ToList();
        }

        //---------------------------------------------------------------------------

        public void SetCameraMode(ECameraMode mode, CameraComponent other = null)
        {
            m_Other = other;
            m_CameraMode = mode;
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

        //---------------------------------------------------------------------------

        public Rectangle Bounds
        {
            get { return new Rectangle((int)(Center.X - Width / 2), (int)(Center.Y - Height / 2), (int)Width, (int)Height); }
        }
    }
}
