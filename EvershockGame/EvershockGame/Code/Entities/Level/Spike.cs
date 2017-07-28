using EvershockGame;
using EvershockGame.Code.Components;
using EvershockGame.Components;
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
    public class Spike : Entity
    {
        public bool IsActive { get; private set; }

        //---------------------------------------------------------------------------

        public Spike(string name, Guid parent) : base(name, parent)
        {
            IsActive = false;

            AddComponent<TransformComponent>();
            AddComponent<PhysicsComponent>();

            RectColliderComponent collider = AddComponent<RectColliderComponent>();
            collider.Init(64, 64, BodyType.Static);
            collider.SetSensor(true);

            AnimationComponent animation = AddComponent<AnimationComponent>();
            animation.Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.Spikes1), new Vector2(3, 3));
            animation.AddSetting(0, new AnimationSetting(3, 1, false)
            {
                FPS = 10
            });
            animation.AddSetting(0, new AnimationSetting(3, 1, true)
            {
                FPS = 10
            });

            Activate();
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

        //---------------------------------------------------------------------------

        public void Activate()
        {
            IsActive = true;
            AnimationComponent animation = GetComponent<AnimationComponent>();
            if (animation != null)
            {
                animation.Play(1);
            }
        }

        //---------------------------------------------------------------------------

        public void Deactivate()
        {
            IsActive = false;
            AnimationComponent animation = GetComponent<AnimationComponent>();
            if (animation != null)
            {
                animation.Play(0);
            }
        }
    }
}
