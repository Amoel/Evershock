using System;
using System.Collections.Generic;

namespace EntityComponent
{
    public interface IComponent
    {
        Guid GUID { get; }
        string Name { get; }

        bool IsEnabled { get; }

        //---------------------------------------------------------------------------
        
        T GetComponent<T>() where T : IComponent;
        List<IComponent> GetComponents();

        void OnCleanup();

        void Enable();
        void Disable();
        void EnableEntity();
        void DisableEntity();
    }
}
