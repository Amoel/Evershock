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
    public enum EImageAlignment
    {
        Center,
        Stretch,
        UniformStretch
    }

    //---------------------------------------------------------------------------

    [Serializable]
    [RequireComponent(typeof(UITransformComponent))]
    public class ImageComponent : Component, IDrawableUIComponent
    {
        public EImageAlignment Alignment { get; set; }
        public Texture2D Image { get; set; }

        //---------------------------------------------------------------------------

        public ImageComponent(Guid entity) : base(entity)
        {
            Init(EImageAlignment.UniformStretch);
        }

        //---------------------------------------------------------------------------

        public void Init(EImageAlignment alignment)
        {
            Alignment = alignment;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            if (Image != null)
            {
                UITransformComponent transform = GetComponent<UITransformComponent>();
                if (transform != null)
                {
                    Rectangle bounds = transform.Bounds();

                    switch (Alignment)
                    {
                        case EImageAlignment.Center:
                            break;
                        case EImageAlignment.UniformStretch:
                            double xFactor = (double)bounds.Width / Image.Width;
                            double yFactor = (double)bounds.Height / Image.Height;

                            Point size = new Point(
                                (xFactor < yFactor ? bounds.Width : (int)(Image.Width * yFactor)), 
                                (xFactor < yFactor ? (int)(Image.Height * xFactor) : bounds.Height));
                            bounds = new Rectangle(
                                bounds.X + (bounds.Width - size.X) / 2,
                                bounds.Y + (bounds.Height - size.Y) / 2, 
                                size.X, 
                                size.Y);
                            break;
                    }

                    batch.Draw(Image,
                            bounds,
                            Image.Bounds,
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
