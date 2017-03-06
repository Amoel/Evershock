using EvershockGame.Code;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Init(width, height, Vector2.Zero, BodyType.Static, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height, Vector2 offset)
        {
            Init(width, height, offset, BodyType.Static, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height, BodyType bodyType)
        {
            Init(width, height, Vector2.Zero, bodyType, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height, Vector2 offset, BodyType bodyType, float dampening)
        {
            ClearFixtures();

            Width = width;
            Height = height;

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
