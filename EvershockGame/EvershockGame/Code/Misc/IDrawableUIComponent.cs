using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame
{
    public interface IDrawableUIComponent
    {
        void Draw(SpriteBatch batch, float deltaTime);
    }
}
