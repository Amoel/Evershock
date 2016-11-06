using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    public interface IComponent
    {
        Guid GUID { get; }
        string Name { get; }

        //---------------------------------------------------------------------------
        
        T GetComponent<T>() where T : IComponent;
        List<IComponent> GetComponents();

        void OnCleanup();
    }
}
