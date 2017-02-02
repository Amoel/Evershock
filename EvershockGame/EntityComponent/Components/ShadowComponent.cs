using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components
{
    [RequireComponent(typeof(TransformComponent))]
    public class ShadowComponent : Component, IDrawableComponent
    {
        public Sprite Shadow { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Offset { get; set; }

        //---------------------------------------------------------------------------

        public ShadowComponent(Guid entity) : base(entity) { Scale = Vector2.One; }

        //---------------------------------------------------------------------------

        public void Init(Sprite shadow)
        {
            Init(shadow, Vector2.One, Vector2.Zero);
        }

        //---------------------------------------------------------------------------

        public void Init(Sprite shadow, Vector2 scale)
        {
            Init(shadow, scale, Vector2.Zero);
        }

        //---------------------------------------------------------------------------

        public void Init(Sprite shadow, Vector2 scale, Vector2 offset)
        {
            Shadow = shadow;
            Scale = scale;
            Offset = offset;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                batch.Draw(
                    Shadow.Texture,
                    transform.Location.ToLocal2DShadow(data),
                    Shadow.Bounds,
                    Color.Black * (0.5f - MathHelper.Clamp(transform.Location.Z / 400.0f, 0.0f, 0.5f)),
                    transform.Rotation,
                    new Vector2(Shadow.Bounds.Width / 2 + Offset.X, Shadow.Bounds.Height / 2 - Offset.Y),
                    new Vector2((1.0f + transform.Location.Z / 120.0f) * Scale.X, (1.0f + transform.Location.Z / 120.0f) * Scale.Y),
                    SpriteEffects.None,
                    0.0001f);
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
