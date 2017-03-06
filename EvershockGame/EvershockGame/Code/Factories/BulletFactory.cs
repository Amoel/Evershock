using EvershockGame.Code.Factory;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Factories
{
    public static class BulletFactory
    {
        public static Bullet Create(Vector3 location, Vector2 direction)
        {
            return Create(location, direction, BulletDesc.Default);
        }

        //---------------------------------------------------------------------------

        public static Bullet Create(Vector3 location, Vector2 direction, BulletDesc desc)
        {
            Bullet bullet = EntityFactory.Create<Bullet>("Bullet");
            bullet.Init(desc, location, direction);
            return bullet;
        }
    }
}
