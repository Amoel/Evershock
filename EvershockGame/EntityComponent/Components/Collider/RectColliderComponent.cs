using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(PhysicsComponent))]
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
            Width = width;
            Height = height;
            Body = BodyFactory.CreateRectangle(PhysicsManager.Get().World, width / Unit, height / Unit, 0.0f, Entity);
            foreach (Fixture fix in Body.FixtureList)
            {
                fix.UserData = Entity;
            }
            Body.BodyType = bodyType;
            Body.IgnoreGravity = true;
            Body.LinearDamping = dampening;

            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                Body.Position = (transform.Location.To2D() + Offset) / Unit;
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
                Texture2D tex = CollisionManager.Get().RectTexture;
                if (transform != null && tex != null && Body != null)
                {
                    Vector2 position = (Body.Position * Unit).ToLocal(data);
                    batch.Draw(tex, new Rectangle((int)((int)position.X - Width / 2), (int)((int)position.Y - Height / 2), (int)Width, (int)Height), tex.Bounds, GetDebugColor(), 0, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
