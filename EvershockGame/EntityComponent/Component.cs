using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    [Serializable]
    public class Component : IComponent
    {
        public Guid GUID { get; private set; }
        public string Name { get { return GetType().Name; } }

        public Guid Entity { get; private set; }

        //---------------------------------------------------------------------------

        public Component(Guid entity)
        {
            GUID = Guid.NewGuid();
            Entity = entity;
        }

        //---------------------------------------------------------------------------

        public T GetComponent<T>() where T : IComponent
        {
            IEntity entity = EntityManager.Get().Find(Entity);
            if (entity != null)
            {
                return entity.GetComponent<T>();
            }
            return default(T);
        }

        //---------------------------------------------------------------------------

        public List<IComponent> GetComponents()
        {
            IEntity entity = EntityManager.Get().Find(Entity);
            if (entity != null)
            {
                return entity.GetComponents();
            }
            return null;
        }

        //---------------------------------------------------------------------------

        public virtual void OnCleanup() { }
    }
}
