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
        private Dictionary<Guid, IEntity> m_Entities;
        private Dictionary<Guid, Guid> m_Hierarchy;

        //---------------------------------------------------------------------------

        protected EntityManager() { GlobalManager.Get().Register(this); }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Entities = new Dictionary<Guid, IEntity>();
            m_Hierarchy = new Dictionary<Guid, Guid>();
        }

        //---------------------------------------------------------------------------

        protected override void InitAfterDeserialize()
        {
            if (m_Entities == null)
            {
                m_Entities = new Dictionary<Guid, IEntity>();
            }
        }

        //---------------------------------------------------------------------------

        public void Register(IEntity entity)
        {
            if (entity != null)
            {
                if (!m_Entities.ContainsKey(entity.GUID)) m_Entities.Add(entity.GUID, (entity));
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

        public IEntity Find(Guid guid)
        {
            if (m_Entities.ContainsKey(guid))
            {
                return m_Entities[guid];
            }
            return null;
        }

        //---------------------------------------------------------------------------

        public T Find<T>(Guid guid) where T : IEntity
        {
            if (m_Entities.ContainsKey(guid))
            {
                return (T)m_Entities[guid];
            }
            return default(T);
        }

        //---------------------------------------------------------------------------

        public void UpdateParent(Guid child, Guid parent)
        {
            m_Hierarchy[child] = parent;
        }

        //---------------------------------------------------------------------------

        public Guid GetParent(Guid child)
        {
            if (m_Hierarchy.ContainsKey(child)) return m_Hierarchy[child];
            return Guid.Empty;
        }

        //---------------------------------------------------------------------------

        public Guid GetRoot(Guid child)
        {
            while (m_Hierarchy.ContainsKey(child))
            {
                child = m_Hierarchy[child];
            }
            return child;
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
