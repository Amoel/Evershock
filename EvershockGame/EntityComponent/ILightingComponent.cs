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
        void Draw(SpriteBatch batch, CameraData data);
    }
}
