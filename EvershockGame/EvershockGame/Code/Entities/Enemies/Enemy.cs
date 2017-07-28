using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Code.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Dynamics;

namespace EvershockGame.Code
{
    public class Enemy : Entity
    {
        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public MovementAnimationComponent Animation { get { return GetComponent<MovementAnimationComponent>(); } }
        public ActorPhysicsComponent Physics { get { return GetComponent<ActorPhysicsComponent>(); } }
        public AIComponent AI { get { return GetComponent<AIComponent>(); } }

        //---------------------------------------------------------------------------

        public Enemy(string name, Guid parent) : base(name, parent)
        {
            AddComponent<TransformComponent>();
            AddComponent<DespawnComponent>();
            AddComponent<AttributesComponent>();
            AddComponent<MovementAnimationComponent>();
            AddComponent<ActorPhysicsComponent>().Init(BodyType.Dynamic, 0.9f, 1.0f, true);
            AddComponent<AIComponent>();
        }
    }
}
