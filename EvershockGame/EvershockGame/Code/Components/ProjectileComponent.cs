﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class ProjectileComponent : Component
    {
        public ProjectileComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
