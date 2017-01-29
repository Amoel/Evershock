using EntityComponent.Components;
using EntityComponent.Factory;
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
        Splash
    }

    //---------------------------------------------------------------------------

    public class ParticleFactory
    {
        public static Particle Create(EParticles particleType, Vector3 location, Vector3 force)
        {
            Particle particle = EntityFactory.Create<Particle>(string.Format("{0}Particle", particleType.ToString()));
            particle.AddComponent<ParticlePhysicsComponent>();

            LightingComponent light = particle.AddComponent<LightingComponent>();
            
            switch (particleType)
            {
                case EParticles.Splash:
                    light.Init(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight), Vector2.Zero, new Vector2(0.4f, 0.4f), Color.Red, 1.0f);
                    break;
            }


            return particle;
        }
    }
}
