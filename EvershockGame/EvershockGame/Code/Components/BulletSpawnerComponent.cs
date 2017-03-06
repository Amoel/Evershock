using EvershockGame.Code.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class BulletSpawnerComponent : SpawnerComponent
    {
        public BulletDesc Description { get; set; }

        //---------------------------------------------------------------------------

        public BulletSpawnerComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Spawn()
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                BulletFactory.Create(transform.AbsoluteLocation, transform.Orientation, Description);
            }
        }
    }
}
