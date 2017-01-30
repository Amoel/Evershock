using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Factories
{
    public enum EParticles
    {
        Smoke,
        Fire,
        Splash,
        Spark
    }

    //---------------------------------------------------------------------------

    public class ParticleFactory
    {
        public static Particle Create(EParticles particleType, Vector3 location, Vector3 force)
        {
            Particle particle = EntityFactory.Create<Particle>(string.Format("{0}Particle", particleType.ToString()));
            particle.AddComponent<TransformComponent>().Init(location);

            PhysicsComponent physics = particle.AddComponent<PhysicsComponent>();
            physics.Init(0.94f, 4.0f, 0.3f);
            physics.ApplyForce(force, true);

            SpriteComponent sprite = particle.AddComponent<SpriteComponent>();
            LightingComponent light = particle.AddComponent<LightingComponent>();
            DespawnComponent despawn = particle.AddComponent<DespawnComponent>();
            
            switch (particleType)
            {
                case EParticles.Spark:
                    float lifeTime = SeedManager.Get().NextRandF(2.0f, 5.0f);

                    sprite.Init(CollisionManager.Get().PointTexture);
                    sprite.Scale = new Vector2(SeedManager.Get().NextRandF(5, 15), SeedManager.Get().NextRandF(5, 15));
                    sprite.AddColorFunction(x => { return Color.Lerp(new Color(0.99f, 1.0f, 0.78f), new Color(1.0f, 0.0f, 0.0f), x); });
                    sprite.AddOpacityFunction(x => { return 1.0f - x / lifeTime; });

                    light.Init(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight), Vector2.Zero, new Vector2(0.4f, 0.4f));
                    light.AddColorFunction(x => { return Color.Lerp(new Color(0.99f, 1.0f, 0.78f), new Color(1.0f, 0.0f, 0.0f), x); });
                    light.AddBrightnessFunction(x => { return 1.0f - x / lifeTime; });

                    despawn.AddTimeTrigger(x => x > lifeTime);
                    break;
            }

            return particle;
        }
    }
}
