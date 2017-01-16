using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Components
{
    [RequireComponent(typeof(TransformComponent))]
    public class LightingComponent : Component, ILightingComponent
    {
        public static Texture2D DefaultTexture { get; set; }

        public Texture2D Texture { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Scale { get; set; }

        //---------------------------------------------------------------------------

        public LightingComponent(Guid entity) : base(entity)
        {
            Init(null, Vector2.Zero, Vector2.One, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture)
        {
            Init(texture, Vector2.Zero, Vector2.One, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Color color)
        {
            Init(texture, Vector2.Zero, Vector2.One, color, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset)
        {
            Init(texture, offset, Vector2.One, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset, Vector2 scale)
        {
            Init(texture, offset, scale, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset, Color color)
        {
            Init(texture, offset, Vector2.One, color, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset, Vector2 scale, Color color, float opacity)
        {
            Texture = texture;
            Offset = offset;
            Scale = scale;
            Color = color;
            Opacity = opacity;
        }

        //---------------------------------------------------------------------------

        private Texture2D GetTex()
        {
            if (Texture != null) return Texture;
            return DefaultTexture;
        }

        //---------------------------------------------------------------------------

        public void DrawArea(SpriteBatch batch, CameraData data)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                LightingManager.Get().DrawArea(batch, transform.Location.To2D(), data);
            }
        }

        //---------------------------------------------------------------------------

        public void DrawLight(SpriteBatch batch, CameraData data)
        {
            if (GetTex() != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    batch.Draw(
                        GetTex(),
                        transform.Location.ToLocal2D(data),
                        GetTex().Bounds,
                        Color * Opacity,
                        transform.Rotation,
                        new Vector2(GetTex().Width / 2 + Offset.X, GetTex().Height / 2 + Offset.Y),
                        Scale.X,
                        SpriteEffects.None,
                        0);
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
