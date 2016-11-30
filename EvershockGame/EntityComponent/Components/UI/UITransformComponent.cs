using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components.UI
{
    public enum EVerticalAlignment
    {
        Top,
        Center,
        Bottom,
        Stretch
    }

    //---------------------------------------------------------------------------

    public enum EHorizontalAlignment
    {
        Left,
        Center,
        Right,
        Stretch
    }

    //---------------------------------------------------------------------------

    [Serializable]
    public class UITransformComponent : Component, IDrawableUIComponent
    {
        public Point Size { get; set; }

        public EVerticalAlignment VerticalAlignment { get; set; }
        public EHorizontalAlignment HorizontalAlignment { get; set; }

        public Rectangle Margin { get; set; }
        public Rectangle Padding { get; set; }

        //---------------------------------------------------------------------------

        public UITransformComponent(Guid entity) : base(entity)
        {
            Size = new Point(100, 100);
            VerticalAlignment = EVerticalAlignment.Center;
            HorizontalAlignment = EHorizontalAlignment.Center;
            Margin = new Rectangle(10, 10, 10, 10);
        }

        //---------------------------------------------------------------------------

        public Rectangle Bounds()
        {
            Rectangle bounds = new Rectangle();
            UITransformComponent transform = GetComponent<UITransformComponent>();
            if (transform != null)
            {
                Rectangle parentBounds;
                UITransformComponent parentTransform = GetComponentInParent<UITransformComponent>();
                if (parentTransform != null)
                {
                    parentBounds = parentTransform.Bounds();
                }
                else
                {
                    parentBounds = UIManager.Get().ScreenBounds;
                }
                parentBounds = new Rectangle(
                    parentBounds.X + transform.Margin.X,
                    parentBounds.Y + transform.Margin.Y,
                    parentBounds.Width - (transform.Margin.X + transform.Margin.Width),
                    parentBounds.Height - (transform.Margin.Y + transform.Margin.Height));

                Point vertical = new Point();
                switch (transform.VerticalAlignment)
                {
                    case EVerticalAlignment.Top:
                        vertical = new Point(parentBounds.Top, parentBounds.Top + transform.Size.Y);
                        break;
                    case EVerticalAlignment.Center:
                        vertical = new Point(parentBounds.Center.Y - transform.Size.Y / 2, parentBounds.Center.Y + transform.Size.Y / 2);
                        break;
                    case EVerticalAlignment.Bottom:
                        vertical = new Point(parentBounds.Bottom - transform.Size.Y, parentBounds.Bottom);
                        break;
                    case EVerticalAlignment.Stretch:
                        vertical = new Point(parentBounds.Top, parentBounds.Bottom);
                        break;
                }

                Point horizontal = new Point();
                switch (transform.HorizontalAlignment)
                {
                    case EHorizontalAlignment.Left:
                        horizontal = new Point(parentBounds.Left, parentBounds.Left + transform.Size.X);
                        break;
                    case EHorizontalAlignment.Center:
                        horizontal = new Point(parentBounds.Center.X - transform.Size.X / 2, parentBounds.Center.X + transform.Size.X / 2);
                        break;
                    case EHorizontalAlignment.Right:
                        horizontal = new Point(parentBounds.Right - transform.Size.X, parentBounds.Right);
                        break;
                    case EHorizontalAlignment.Stretch:
                        horizontal = new Point(parentBounds.Left, parentBounds.Right);
                        break;
                }

                bounds = new Rectangle(horizontal.X, vertical.X, horizontal.Y - horizontal.X, vertical.Y - vertical.X);
            }
            return bounds;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            if (UIManager.Get().IsUIDebugViewActive)
            {
                Rectangle bounds = Bounds();
                batch.Draw(CollisionManager.Get().PointTexture,
                    new Rectangle(bounds.X, bounds.Y, bounds.Width, 1),
                    CollisionManager.Get().PointTexture.Bounds,
                    Color.Cyan,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1.0f);
                batch.Draw(CollisionManager.Get().PointTexture,
                    new Rectangle(bounds.X, bounds.Y, 1, bounds.Height),
                    CollisionManager.Get().PointTexture.Bounds,
                    Color.Cyan,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1.0f);
                batch.Draw(CollisionManager.Get().PointTexture,
                    new Rectangle(bounds.X + bounds.Width, bounds.Y, 1, bounds.Height),
                    CollisionManager.Get().PointTexture.Bounds,
                    Color.Cyan,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1.0f);
                batch.Draw(CollisionManager.Get().PointTexture,
                    new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, 1),
                    CollisionManager.Get().PointTexture.Bounds,
                    Color.Cyan,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1.0f);
            }
        }
    }
}
