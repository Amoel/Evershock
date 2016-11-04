using Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Manager
{
    public class PhysicsManager : BaseManager<PhysicsManager>
    {
        public Vector3 Gravity { get; set; }

        //---------------------------------------------------------------------------

        protected PhysicsManager() { }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            Gravity = new Vector3(0.0f, 0.0f, -1.0f);
        }
    }
}
