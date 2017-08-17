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
    public class RectColliderComponent : ColliderComponent
    {
        public float Width { get; set; }
        public float Height { get; set; }

        //---------------------------------------------------------------------------

        public RectColliderComponent(Guid entity) : base(entity)
        {
            CollisionManager.Get().Register(Entity);
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height)
        {
            Init(width, height, Vector2.Zero, 0.0f, ECollisionCategory.All);
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height, Vector2 offset)
        {
            Init(width, height, offset, 0.0f, ECollisionCategory.All);
        }
        
        //---------------------------------------------------------------------------

        public void Init(int width, int height, ECollisionCategory collisionCategory)
        {
            Init(width, height, Vector2.Zero, 0.0f, collisionCategory);
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height, Vector2 offset, float dampening, ECollisionCategory collisionCategory)
        {
            ClearFixtures();

            Width = width;
            Height = height;
            Offset = offset;

            PhysicsComponent physics = GetComponentInAncestor<PhysicsComponent>();
            if (physics != null)
            {
                Fixture fixture = FixtureFactory.AttachRectangle(width / Unit, height / Unit, 0, offset / Unit, physics.Body, Entity);
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
                Texture2D tex = CollisionManager.Get().PointTexture;
                if (tex != null)
                {
                    foreach (Fixture fixture in Fixtures)
                    {
                        PolygonShape shape = fixture.Shape as PolygonShape;
                        if (shape != null)
                        {
                            for (int i = 0; i < shape.Vertices.Count; i++)
                            {
                                Vector2 start = (fixture.Body.Position + shape.Vertices[i]) * Unit;
                                Vector2 end = (fixture.Body.Position + shape.Vertices[(i + 1) % shape.Vertices.Count]) * Unit;
                                DrawLine(batch, tex, start, end, data);
                            }
                        }
                    }
                }
            }
        }
    }
}
