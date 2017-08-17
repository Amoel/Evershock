using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvershockGame.Code.Components;
using EvershockGame.Components;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvershockGame.Code
{
    public class Dagger : MeleeWeapon, IDrawableComponent
    {


        public Dagger(string name, Guid parent) : base (name, parent)
        {
            AnimationComponent animation = AddComponent<AnimationComponent>();
            animation.Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.CoinAnimation));
        }

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {

        }
    }
}