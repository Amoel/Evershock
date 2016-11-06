using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace EntityComponent.Components
{
    public class WallColliderComponent : ColliderComponent
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        //---------------------------------------------------------------------------

        public WallColliderComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Init(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            Body = BodyFactory.CreateEdge(PhysicsManager.Get().World, Start, End, Entity);
            foreach (Fixture fix in Body.FixtureList)
            {
                fix.UserData = Entity;
            }
            Body.BodyType = BodyType.Static;
            Body.IgnoreGravity = true;

            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                Body.Position = transform.Location.To2D() + Offset;
            }

            Body.OnCollision += OnCollision;
            Body.OnSeparation += OnSeparation;
        }

        //---------------------------------------------------------------------------

        public override void Draw(SpriteBatch batch, CameraData data)
        {
            if (CollisionManager.Get().IsDebugViewActive)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                Texture2D tex = CollisionManager.Get().PointTexture;
                if (transform != null && tex != null && Body != null)
                {
                    Vector2 position = Body.Position.ToLocal(data);
                    float length = Vector2.Distance(Start, End);
                    float angle = (float)Math.Atan2(End.Y - Start.Y, End.X - Start.X);
                    batch.Draw(tex, new Rectangle((int)(position.X), (int)(position.Y), (int)length, 2), tex.Bounds, Color.White, angle, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }
        }
    }
}
