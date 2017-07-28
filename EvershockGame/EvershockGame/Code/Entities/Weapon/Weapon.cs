using EvershockGame.Code.Components;
using EvershockGame.Code.Items;
using EvershockGame.Components;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public abstract class Weapon : Entity
    {
        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public SpriteComponent Sprite { get { return GetComponent<SpriteComponent>(); } }
        //public EngineComponent Engine { get { return GetComponent<EngineComponent>(); } }

        public bool IsAttacking { get; protected set; }

        //---------------------------------------------------------------------------

        public Weapon(string name, Guid parent) : base(name, parent)
        {
            TransformComponent transform = AddComponent<TransformComponent>();
            transform.Init(new Vector3(40, 1, 20), Vector2.One, Vector2.UnitY, 0.0f);

            SpriteComponent sprite = AddComponent<SpriteComponent>();
            sprite.Scale = new Vector2(1.5f, 1.5f);
            sprite.Offset = new Vector2(-20, 20);

            //EngineComponent engine = AddComponent<EngineComponent>();

            //EnginePath pathRight = engine.AddPath(0, false);
            //pathRight.AddKeyframe(new Keyframe(new Vector3(15, 1, 80), 0.4f, 0.0f));
            //pathRight.AddKeyframe(new Keyframe(new Vector3(15, 1, 80), 2.0f, 0.12f));
            //pathRight.AddKeyframe(new Keyframe(new Vector3(15, 1, 80), 0.4f, 0.24f));

            //EnginePath pathLeft = engine.AddPath(1, false);
            //pathLeft.AddKeyframe(new Keyframe(new Vector3(-15, 1, 80), -0.4f, 0.0f));
            //pathLeft.AddKeyframe(new Keyframe(new Vector3(-15, 1, 80), -2.0f, 0.12f));
            //pathLeft.AddKeyframe(new Keyframe(new Vector3(-15, 1, 80), -0.4f, 0.24f));

            Init();
        }

        //---------------------------------------------------------------------------

        public void Init()
        {
            IEntity parent = GetParent();
            if (parent != null)
            {
                TransformComponent parentTransform = parent.GetComponent<TransformComponent>();
                if (parentTransform != null)
                {
                    parentTransform.OrientationChanged += OnParentOrientationChanged;
                }

                PhysicsComponent parentPhysics = parent.GetComponent<PhysicsComponent>();
                PhysicsComponent physics = GetComponent<PhysicsComponent>();
                if (parentPhysics != null)
                {
                    parentPhysics.AddJoint(physics);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void TryUpdate(ItemDesc desc)
        {
            Sprite.Sprite = desc.Sprite;
        }

        //---------------------------------------------------------------------------

        public abstract void TryAttack();

        //---------------------------------------------------------------------------

        protected abstract void OnAttackEnded();

        //---------------------------------------------------------------------------

        private void OnParentOrientationChanged(Vector2 oldOrientation, Vector2 orientation)
        {
            Transform.OrientateTo(orientation);
            if (orientation.X > 0)
            {
                //if (Engine.State == EAnimationState.Stopped)
                //{
                //    Engine.SetCurrentPath(0);
                //}
                Sprite.Orientation = SpriteEffects.FlipHorizontally;
                Sprite.Offset = new Vector2(-20, 20);
            }
            else
            {
                //if (Engine.State == EAnimationState.Stopped)
                //{
                //    Engine.SetCurrentPath(1);
                //}
                Sprite.Orientation = SpriteEffects.None;
                Sprite.Offset = new Vector2(20, 20);
            }
        }
    }
}
