using EntityComponent.Factory;
using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    [Serializable]
    public class Entity : IEntity
    {
        public Guid GUID { get; private set; }

        public Guid Parent { get; private set; }
        public List<Guid> Children { get; private set; }
        public List<Guid> Components { get; private set; }

        //---------------------------------------------------------------------------

        public Entity()
        {
            GUID = Guid.NewGuid();

            Children = new List<Guid>();
            Components = new List<Guid>();

            SetParent(null);
        }

        //---------------------------------------------------------------------------

        public void SetParent(Guid guid)
        {
            Parent = guid;
        }

        public void SetParent(IEntity parent)
        {
            if (parent != null)
            {
                Parent = parent.GUID;
            }
            else
            {
                Parent = Guid.Empty;
            }
        }

        //---------------------------------------------------------------------------

        public void AddChild(Guid guid)
        {
            if (!HasChild(guid))
            {
                Children.Add(guid);
            }
        }

        public void AddChild(IEntity child)
        {
            if (child != null && !HasChild(child))
            {
                Children.Add(child.GUID);
            }
        }

        public void AddChildren(List<Guid> guids)
        {
            if (guids != null && guids.Count > 0)
            {
                foreach (Guid guid in guids)
                {
                    AddChild(guid);
                }
            }
        }

        public void AddChildren(List<IEntity> children)
        {
            if (children != null && children.Count > 0)
            {
                foreach (IEntity child in children)
                {
                    AddChild(child);
                }
            }
        }

        public void RemoveChild(Guid guid)
        {
            if (HasChild(guid))
            {
                Children.Remove(guid);
            }
        }

        public void RemoveChild(IEntity child)
        {
            if (child != null && HasChild(child))
            {
                Children.Remove(child.GUID);
            }
        }

        public void RemoveChildren(List<Guid> guids)
        {
            if (guids != null && guids.Count > 0)
            {
                foreach (Guid guid in guids)
                {
                    RemoveChild(guid);
                }
            }
        }

        public void RemoveChildren(List<IEntity> children)
        {
            if (children != null && children.Count > 0)
            {
                foreach (IEntity child in children)
                {
                    RemoveChild(child);
                }
            }
        }

        public void RemoveChildren()
        {
            Children.Clear();
        }

        public bool HasChild(Guid guid)
        {
            return Children.Contains(guid);
        }
        
        public bool HasChild(IEntity child)
        {
            if (child != null)
            {
                return Children.Contains(child.GUID);
            }
            return false;
        }

        public List<IEntity> GetChildren()
        {
            List<IEntity> children = new List<IEntity>();
            foreach (Guid guid in Components)
            {
                IEntity child = EntityManager.Get().Find(guid);
                if (child != null)
                {
                    children.Add(child);
                }
            }
            return children;
        }

        //---------------------------------------------------------------------------

        public T AddComponent<T>() where T : IComponent
        {
            AddRequirements<T>();

            if (!HasComponent<T>())
            {
                T component = ComponentFactory.Create<T>(GUID);
                if (component != null)
                {
                    Components.Add(component.GUID);
                }
                return component;
            }
            else
            {
                return GetComponent<T>();
            }
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            T component = GetComponent<T>();
            if (component != null)
            {
                Components.Remove(component.GUID);
            }
        }

        public T GetComponent<T>() where T : IComponent
        {
            T component = default(T);
            foreach (Guid guid in Components)
            {
                if ((component = ComponentManager.Get().Find<T>(guid)) != null) return component;
            }
            return component;
        }

        public bool HasComponent<T>() where T : IComponent
        {
            foreach (Guid guid in Components)
            {
                if (ComponentManager.Get().Find<T>(guid) != null) return true;
            }
            return false;
        }

        public void AddComponent(Guid guid)
        {
            if (!HasComponent(guid))
            {
                Components.Add(guid);
            }
        }

        public void AddComponent(IComponent component)
        {
            if (component != null && !HasComponent(component))
            {
                Components.Add(component.GUID);
            }
        }

        public void RemoveComponent(Guid guid)
        {
            if (HasComponent(guid))
            {
                Components.Remove(guid);
            }
        }

        public void RemoveComponent(IComponent component)
        {
            if (component != null && HasComponent(component))
            {
                Components.Remove(component.GUID);
            }
        }

        public bool HasComponent(Guid guid)
        {
            return Components.Contains(guid);
        }

        public bool HasComponent(IComponent component)
        {
            if (component != null)
            {
                return Components.Contains(component.GUID);
            }
            return false;
        }

        public List<IComponent> GetComponents()
        {
            List<IComponent> components = new List<IComponent>();
            foreach (Guid guid in Components)
            {
                IComponent component = ComponentManager.Get().Find(guid);
                if (component != null)
                {
                    components.Add(component);
                }
            }
            return components;
        }

        public void ClearComponents()
        {
            foreach (Guid guid in Components)
            {
                ComponentManager.Get().Unregister(guid);
            }
        }

        //---------------------------------------------------------------------------

        public virtual IEntity Duplicate() { return null; }

        //---------------------------------------------------------------------------

        private void AddRequirements<T>() where T : IComponent
        {
            RequireComponentAttribute attribute = Attribute.GetCustomAttribute(typeof(T), typeof(RequireComponentAttribute)) as RequireComponentAttribute;
            if (attribute != null)
            {
                var method = GetType().GetMethods().Single(m => m.Name == "AddComponent" && m.IsGenericMethodDefinition);
                if (method != null)
                {
                    foreach (Type type in attribute.Types)
                    {
                        var genericMethod = method.MakeGenericMethod(new[] { type });
                        genericMethod.Invoke(this, null);
                    }
                }
            }
        }
    }
}
