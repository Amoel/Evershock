using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using EntityComponent.Manager;
using Microsoft.Xna.Framework;

namespace EntityComponent.Components
{
    public class AreaColliderComponent : ColliderComponent
    {
        public List<Rectangle> Rects { get; private set; }

        //---------------------------------------------------------------------------

        public AreaColliderComponent(Guid entity) : base(entity) { Init(); }

        //---------------------------------------------------------------------------

        public void Init()
        {
            Body = BodyFactory.CreateBody(PhysicsManager.Get().World, Vector2.Zero, 0, Entity);
            foreach (Fixture fix in Body.FixtureList)
            {
                fix.UserData = Entity;
            }
            Body.BodyType = BodyType.Static;
            Body.IgnoreGravity = true;

            Rects = new List<Rectangle>();
        }

        //---------------------------------------------------------------------------

        public void AddRect(int x, int y, int width, int height)
        {
            Fixture fixture = FixtureFactory.AttachRectangle(width * 32.0f / Unit, height * 32.0f / Unit, 0, new Vector2((x + width / 2.0f) * 32.0f / Unit, (y + height / 2.0f) * 32.0f / Unit), Body, Entity);
            fixture.CollisionCategories = m_CategoryMapping[CollisionCategory];
            fixture.CollidesWith = m_CategoryMapping[CollidesWith];
            fixture.IsSensor = true;
            fixture.OnCollision += OnCollision;
            fixture.OnSeparation += OnSeparation;
            Body.FixtureList.Add(fixture);
            Rects.Add(new Rectangle(x * 32, y * 32, width * 32, height * 32));
        }

        //---------------------------------------------------------------------------

        public override void Draw(SpriteBatch batch, CameraData data)
        {
            if (CollisionManager.Get().IsDebugViewActive && Rects != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                Texture2D tex = CollisionManager.Get().PointTexture;
                if (transform != null && tex != null && Body != null)
                {
                    foreach (Rectangle rect in Rects)
                    {
                        Vector2 location = new Vector2(rect.X, rect.Y).ToLocal(data);
                        batch.Draw(tex, new Rectangle((int)location.X - 6, (int)location.Y - 6, rect.Width + 12, 6), tex.Bounds, Color.Purple, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
                        batch.Draw(tex, new Rectangle((int)location.X - 6, (int)location.Y + rect.Height, rect.Width + 12, 6), tex.Bounds, Color.Purple, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
                        batch.Draw(tex, new Rectangle((int)location.X - 6, (int)location.Y, 6, rect.Height), tex.Bounds, Color.Purple, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
                        batch.Draw(tex, new Rectangle((int)location.X + rect.Width, (int)location.Y, 6, rect.Height), tex.Bounds, Color.Purple, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
                    }
                }
            }
        }
    }
}
