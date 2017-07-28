using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class BulletPhysicsComponent : PhysicsComponent
    {
        public BulletPhysicsComponent(Guid entity) : base(entity)
        {
            Body.IsSensor = true;
            Body.IsBullet = true;
            Body.Friction = 0.0f;
            IsGravityAffected = false;
        }

        //---------------------------------------------------------------------------

        public void Init(BulletDesc desc, Vector3 location, Vector2 direction)
        {
            Init(BodyType.Dynamic, 0, 0);
            ResetLocation();
            ApplyAbsoluteForce(Vector3.Normalize(direction.To3D()) * desc.Velocity);
        }
    }
}
