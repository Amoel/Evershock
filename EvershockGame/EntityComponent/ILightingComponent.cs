using EntityComponent.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent
{
    public interface ILightingComponent
    {
        void DrawArea(SpriteBatch batch, CameraData data);
        void DrawLight(SpriteBatch batch, CameraData data);
    }
}
