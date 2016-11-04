using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    [Serializable]
    public class GlobalManager : BaseManager<GlobalManager>
    {
        [JsonIgnore]
        private List<IManager> m_Managers;

        //---------------------------------------------------------------------------

        protected GlobalManager() { }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Managers = new List<IManager>() { this };
        }

        //---------------------------------------------------------------------------

        public void Register(IManager manager)
        {
            if (!m_Managers.Contains(manager))
            {
                m_Managers.Add(manager);
            }
        }

        //---------------------------------------------------------------------------

        public void SerializeManagers()
        {
            foreach (IManager manager in m_Managers)
            {
                manager.Serialize();
            }
        }
    }
}
