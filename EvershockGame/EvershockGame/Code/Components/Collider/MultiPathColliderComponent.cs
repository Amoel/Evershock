using EvershockGame.Manager;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Components
{
    public class MultiPathColliderComponent : ColliderComponent
    {
        public List<Edge> Edges { get; private set; }

        //---------------------------------------------------------------------------

        public MultiPathColliderComponent(Guid entity) : base(entity)
        {
            Edges = new List<Edge>();
        }

        //---------------------------------------------------------------------------
        
        public void Init()
        {
            Body = new Body(PhysicsManager.Get().World);
            Body.BodyType = BodyType.Static;
            Body.IgnoreGravity = true;
        }

        //---------------------------------------------------------------------------

        public void Reset()
        {
            Body.FixtureList.Clear();
        }

        //---------------------------------------------------------------------------

        public void AddPath(Vector2 start, Vector2 end)
        {
            Fixture fixture = FixtureFactory.AttachEdge(start / Unit, end / Unit, Body, Entity);
            fixture.CollisionCategories = m_CategoryMapping[CollisionCategory];
            //fixture.CollidesWith = m_CategoryMapping[CollidesWith];
            Edges.Add(new Edge(start, end));
        }

        //---------------------------------------------------------------------------

        public override void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (CollisionManager.Get().IsDebugViewActive)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                Texture2D tex = CollisionManager.Get().PointTexture;
                if (transform != null && tex != null && Body != null)
                {
                    foreach (Edge edge in Edges)
                    {
                        Vector2 location = edge.Start.ToLocal(data) + Vector2.One;
                        float length = Vector2.Distance(edge.Start, edge.End);
                        float angle = (float)Math.Atan2(edge.End.Y - edge.Start.Y, edge.End.X - edge.Start.X);
                        batch.Draw(tex, new Rectangle((int)location.X, (int)location.Y, (int)length, 2), tex.Bounds, GetDebugColor(), angle, Vector2.Zero, SpriteEffects.None, 1.0f);
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
    
    //---------------------------------------------------------------------------

    public struct Edge
    {
        public Vector2 Start { get; private set; }
        public Vector2 End { get; private set; }

        //---------------------------------------------------------------------------

        public Edge(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }
}
