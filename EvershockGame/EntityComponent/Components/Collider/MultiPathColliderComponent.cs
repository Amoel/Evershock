using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public override void Draw(SpriteBatch spritebatch, CameraData cameradata)
        {
            //i did something
        }

        public void AddPath(Vector2 start, Vector2 end)
        {
            
        }
    }
}
