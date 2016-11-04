using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent
{
    public interface ICameraComponent
    {
        void Render(GraphicsDevice device, SpriteBatch batch);
        void ResizeCamera(GraphicsDevice device, int width, int height);
        RenderTarget2D GetTexture();
    }
}
