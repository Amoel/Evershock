using EvershockGame.Code;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EvershockGame.Code.Components
{
    [Serializable]
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

            PhysicsComponent physics = GetComponentInAncestor<PhysicsComponent>();
            if (physics != null)
            {
                Fixture fixture = FixtureFactory.AttachCircle(radius / Unit, 0, physics.Body, offset / Unit, Entity);
                fixture.Friction = 0.0f;
                fixture.OnCollision += OnCollision;
                fixture.OnSeparation += OnSeparation;
                Fixtures.Add(fixture);
            }
        }

        //---------------------------------------------------------------------------

        public override void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (CollisionManager.Get().IsDebugViewActive)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                Texture2D tex = CollisionManager.Get().CircleTexture;
                if (transform != null && tex != null && Fixtures.Count > 0)
                {
                    //Vector2 position = (Body.Position * Unit).ToLocal(data);
                    //batch.Draw(tex, new Rectangle((int)(position.X - Radius), (int)(position.Y - Radius), (int)(Radius * 2), (int)(Radius * 2)), tex.Bounds, GetDebugColor(), 0, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }
        }
    }
}
