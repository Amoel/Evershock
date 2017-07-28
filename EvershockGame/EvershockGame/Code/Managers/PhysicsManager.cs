using VelcroPhysics.Dynamics;
using Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvershockGame.Code.Manager
{
    public class PhysicsManager : BaseManager<PhysicsManager>
    {
        public Vector3 Gravity { get; set; }
        public World World { get; private set; }

        //---------------------------------------------------------------------------

        protected PhysicsManager() { }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            Gravity = new Vector3(0.0f, 0.0f, -5.0f);
            World = new World(Vector2.Zero);
        }

        //---------------------------------------------------------------------------

        public void Step(float deltaTime)
        {
            if (World != null)
            {
                World.Step(deltaTime);
            }
        }
    }
}
