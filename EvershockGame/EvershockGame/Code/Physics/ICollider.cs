using EvershockGame.Components;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvershockGame
{
    public interface ICollider
    {
        event CollisionEnterEventHandler Enter;
        event CollisionLeaveEventHandler Leave;

        //---------------------------------------------------------------------------

        Vector2 Step(Vector2 force);
        void ResetLocation(Vector2 location);
        void SetSensor(bool isSensor);
    }
}
