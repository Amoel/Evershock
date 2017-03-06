using System;
using System.Collections.Generic;

namespace EvershockGame
{
    public interface IComponent
    {
        Guid GUID { get; }
        string Name { get; }

        Guid Entity { get; }
        string EntityName { get; }

        bool IsEnabled { get; }

        //---------------------------------------------------------------------------
        
        T GetComponent<T>() where T : IComponent;
        T GetComponentInParent<T>() where T : IComponent;
        T GetComponentInAncestor<T>() where T : IComponent;
        List<IComponent> GetComponents();

        List<T> GetChildren<T>() where T : class, IEntity;

        void OnCleanup();

        void Enable();
        void Disable();
        void EnableEntity();
        void DisableEntity();
    }
}
