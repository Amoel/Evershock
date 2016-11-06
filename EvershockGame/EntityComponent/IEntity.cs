using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    public interface IEntity
    {
        Guid GUID { get; }
        string Name { get; }

        Guid Parent { get; }
        List<Guid> Children { get; }
        List<Guid> Components { get; }

        //---------------------------------------------------------------------------

        void SetParent(Guid guid);
        void SetParent(IEntity parent);

        //---------------------------------------------------------------------------

        void AddChild(Guid guid);
        void AddChild(IEntity child);

        void AddChildren(List<Guid> guids);
        void AddChildren(List<IEntity> entities);

        void RemoveChild(Guid guid);
        void RemoveChild(IEntity child);

        void RemoveChildren(List<Guid> guids);
        void RemoveChildren(List<IEntity> entities);

        void RemoveChildren();

        bool HasChild(Guid guid);
        bool HasChild(IEntity entity);

        List<IEntity> GetChildren();

        //---------------------------------------------------------------------------

        T AddComponent<T>() where T : IComponent;
        void RemoveComponent<T>() where T : IComponent;
        T GetComponent<T>() where T : IComponent;
        bool HasComponent<T>() where T : IComponent;

        void AddComponent(Guid guid);
        void AddComponent(IComponent component);

        void RemoveComponent(Guid guid);
        void RemoveComponent(IComponent component);

        bool HasComponent(Guid guid);
        bool HasComponent(IComponent component);

        List<IComponent> GetComponents();
        void ClearComponents();

        //---------------------------------------------------------------------------

        IEntity Duplicate();
    }
}
