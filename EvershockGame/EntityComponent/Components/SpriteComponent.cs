﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(TransformComponent))]
    public class SpriteComponent : Component, IDrawableComponent
    {
        public static Texture2D DefaultTexture { get; set; }

        public Texture2D Texture { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }

        //---------------------------------------------------------------------------

        public SpriteComponent(Guid entity) : base(entity)
        {
            Init(null, Vector2.Zero, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture)
        {
            Init(texture, Vector2.Zero, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Color color)
        {
            Init(texture, Vector2.Zero, color, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset)
        {
            Init(texture, offset, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset, Color color)
        {
            Init(texture, offset, color, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset, Color color, float opacity)
        {
            Texture = texture;
            Offset = offset;
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

        public void Draw(SpriteBatch batch, CameraData data)
        {
            if (GetTex() != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    if (transform.Location.Z > 0.0f)
                    {
                        batch.Draw(
                            GetTex(),
                            transform.Location.ToLocal2DShadow(data),
                            Texture.Bounds,
                            Color.Black * (0.3f - MathHelper.Clamp(transform.Location.Z / 400.0f, 0.0f, 0.3f)),
                            transform.Rotation,
                            new Vector2(Texture.Width / 2 + Offset.X, Texture.Height / 2 + Offset.Y),
                            1.0f,
                            SpriteEffects.None,
                            0.0f);
                    }
                    batch.Draw(
                        GetTex(), 
                        transform.Location.ToLocal2D(data),
                        GetTex().Bounds, 
                        Color * Opacity, 
                        transform.Rotation, 
                        new Vector2(GetTex().Width / 2 + Offset.X, GetTex().Height / 2 + Offset.Y), 
                        1.0f,
                        SpriteEffects.None, 
                        Math.Max(0.0001f, transform.Location.Z / 1000.0f) + (transform.Location.Y + 10000.0f) / 100000.0f);
                }
            }
        }
    }
}