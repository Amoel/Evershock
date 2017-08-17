using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvershockGame.Code.Components;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace EvershockGame.Code.Entities
{
    public class TestObject : Entity
    {
        private TransformComponent transform;
        private PhysicsComponent physics;
        private RectColliderComponent collider; 

        public TestObject (string name, Guid parent) : base (name, parent)
        {
            transform = AddComponent<TransformComponent>();
            physics = AddComponent<PhysicsComponent>();
            collider = AddComponent<RectColliderComponent>();

            //TODO_lukas: Check
            //Always call Init() on Entities, but never on components, due to possibly scrambled-up dependencies (e.g. transform <-> physics)

            Init();

            /*
            TODO_lukas: Check
            does this also work, when the physics component is initiated, before the transform component?
            If not, how do we work around it?
            - 
            -
            -
            */
        }

        public void Init()
        {
            transform.Init(new Vector3(420, 400, 0));
            physics.Init(BodyType.Kinematic, 0, 0);
            collider.Init(75, 75, ECollisionCategory.All);
            //physics.Body.Position = new Vector2(transform.Location.X, transform.Location.Y);
            physics.Body.LinearVelocity = new Vector2(5, 2);
        }
    }
}
