using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components.UI
{
    [Serializable]
    [RequireComponent(typeof(UITransformComponent))]
    public class TextComponent : Component, IDrawableUIComponent
    {
        public SpriteFont Font { get; set; }
        public string Text { get; set; }

        //---------------------------------------------------------------------------

        public TextComponent(Guid entity) : base(entity) { Text = Name; }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            if (Font != null)
            {
                UITransformComponent transform = GetComponent<UITransformComponent>();
                if (transform != null)
                {
                    batch.DrawString(Font, Text, new Vector2(transform.Bounds().X, transform.Bounds().Y), Color.White);
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
