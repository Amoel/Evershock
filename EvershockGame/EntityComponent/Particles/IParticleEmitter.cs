using EntityComponent.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Particles
{
    public enum EEmitterType
    {
        Point,
        Circle,
        Directional,
        Vortex
    }

    //---------------------------------------------------------------------------

    public interface IParticleEmitter
    {
        EEmitterType Type { get; }

        Func<float, int> SpawnRate { get; set; }
        ParticleDesc Description { get; set; }

        //---------------------------------------------------------------------------

        void Update(float deltaTime);

        //---------------------------------------------------------------------------

        void Draw(SpriteBatch batch, CameraData data, float deltaTime);
        void DrawLight(SpriteBatch batch, CameraData data, float deltaTime);

        //---------------------------------------------------------------------------

        void SetDynamicSpawnRate(Func<float, int> spawnRate);
    }
}
