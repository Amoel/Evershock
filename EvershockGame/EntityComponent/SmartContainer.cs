using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    internal class SmartContainer<T>
    {
        private List<Guid> m_References;

        public T Data { get; set; }

        //---------------------------------------------------------------------------

        public SmartContainer(T data)
        {
            m_References = new List<Guid>();
            Data = data;
        }

        //---------------------------------------------------------------------------

        public void AddReference(Guid guid)
        {
            if (!m_References.Contains(guid))
            {
                m_References.Add(guid);
            }
        }

        //---------------------------------------------------------------------------

        public bool RemoveReference(Guid guid)
        {
            if (m_References.Contains(guid))
            {
                m_References.Remove(guid);
            }
            return m_References.Count > 0;
        }
    }
}
