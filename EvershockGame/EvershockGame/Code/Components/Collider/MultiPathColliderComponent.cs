using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class MultiPathColliderComponent : ColliderComponent
    {
        public MultiPathColliderComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Reset()
        {
            ClearFixtures();
        }

        //---------------------------------------------------------------------------

        public void AddPath(Vector2 start, Vector2 end)
        {
            PhysicsComponent physics = GetComponent<PhysicsComponent>();
            if (physics != null)
            {
                Fixture fixture = FixtureFactory.AttachEdge(start / Unit, end / Unit, physics.Body, Entity);
                fixture.CollisionCategories = m_CategoryMapping[CollisionCategory];
                Fixtures.Add(fixture);
                //fixture.CollidesWith = m_CategoryMapping[CollidesWith];
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
                        EdgeShape shape = fixture.Shape as EdgeShape;
                        if (shape != null)
                        {
                            Vector2 start = shape.Vertex1 * Unit;
                            Vector2 end = shape.Vertex2 * Unit;
                            DrawLine(batch, tex, start, end, data);
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
