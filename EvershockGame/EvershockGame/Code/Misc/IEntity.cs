using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame
{
    public interface IEntity
    {
        Guid GUID { get; }
        string Name { get; }

        Guid Parent { get; }
        //Dictionary<Type, List<Guid>> Children { get; }
        Dictionary<Guid, Type> Components { get; }

        bool IsEnabled { get; }

        //---------------------------------------------------------------------------

        //void SetParent(Guid guid);
        //void SetParent(IEntity parent);

        //---------------------------------------------------------------------------

        void AddChild(Guid guid);
        void AddChild<T>(T child) where T : class, IEntity;

        void AddChildren(List<Guid> guids);
        void AddChildren(List<IEntity> entities);

        void RemoveChild(Guid guid);
        void RemoveChild<T>(T child) where T : class, IEntity;

        void RemoveChildren(List<Guid> guids);
        void RemoveChildren(List<IEntity> entities);

        void RemoveChildren();

        bool HasChild(Guid guid);
        bool HasChild<T>(T entity) where T : class, IEntity;

        List<IEntity> GetChildren();
        List<T> GetChildren<T>() where T : class, IEntity;

        //---------------------------------------------------------------------------

        T AddComponent<T>(bool isRequiredComponent = false) where T : IComponent;
        void RemoveComponent<T>() where T : IComponent;
        T GetComponent<T>() where T : IComponent;
        bool HasComponent<T>() where T : IComponent;
        
        List<IComponent> GetComponents();
        void ClearComponents();

        //---------------------------------------------------------------------------

        IEntity Duplicate();

        //---------------------------------------------------------------------------

        void Enable();
        void Disable();
    }
}
