using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent
{
    public interface ICollider
    {
        EColliderType Type { get; set; }
        EColliderMobility Mobility { get; set; }
    }
}
