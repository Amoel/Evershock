using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components
{
    public class MultiPathColliderComponent : ColliderComponent
    {
        public MultiPathColliderComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void AddPath(Vector2 start, Vector2 end)
        {

        }
    }
}
