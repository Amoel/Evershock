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
        public Vector2 Offset { get; set; }
        
        private Func<float, Color> m_ColorFunction;
        public Color Color
        {
            get { return m_ColorFunction(m_Time); }
            set { m_ColorFunction = (time) => { return value; }; }
        }

        private Func<float, Vector2> m_ScaleFunction;
        public Vector2 Scale
        {
            get { return m_ScaleFunction(m_Time); }
            set { m_ScaleFunction = (time) => { return value; }; }
        }

        private Func<float, float> m_BrightnessFunction;
        public float Brightness
        {
            get { return m_BrightnessFunction(m_Time); }
            set { m_BrightnessFunction = (time) => { return value; }; }
        }

        private float m_Time;

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

        public void Init(Texture2D texture, Vector2 offset, Vector2 scale, Color color, float brightness)
        {
            Texture = texture;
            Offset = offset;
            Scale = scale;
            Color = color;
            Brightness = brightness;
        }

        //---------------------------------------------------------------------------

        private Texture2D GetTex()
        {
            if (Texture != null) return Texture;
            return DefaultTexture;
        }

        //---------------------------------------------------------------------------

        public void DrawLight(SpriteBatch batch, CameraData data, float deltaTime)
        {
            m_Time += deltaTime;

            Texture2D tex = GetTex();
            if (tex != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    Color color = m_ColorFunction(m_Time);
                    float brightness = MathHelper.Clamp(m_BrightnessFunction(m_Time), 0.0f, 1.0f);
                    Vector2 scale = Vector2.Clamp(m_ScaleFunction(m_Time), Vector2.Zero, new Vector2(10, 10));

                    batch.Draw(
                        tex,
                        transform.Location.ToLocal2D(data),
                        tex.Bounds,
                        color * brightness,
                        transform.Rotation,
                        new Vector2(tex.Width / 2 + Offset.X, tex.Height / 2 + Offset.Y),
                        scale,
                        SpriteEffects.None,
                        0);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void AddColorFunction(Func<float, Color> colorFunction)
        {
            if (colorFunction != null)
            {
                m_ColorFunction = colorFunction;
            }
        }

        //---------------------------------------------------------------------------

        public void AddScaleFunction(Func<float, Vector2> scaleFunction)
        {
            if (scaleFunction != null)
            {
                m_ScaleFunction = scaleFunction;
            }
        }

        //---------------------------------------------------------------------------

        public void AddBrightnessFunction(Func<float, float> brightnessFunction)
        {
            if (brightnessFunction != null)
            {
                m_BrightnessFunction = brightnessFunction;
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
