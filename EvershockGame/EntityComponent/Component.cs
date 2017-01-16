using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    [Serializable]
    public abstract class Component : IComponent
    {
        public Guid GUID { get; private set; }
        public string Name { get { return GetType().Name; } }

        public Guid Entity { get; private set; }

        public bool IsEnabled { get; private set; }

        //---------------------------------------------------------------------------

        public Component(Guid entity)
        {
            GUID = Guid.NewGuid();
            Entity = entity;

            IsEnabled = true;
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

        public T GetComponentInParent<T>() where T : IComponent
        {
            Guid parent = EntityManager.Get().GetParent(Entity);
            IEntity entity = EntityManager.Get().Find(parent);
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

        public abstract void OnCleanup();

        //---------------------------------------------------------------------------

        public void Enable()
        {
            IsEnabled = true;
        }

        //---------------------------------------------------------------------------

        public void Disable()
        {
            IsEnabled = false;
        }

        //---------------------------------------------------------------------------

        public void EnableEntity()
        {
            IEntity entity = EntityManager.Get().Find(Entity);
            if (entity != null)
            {
                entity.Enable();
            }
        }

        //---------------------------------------------------------------------------

        public void DisableEntity()
        {
            IEntity entity = EntityManager.Get().Find(Entity);
            if (entity != null)
            {
                entity.Disable();
            }
        }
    }
}
