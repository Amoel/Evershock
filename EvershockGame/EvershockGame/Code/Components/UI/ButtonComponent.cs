using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Components.UI
{
    public enum EButtonState
    {
        Idle,
        Hover,
        Pressed
    }

    //---------------------------------------------------------------------------

    [Serializable]
    [RequireComponent(typeof(UITransformComponent))]
    public class ButtonComponent : Component, IDrawableComponent
    {
        public EButtonState State { get; private set; }

        public string Text { get; set; }
        public Texture2D Icon { get; set; }

        //---------------------------------------------------------------------------

        public ButtonComponent(Guid entity) : base(entity)
        {
            Init();
        }

        //---------------------------------------------------------------------------

        public void Init()
        {
            Icon = CollisionManager.Get().RectTexture;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (Icon != null)
            {
                UITransformComponent transform = GetComponent<UITransformComponent>();
                if (transform != null)
                {
                    batch.Draw(Icon,
                            transform.Bounds(),
                            Icon.Bounds,
                            Color.White,
                            0,
                            Vector2.Zero,
                            SpriteEffects.None,
                            1.0f);
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
