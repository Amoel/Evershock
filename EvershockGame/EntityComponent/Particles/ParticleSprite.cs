using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Particles
{
    public class ParticleSprite
    {
        public Texture2D Sprite { get; set; }

        private List<Rectangle> m_Frames;

        //---------------------------------------------------------------------------

        public ParticleSprite(Texture2D sprite, int width = 1, int height = 1)
        {
            Sprite = sprite;

            m_Frames = new List<Rectangle>();
            int pxWidth = sprite.Width / width;
            int pxHeight = sprite.Height / height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    m_Frames.Add(new Rectangle(x * pxWidth, y * pxHeight, pxWidth, pxHeight));
                }
            }
        }

        //---------------------------------------------------------------------------

        public Rectangle GetFrame(int index)
        {
            if (index < 0 || index > m_Frames.Count) return new Rectangle();
            return m_Frames[index];
        }
    }
}
