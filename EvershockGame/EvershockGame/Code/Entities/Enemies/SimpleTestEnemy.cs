﻿using EvershockGame;
using EvershockGame.Code.Components;
using EvershockGame.Components;
using EvershockGame.Manager;
using VelcroPhysics.Dynamics;
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
        public SimpleTestEnemy(string name, Guid parent) : base(name, parent)
        {
            Animation.Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.WalkingAnimation3), Vector2.One);
            Animation.AddSetting((int)Tag.MoveLeft, new AnimationSetting(8, 2, 8, 15, true, true));
            Animation.AddSetting((int)Tag.MoveRight, new AnimationSetting(8, 2, 0, 7, true));
            Animation.Stop();

            AddComponent<ShadowComponent>().Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.RedOrb), new Vector2(6.0f, 2.0f), new Vector2(0.0f, 10.0f));

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
