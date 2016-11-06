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
        public static T Create<T>(string name) where T : IEntity
        {
            return Create<T>(Guid.Empty, name);
        }

        //---------------------------------------------------------------------------

        public static T Create<T>(Guid parent, string name) where T : IEntity
        {
            
            T entity = (T)Activator.CreateInstance(typeof(T),  name);
            if (entity != null)
            {
                entity.SetParent(parent);
                EntityManager.Get().Register(entity);
            }
            return entity;
        }
    }
}
