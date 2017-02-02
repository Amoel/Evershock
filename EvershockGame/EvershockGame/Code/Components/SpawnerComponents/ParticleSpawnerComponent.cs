using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using EntityComponent.Particles;
using EvershockGame.Code.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class ParticleSpawnerComponent : SpawnerComponent, ITickableComponent, IDrawableComponent, ILightingComponent
    {
        public IParticleEmitter Emitter { get; private set; }

        //---------------------------------------------------------------------------

        public ParticleSpawnerComponent(Guid entity) : base(entity)
        {
            Emitter = new PointParticleEmitter(Vector3.Zero)
            {
                SpawnRate = (time) => 60,
                Sprite = new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Particles), 9, 1, 0),
                Light = new Sprite(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight))
            };
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            if (Emitter != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    Emitter.Center = transform.Location;
                }
                Emitter.Update(deltaTime);
            }
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (Emitter != null)
            {
                Emitter.Draw(batch, data, deltaTime);
            }
        }

        //---------------------------------------------------------------------------

        public void DrawLight(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (Emitter != null)
            {
                Emitter.DrawLight(batch, data, deltaTime);
            }
        }
    }
}
