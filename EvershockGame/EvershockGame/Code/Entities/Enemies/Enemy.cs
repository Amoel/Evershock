using EntityComponent;
using EntityComponent.Components;
using EvershockGame.Code.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class Enemy : Entity
    {
        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public PhysicsComponent Physics { get { return GetComponent<PhysicsComponent>(); } }
        public AIComponent AI { get { return GetComponent<AIComponent>(); } }

        //---------------------------------------------------------------------------

        public Enemy(string name) : base(name)
        {
            AddComponent<TransformComponent>();
            AddComponent<PhysicsComponent>();
            AddComponent<AIComponent>();
        }
    }
}
