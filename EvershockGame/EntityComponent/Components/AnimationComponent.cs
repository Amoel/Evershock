using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components
{
    public enum EAnimationState
    {
        Playing,
        Paused,
        Stopped
    }

    //---------------------------------------------------------------------------

    [RequireComponent(typeof(TransformComponent))]
    public class AnimationComponent : Component, ITickableComponent, IDrawableComponent
    {
        private EAnimationState m_State;
        private Dictionary<int, AnimationSetting> m_Settings;
        private int m_ActiveSetting;

        public Texture2D Spritesheet { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Scale { get; set; }

        //---------------------------------------------------------------------------

        public AnimationComponent(Guid entity) : base(entity) { m_Settings = new Dictionary<int, AnimationSetting>(); }

        //---------------------------------------------------------------------------

        public void Init(Texture2D spritesheet)
        {
            Spritesheet = spritesheet;

            Color = Color.White;
            Opacity = 1.0f;
            Scale = new Vector2(1.0f, 1.0f);
            Offset = new Vector2(0, 25);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D spritesheet, Vector2 scale, float opacity = 1.0f)
        {
            Spritesheet = spritesheet;

            Color = Color.White;
            Opacity = opacity;
            Scale = scale;
            Offset = new Vector2(0, 25);
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D spritesheet, Vector2 scale, Vector2 offset, float opacity = 1.0f)
        {
            Spritesheet = spritesheet;

            Color = Color.White;
            Opacity = opacity;
            Scale = scale;
            Offset = offset;
        }

        //---------------------------------------------------------------------------

        public void Init(Texture2D spritesheet, Vector2 scale, Vector2 offset, Color color, float opacity = 1.0f)
        {
            Spritesheet = spritesheet;

            Color = color;
            Opacity = opacity;
            Scale = scale;
            Offset = offset;
        }

        //---------------------------------------------------------------------------

        public void Play()
        {
            m_State = EAnimationState.Playing;
        }

        //---------------------------------------------------------------------------

        public void Play(int tag)
        {
            if (m_ActiveSetting == tag && m_State == EAnimationState.Playing) return;
            if (!ChangeSetting(tag)) return;
            m_State = EAnimationState.Playing;
        }

        //---------------------------------------------------------------------------

        public void Stop()
        {
            AnimationSetting setting = GetActiveSetting();
            if (setting != null)
            {
                setting.Reset();
            }
            m_State = EAnimationState.Stopped;
        }

        //---------------------------------------------------------------------------

        public void Pause()
        {
            m_State = EAnimationState.Paused;
        }

        //---------------------------------------------------------------------------

        public void AddSetting(int tag, AnimationSetting setting)
        {
            if (!m_Settings.ContainsKey(tag))
            {
                m_Settings.Add(tag, setting);
                m_ActiveSetting = tag;
            }
        }

        //---------------------------------------------------------------------------

        public bool ChangeSetting(int tag)
        {
            if (m_Settings.ContainsKey(tag))
            {
                AnimationSetting setting = GetActiveSetting();
                if (setting != null)
                {
                    setting.Reset();
                }
                m_ActiveSetting = tag;
                return true;
            }
            return false;
        }

        //---------------------------------------------------------------------------

        public AnimationSetting GetActiveSetting()
        {
            return (m_Settings.ContainsKey(m_ActiveSetting) ? m_Settings[m_ActiveSetting] : null);
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            if (m_State == EAnimationState.Playing) m_Settings[m_ActiveSetting].Tick(deltaTime);
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data)
        {
            if (Spritesheet != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    Rectangle frame = GetActiveSetting().GetFrame(Spritesheet);
                    if (frame != null)
                    {
                        Vector3 absoluteLocation = transform.AbsoluteLocation;
                        batch.Draw(
                            Spritesheet,
                            absoluteLocation.ToLocal2D(data),
                            frame,
                            Color * Opacity,
                            transform.Rotation,
                            new Vector2(frame.Width / 2 + Offset.X, frame.Height / 2 + Offset.Y),
                            Scale,
                            SpriteEffects.None,
                            Math.Max(0.0001f, absoluteLocation.Z / 1000.0f) + (absoluteLocation.Y + 10000.0f) / 100000.0f);
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }

    //---------------------------------------------------------------------------

    public class AnimationSetting
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }
        public int FPS { get; set; }
        public bool IsReversed { get; set; }

        private int m_Frame;
        private float m_Time;

        //---------------------------------------------------------------------------

        public AnimationSetting(int width, int height, bool isReversed = false)
            : this(width, height, 0, width * height, isReversed) { }

        //---------------------------------------------------------------------------

        public AnimationSetting(int width, int height, int startFrame, int endFrame, bool isReversed = false)
        {
            Width = width;
            Height = height;
            StartFrame = startFrame;
            EndFrame = endFrame;
            FPS = 12;
            IsReversed = isReversed;

            Reset();
        }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            float duration = (1.0f / FPS);
            float maxDuration = duration * (EndFrame - StartFrame);
            m_Time += deltaTime;
            if (m_Time > maxDuration) m_Time -= maxDuration;
            if (IsReversed)
            {
                m_Frame = EndFrame - (int)(m_Time / duration);
            }
            else
            {
                m_Frame = StartFrame + (int)(m_Time / duration);
            }
        }

        //---------------------------------------------------------------------------

        public Rectangle GetFrame(Texture2D texture)
        {
            int width = texture.Width / Math.Max(1, Width);
            int height = texture.Height / Math.Max(1, Height);
            int x = width * (m_Frame % Width);
            int y = height * (m_Frame / Width);
            return new Rectangle(x, y, width, height);
        }

        //---------------------------------------------------------------------------

        public void Reset()
        {
            m_Time = 0.0f;
            m_Frame = IsReversed ? EndFrame : StartFrame;
        }
    }
}
