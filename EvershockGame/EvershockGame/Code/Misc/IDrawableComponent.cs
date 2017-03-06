using EvershockGame.Code.Components;
using EvershockGame.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public interface IDrawableComponent
    {
        void Draw(SpriteBatch batch, CameraData data, float deltaTime);
    }
}
