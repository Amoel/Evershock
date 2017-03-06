using EvershockGame.Code.Components;
using EvershockGame.Components;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvershockGame.Code
{
    public interface ICollider
    {
        event CollisionEnterEventHandler Enter;
        event CollisionLeaveEventHandler Leave;

        //---------------------------------------------------------------------------
        
        void SetSensor(bool isSensor);
        void SetRestitution(float restitution);
        void ClearFixtures();
    }
}
