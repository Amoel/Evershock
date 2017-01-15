using EntityComponent.Components;
using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class SimpleTestEnemy : Enemy
    {
        public SimpleTestEnemy(string name) : base(name)
        {
            MovementAnimationComponent animation = AddComponent<MovementAnimationComponent>();
            animation.Init(AssetManager.Get().Find<Texture2D>("WalkingAnimation"), new Vector2(0.5f, 0.5f));
            animation.AddSetting((int)Tag.MoveLeft, new AnimationSetting(8, 2, 8, 15, true));
            animation.AddSetting((int)Tag.MoveRight, new AnimationSetting(8, 2, 0, 7));

            CircleColliderComponent collider = AddComponent<CircleColliderComponent>();
            collider.Init(22, BodyType.Dynamic);
        }
    }
}
