using EvershockGame.Code;
using EvershockGame.Code.Components;
using EvershockGame.Code.Manager;
using EvershockGame.Components;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Particles
{
    public abstract class ParticleEmitter : IParticleEmitter
    {
        public Vector3 Center { get; set; }

        public EEmitterType Type { get; private set; }
        public float Time { get; private set; }
        private float m_SpawnTime;

        public Func<float, int> SpawnRate { get; set; }

        public ParticleDesc Description { get; set; }

        public Sprite Sprite { get; set; }
        public Sprite Light { get; set; }

        private List<Particle> m_Particles;

        protected Random m_Rand;

        //---------------------------------------------------------------------------

        public ParticleEmitter(EEmitterType type, ParticleDesc desc)
        {
            Type = type;
            m_Particles = new List<Particle>();
            m_Rand = new Random();

            Description = (desc != null ? desc : ParticleDesc.Default);
        }

        //---------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            if (IsValidDistance())
            {
                Time += deltaTime;
                m_SpawnTime += deltaTime;

                int currentSpawnRate = SpawnRate(Time);
                if (currentSpawnRate > 0)
                {
                    int count = (int)(m_SpawnTime * currentSpawnRate);

                    for (int i = 0; i < count; i++)
                    {
                        SpawnParticle(NextLocation(), NextVelocity());
                    }

                    float delta = 1.0f / currentSpawnRate;
                    m_SpawnTime %= delta;
                }
                m_Particles.RemoveAll(particle => particle.Update(deltaTime, Description));
            }
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (Description != null && IsValidDistance(data))
            {
                foreach (Particle particle in m_Particles)
                {
                    float relativeLifeTime = particle.RelativeLifeTime;

                    Vector2 location = particle.Location.ToLocal2D(data);
                    Vector2 size = Description.ParticleSize(relativeLifeTime) * new Vector2(Sprite.Bounds.Width, Sprite.Bounds.Height);

                    if (Description.HasShadow)
                    {
                        Vector2 shadowLocation = particle.Location.ToLocal2DShadow(data);
                        Vector2 shadowSize = size;
                        batch.Draw(
                        Sprite.Texture,
                        new Rectangle((int)(shadowLocation.X - shadowSize.X / 2), (int)(shadowLocation.Y - shadowSize.Y / 2), (int)shadowSize.X, (int)shadowSize.Y),
                        Sprite.Bounds,
                        Color.Black * (0.5f - MathHelper.Clamp(particle.Location.Z / 400.0f, 0.0f, 0.5f)) * Description.ParticleOpacity(relativeLifeTime),
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.0001f);
                    }

                    batch.Draw(
                        Sprite.Texture,
                        new Rectangle((int)(location.X - size.X / 2), (int)(location.Y - size.Y / 2), (int)size.X, (int)size.Y),
                        Sprite.Bounds,
                        Description.ParticleColor(relativeLifeTime) * Description.ParticleOpacity(relativeLifeTime),
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        (particle.Location.Y + 10000.0f) / 100000.0f);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void DrawLight(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (Description != null && IsValidDistance(data))
            {
                foreach (Particle particle in m_Particles)
                {
                    float relativeLifeTime = particle.RelativeLifeTime;

                    Vector2 location = particle.Location.ToLocal2D(data);
                    Vector2 size = Description.LightSize(relativeLifeTime) * new Vector2(Light.Bounds.Width, Light.Bounds.Height);

                    batch.Draw(
                        Light.Texture,
                        new Rectangle((int)(location.X - size.X / 2), (int)(location.Y - size.Y / 2), (int)size.X, (int)size.Y),
                        Light.Bounds,
                        Description.LightColor(relativeLifeTime) * Description.LightOpacity(relativeLifeTime),
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        1.0f);
                }
            }
        }

        //---------------------------------------------------------------------------

        protected void SpawnParticle(Vector3 location, Vector3 velocity)
        {
            //float lifeRandom = ((float)m_Rand.NextDouble() - 0.5f) * m_ParticleLifeTimeRandomness;
            m_Particles.Add(new Particle(location, velocity, Description.LifeTime));
        }

        //---------------------------------------------------------------------------

        protected abstract Vector3 NextLocation();

        //---------------------------------------------------------------------------

        protected abstract Vector3 NextVelocity();

        //---------------------------------------------------------------------------

        public void SetDynamicSpawnRate(Func<float, int> spawnRate)
        {
            if (spawnRate != null)
            {
                SpawnRate = spawnRate;
            }
        }

        //---------------------------------------------------------------------------

        private bool IsValidDistance()
        {
            foreach (Camera cam in EntityManager.Get().Find<Camera>())
            {
                TransformComponent transform = cam.GetComponent<TransformComponent>();
                CameraComponent camera = cam.GetComponent<CameraComponent>();
                if (transform != null && camera != null)
                {
                    if (Vector2.Distance(transform.Location.To2D(), Center.To2D()) <= Math.Sqrt(Math.Pow(camera.Width, 2) + Math.Pow(camera.Height, 2)) / 2) return true;
                }
            }
            return false;
        }
        
        //---------------------------------------------------------------------------

        private bool IsValidDistance(CameraData data)
        {
            return Vector2.Distance(data.Center, Center.To2D()) <= Math.Sqrt(Math.Pow(data.Width, 2) + Math.Pow(data.Height, 2)) / 2;
        }
    }
}
