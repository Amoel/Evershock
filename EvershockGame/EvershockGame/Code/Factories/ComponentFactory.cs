using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Factory
{
    public static class ComponentFactory
    {
        public static T Create<T>(Guid entity) where T : IComponent
        {
            T component = (T)Activator.CreateInstance(typeof(T), entity);
            if (component != null)
            {
                ComponentManager.Get().Register(component);
            }
            return component;
        }
    }
}
