using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame
{
    public struct Sprite
    {
        public bool IsEmpty { get { return Texture == null; } }

        [JsonIgnore]
        public Texture2D Texture { get; private set; }
        public Rectangle Bounds { get; private set; }

        //---------------------------------------------------------------------------

        public Sprite(Texture2D texture) : this()
        {
            Texture = texture;
            Bounds = (Texture != null ? new Rectangle(0, 0, Texture.Width, Texture.Height) : new Rectangle());
        }

        //---------------------------------------------------------------------------

        public Sprite(Texture2D texture, Rectangle bounds) : this()
        {
            Texture = texture;
            Bounds = bounds;
        }

        //---------------------------------------------------------------------------

        public Sprite(Texture2D texture, int width, int height, int index) : this()
        {
            Texture = texture;
            int pxWidth = Texture.Width / width;
            int pxHeight = Texture.Height / height;
            Bounds = new Rectangle((index % width) * pxWidth, (index / width) * pxHeight, pxWidth, pxHeight);
        }

        //---------------------------------------------------------------------------

        public Sprite(Texture2D texture, int width, int height, int x, int y) : this()
        {
            Texture = texture;
            int pxWidth = Texture.Width / width;
            int pxHeight = Texture.Height / height;
            Bounds = new Rectangle(x * pxWidth, y * pxHeight, pxWidth, pxHeight);
        }

        //---------------------------------------------------------------------------

        public static implicit operator Sprite(Texture2D texture)
        {
            return new Sprite(texture);
        }
    }
}
