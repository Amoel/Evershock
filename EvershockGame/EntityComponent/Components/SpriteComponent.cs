using Microsoft.Xna.Framework;
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
        public Vector2 Offset { get; set; }

        private Func<float, Color> m_ColorFunction;
        public Color Color
        {
            get { return m_ColorFunction(m_Time); }
            set { m_ColorFunction = (time) => { return value; }; }
        }

        private Func<float, float> m_OpacityFunction;
        public float Opacity
        {
            get { return m_OpacityFunction(m_Time); }
            set { m_OpacityFunction = (time) => { return value; }; }
        }

        private Func<float, Vector2> m_ScaleFunction;
        public Vector2 Scale
        {
            get { return m_ScaleFunction(m_Time); }
            set { m_ScaleFunction = (time) => { return value; }; }
        }

        private float m_Time = 0;

        //---------------------------------------------------------------------------

        public SpriteComponent(Guid entity) : base(entity)
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

        public void Init(Texture2D texture, Vector2 offset, Vector2 scale)
        {
            Init(texture, offset, scale, Color.White, 1.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D texture, Vector2 offset, Vector2 scale, Color color)
        {
            Init(texture, offset, scale, color, 1.0f);
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

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            m_Time += deltaTime;

            Texture2D tex = GetTex();
            if (tex != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null && Vector2.Distance(data.Center, transform.Location.To2D()) <= Math.Sqrt(Math.Pow(data.Width, 2) + Math.Pow(data.Height, 2)) / 2)
                {
                    Color color = m_ColorFunction(m_Time);
                    float opacity = MathHelper.Clamp(m_OpacityFunction(m_Time), 0.0f, 1.0f);
                    Vector2 scale = Vector2.Clamp(m_ScaleFunction(m_Time), Vector2.Zero, new Vector2(10, 10));
                    
                    Vector3 absoluteLocation = transform.AbsoluteLocation;
                    batch.Draw(
                        tex,
                        absoluteLocation.ToLocal2D(data),
                        tex.Bounds, 
                        color * opacity, 
                        transform.Rotation, 
                        new Vector2(tex.Width / 2 + Offset.X, tex.Height / 2 + Offset.Y), 
                        scale,
                        SpriteEffects.None, 
                        Math.Max(0.0001f, absoluteLocation.Z / 1000.0f) + (absoluteLocation.Y + 10000.0f) / 100000.0f);
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

        public void AddOpacityFunction(Func<float, float> opacityFunction)
        {
            if (opacityFunction != null)
            {
                m_OpacityFunction = opacityFunction;
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
