using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using EvershockGame.Code;
using EvershockGame.Code.Manager;
using EvershockGame.Code.Factory;

namespace EvershockGame.Code
{
    [Serializable]
    public class Entity : IEntity
    {
        public Guid GUID { get; private set; }
        public string Name { get; private set; }

        public Guid Parent { get { return EntityManager.Get().GetParent(GUID); } }
        public Dictionary<Guid, Type> Components { get; private set; }

        public bool IsEnabled { get; private set; }

        //---------------------------------------------------------------------------

        public Entity(string name, Guid parent)
        {
            GUID = Guid.NewGuid();
            Name = name;

            EntityManager.Get().Register(this);
            Components = new Dictionary<Guid, Type>();

            IsEnabled = true;

            if (parent != Guid.Empty)
            {
                EntityManager.Get().UpdateParent(GUID, parent);
            }
        }

        //---------------------------------------------------------------------------
        
        public void AddChild<T>(T child) where T : class, IEntity
        {
            if (child != null)
            {
                EntityManager.Get().UpdateParent(child.GUID, GUID);
            }
        }

        public void AddChild(Guid child)
        {
            IEntity entity = EntityManager.Get().Find(child);
            if (entity != null)
            {
                EntityManager.Get().UpdateParent(child, GUID);
            }
        }

        public void AddChildren(List<Guid> children)
        {
            if (children != null && children.Count > 0)
            {
                foreach (Guid child in children)
                {
                    AddChild(child);
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

        public void RemoveChild<T>(T child) where T : class, IEntity
        {
            EntityManager.Get().UpdateParent(child.GUID, Guid.Empty);
            //if (Children.ContainsKey(typeof(T)))
            //{
            //    if (Children[typeof(T)].Contains(child.GUID))
            //    {
            //        Children[typeof(T)].Remove(child.GUID);
            //    }
            //}
        }

        public void RemoveChild(Guid guid)
        {
            EntityManager.Get().UpdateParent(guid, Guid.Empty);
            //foreach (List<Guid> guids in Children.Values)
            //{
            //    if (guids.Contains(guid))
            //    {
            //        guids.Remove(guid);
            //    }
            //}
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
            //Children.Clear();
        }

        public bool HasChild(Guid guid)
        {
            //foreach (List<Guid> guids in Children.Values)
            //{
            //    if (guids.Contains(guid)) return true;
            //}
            return false;
        }
        
        public bool HasChild<T>(T child) where T : class, IEntity
        {
            if (child != null)
            {
                return HasChild(child.GUID);
            }
            return false;
        }

        public List<T> GetChildren<T>() where T : class, IEntity
        {
            return EntityManager.Get().GetChildren<T>(GUID);
        }

        public List<IEntity> GetChildren()
        {
            return EntityManager.Get().GetChildren(GUID);
        }

        public IEntity GetParent()
        {
            if (Parent != Guid.Empty)
            {
                return EntityManager.Get().Find(Parent);
            }
            return null;
        }

        //---------------------------------------------------------------------------

        public T AddComponent<T>(bool isRequiredComponent = false) where T : IComponent
        {
            AddRequirements<T>();

            if (!HasComponent<T>())
            {
                AssertManager.Get().Show(!isRequiredComponent, string.Format("{0} is missing {1}.", Name, typeof(T).Name));
                T component = ComponentFactory.Create<T>(GUID);
                if (component != null)
                {
                    Components.Add(component.GUID, typeof(T));
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
            foreach (Guid guid in Components.Keys)
            {
                if ((component = ComponentManager.Get().Find<T>(guid)) != null) return component;
            }
            return component;
        }

        public bool HasComponent<T>() where T : IComponent
        {
            foreach (Type type in Components.Values)
            {
                if (typeof(T).IsAssignableFrom(type)) return true;
            }
            //foreach (Guid guid in Components)
            //{
            //    if (ComponentManager.Get().Find<T>(guid) != null) return true;
            //}
            return false;
        }

        public List<IComponent> GetComponents()
        {
            List<IComponent> components = new List<IComponent>();
            foreach (Guid guid in Components.Keys)
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
            foreach (Guid guid in Components.Keys)
            {
                ComponentManager.Get().Unregister(guid);
            }
        }

        //---------------------------------------------------------------------------

        public virtual IEntity Duplicate() { return null; }

        public void Enable()
        {
            IsEnabled = true;
            foreach (IComponent component in GetComponents())
            {
                component.Enable();
            }
        }

        public void Disable()
        {
            IsEnabled = false;
            foreach (IComponent component in GetComponents())
            {
                component.Disable();
            }
        }

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
                        genericMethod.Invoke(this, new object[] { true });
                    }
                }
            }
        }
    }
}
