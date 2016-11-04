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
        public static T Create<T>() where T : IEntity, new()
        {
            return Create<T>(Guid.Empty);
        }

        //---------------------------------------------------------------------------

        public static T Create<T>(Guid parent) where T : IEntity, new()
        {
            T entity = Activator.CreateInstance<T>();
            if (entity != null)
            {
                entity.SetParent(parent);
                EntityManager.Get().Register(entity);
            }
            return entity;
        }
    }
}
