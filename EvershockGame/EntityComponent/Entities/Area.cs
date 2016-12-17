using EntityComponent.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    public class Area : Entity
    {
        public static List<Area> Areas = new List<Area>();

        //---------------------------------------------------------------------------

        public AreaColliderComponent Collider { get { return GetComponent<AreaColliderComponent>(); } }
        public int Within { get; private set; }

        //---------------------------------------------------------------------------

        public Area(string name) : base(name)
        {
            Areas.Add(this);
            AddComponent<TransformComponent>();
            AddComponent<PhysicsComponent>();
            AreaColliderComponent collider = AddComponent<AreaColliderComponent>();
            if (collider != null)
            {
                collider.SetCollidesWith(ECollisionCategory.All);
                collider.Enter += OnEnter;
                collider.Leave += OnLeave;
            }
        }

        //---------------------------------------------------------------------------

        private void OnEnter(IEntity source, IEntity target)
        {
            Console.WriteLine(string.Format("{0} entered {1}.", target.Name, source.Name));
            Within++;
        }

        //---------------------------------------------------------------------------

        private void OnLeave(IEntity source, IEntity target)
        {
            Console.WriteLine(string.Format("{0} left {1}.", target.Name, source.Name));
            Within--;
        }
    }
}
