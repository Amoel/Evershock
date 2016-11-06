using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using Newtonsoft.Json;
using System.IO;

namespace EntityComponent.Manager
{
    [Serializable]
    [SerializeManager(true, "data", "Managers")]
    public class EntityManager : BaseManager<EntityManager>
    {
        [JsonIgnore]
        private Dictionary<Guid, SmartContainer<IEntity>> m_Entities;

        //---------------------------------------------------------------------------

        protected EntityManager() { GlobalManager.Get().Register(this); }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Entities = new Dictionary<Guid, SmartContainer<IEntity>>();
        }

        //---------------------------------------------------------------------------

        protected override void InitAfterDeserialize()
        {
            if (m_Entities == null)
            {
                m_Entities = new Dictionary<Guid, SmartContainer<IEntity>>();
            }
        }

        //---------------------------------------------------------------------------

        public void Register(IEntity entity)
        {
            if (entity != null)
            {
                if (!m_Entities.ContainsKey(entity.GUID)) m_Entities.Add(entity.GUID, new SmartContainer<IEntity>(entity));
            }
        }

        //---------------------------------------------------------------------------

        public void Unregister(IEntity entity)
        {
            if (entity != null && m_Entities.ContainsKey(entity.GUID))
            {
                entity.ClearComponents();
                m_Entities.Remove(entity.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public void Unregister(Guid guid)
        {
            Unregister(Find(guid));
        }

        //---------------------------------------------------------------------------

        public void AddReference(Guid source, Guid target)
        {
            if (m_Entities.ContainsKey(target))
            {
                m_Entities[target].AddReference(source);
            }
        }

        //---------------------------------------------------------------------------

        public void RemoveReference(Guid source, Guid target)
        {
            if (m_Entities.ContainsKey(target))
            {
                bool hasReferences = m_Entities[target].RemoveReference(source);
                if (!hasReferences)
                {
                    m_Entities.Remove(target);
                }
            }
        }

        //---------------------------------------------------------------------------

        public IEntity Find(Guid guid)
        {
            if (m_Entities.ContainsKey(guid))
            {
                return m_Entities[guid].Data;
            }
            return null;
        }

        //---------------------------------------------------------------------------

        public void Load()
        {
        }

        //---------------------------------------------------------------------------

        public void Save()
        {

        }
    }
}
