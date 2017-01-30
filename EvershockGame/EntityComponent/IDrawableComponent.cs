﻿using EntityComponent.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    public interface IDrawableComponent
    {
        void Draw(SpriteBatch batch, CameraData data, float deltaTime);
    }
}
