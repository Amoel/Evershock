using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    [Serializable]
    public abstract class Component : IComponent
    {
        public Guid GUID { get; private set; }
        public string Name { get { return GetType().Name; } }

        public Guid Entity { get; private set; }
        public string EntityName { get; private set; }

        public bool IsEnabled { get; private set; }

        //---------------------------------------------------------------------------

        public Component(Guid entity)
        {
            GUID = Guid.NewGuid();
            Entity = entity;
            EntityName = EntityManager.Get().Find(Entity).Name;

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

        public T GetComponentInAncestor<T>() where T : IComponent
        {
            Guid guid = Entity;
            do
            {
                IEntity entity = EntityManager.Get().Find(guid);
                if (entity != null)
                {
                    if (entity.HasComponent<T>())
                    {
                        return entity.GetComponent<T>();
                    }
                    guid = entity.Parent;
                }
            }
            while (guid != Guid.Empty);
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

        public List<T> GetChildren<T>() where T : class, IEntity
        {
            IEntity entity = EntityManager.Get().Find(Entity);
            if (entity != null)
            {
                return entity.GetChildren<T>();
            }
            return new List<T>();
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

        //---------------------------------------------------------------------------

        protected void OnPropertyChanged(dynamic value, [CallerMemberName]string propertyName = "")
        {
            UIManager.Get().UpdateProperty(GUID, propertyName, value);
        }
    }
}
