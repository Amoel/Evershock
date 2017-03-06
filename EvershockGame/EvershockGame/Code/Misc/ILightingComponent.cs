using EvershockGame.Code.Components;
using EvershockGame.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvershockGame.Code
{
    public interface ILightingComponent
    {
        void DrawLight(SpriteBatch batch, CameraData data, float deltaTime);
    }
}
