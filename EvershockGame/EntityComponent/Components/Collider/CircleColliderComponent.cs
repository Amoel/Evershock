using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(PhysicsComponent))]
    public class CircleColliderComponent : ColliderComponent
    {
        public float Radius { get; set; }

        //---------------------------------------------------------------------------

        public CircleColliderComponent(Guid entity) : base(entity)
        {
            CollisionManager.Get().Register(Entity);
        }

        //---------------------------------------------------------------------------

        public void Init(int radius)
        {
            Init(radius, Vector2.Zero, BodyType.Static, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(int radius, Vector2 offset)
        {
            Init(radius, offset, BodyType.Static, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(int radius, BodyType bodyType)
        {
            Init(radius, Vector2.Zero, bodyType, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(int radius, Vector2 offset, BodyType bodyType, float dampening)
        {
            Radius = radius;
            Body = BodyFactory.CreateCircle(PhysicsManager.Get().World, Radius / Unit, 0.0f, Entity);
            foreach (Fixture fix in Body.FixtureList)
            {
                fix.UserData = Entity;
            }
            Body.BodyType = bodyType;
            Body.IgnoreGravity = true;
            Body.LinearDamping = dampening;
            //Body.Restitution = 1.0f;

            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                Body.Position = transform.Location.To2D() / Unit;
            }

            Body.OnCollision += OnCollision;
            Body.OnSeparation += OnSeparation;
        }

        //---------------------------------------------------------------------------

        public override void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (CollisionManager.Get().IsDebugViewActive)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                Texture2D tex = CollisionManager.Get().CircleTexture;
                if (transform != null && tex != null && Body != null)
                {
                    Vector2 position = (Body.Position * Unit).ToLocal(data);
                    batch.Draw(tex, new Rectangle((int)(position.X - Radius), (int)(position.Y - Radius), (int)(Radius * 2), (int)(Radius * 2)), tex.Bounds, GetDebugColor(), 0, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup()
        {
            PhysicsManager.Get().World.RemoveBody(Body);
        }
    }
}
