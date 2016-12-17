using EntityComponent.Entities;
using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Factory
{
    public static class EntityFactory
    {
        public static T Create<T>(string name) where T : class, IEntity
        {
            if (AssertManager.Get().Show(!typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for UI entity. Please use CreateUI<T>."))
            {
                return CreateUI<T>(name, null);
            }
            return Create<T>(Guid.Empty, name);
        }

        //---------------------------------------------------------------------------

        public static T Create<T>(Guid parent, string name) where T : class, IEntity
        {
            if (AssertManager.Get().Show(!typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for UI entity. Please use CreateUI<T>."))
            {
                return CreateUI<T>(parent, name, null);
            }
            T entity = (T)Activator.CreateInstance(typeof(T), name);
            if (entity != null)
            {
                entity.SetParent(parent);
            }
            return entity;
        }

        //---------------------------------------------------------------------------

        public static T CreateUI<T>(string name, Frame frame = null) where T : class, IEntity
        {
            if (AssertManager.Get().Show(typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for entity. Please use Create<T>."))
            {
                return Create<T>(name);
            }
            return CreateUI<T>(Guid.Empty, name, frame);
        }

        //---------------------------------------------------------------------------

        public static T CreateUI<T>(Guid parent, string name, Frame frame = null) where T : class, IEntity
        {
            if (AssertManager.Get().Show(typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for entity. Please use Create<T>."))
            {
                return Create<T>(parent, name);
            }
            T entity = (T)Activator.CreateInstance(typeof(T), name, frame);
            if (entity != null)
            {
                entity.SetParent(parent);
            }
            return entity;
        }
    }
}
