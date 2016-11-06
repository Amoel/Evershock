﻿using EntityComponent.Components;
using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent
{
    public interface ICollider
    {
        event CollisionEnterEventHandler Enter;
        event CollisionLeaveEventHandler Leave;

        //---------------------------------------------------------------------------

        Vector2 Step(Vector2 force);
    }
}
