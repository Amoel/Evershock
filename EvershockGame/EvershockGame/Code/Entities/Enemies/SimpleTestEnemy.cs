using EntityComponent;
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
            Animation.Init(AssetManager.Get().Find<Texture2D>("WalkingAnimation"), Vector2.One);
            Animation.AddSetting((int)Tag.MoveLeft, new AnimationSetting(8, 2, 8, 15, true));
            Animation.AddSetting((int)Tag.MoveRight, new AnimationSetting(8, 2, 0, 7));
            Animation.Stop();

            CircleColliderComponent collider = AddComponent<CircleColliderComponent>();
            collider.Init(22, BodyType.Dynamic);
        }

        //---------------------------------------------------------------------------

        public void Init(Vector2 location)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                transform.MoveTo(location.To3D());

                PhysicsComponent physics = GetComponent<PhysicsComponent>();
                if (physics != null)
                {
                    physics.ResetLocation();
                }
            }
        }
    }
}
