using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using Newtonsoft.Json;
using System.IO;

namespace EvershockGame.Code.Manager
{
    [Serializable]
    [SerializeManager(true, "data", "Managers")]
    public class EntityManager : BaseManager<EntityManager>
    {
        [JsonIgnore]
        private Dictionary<Guid, IEntity> m_Entities;

        private Dictionary<Guid, List<Guid>> m_HierarchyTopDown;
        private Dictionary<Guid, Guid> m_HierarchyBottomUp;

        private Dictionary<Type, List<Guid>> m_Types;

        //---------------------------------------------------------------------------

        protected EntityManager() { GlobalManager.Get().Register(this); }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Entities = new Dictionary<Guid, IEntity>();
            m_HierarchyTopDown = new Dictionary<Guid, List<Guid>>();
            m_HierarchyBottomUp = new Dictionary<Guid, Guid>();
            m_Types = new Dictionary<Type, List<Guid>>();
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
                if (!m_Entities.ContainsKey(entity.GUID))
                {
                    m_Entities.Add(entity.GUID, (entity));
                }
                if (!m_Types.ContainsKey(entity.GetType()))
                {
                    m_Types.Add(entity.GetType(), new List<Guid>() { entity.GUID });
                }
                else
                {
                    m_Types[entity.GetType()].Add(entity.GUID);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Unregister(IEntity entity)
        {
            if (entity != null)
            {
                if (m_Entities.ContainsKey(entity.GUID))
                {
                    entity.ClearComponents();
                    m_Entities.Remove(entity.GUID);
                }
                if (m_Types.ContainsKey(entity.GetType()) && m_Types[entity.GetType()].Contains(entity.GUID))
                {
                    m_Types[entity.GetType()].Remove(entity.GUID);
                }
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

        public T Find<T>(Guid guid) where T : class, IEntity
        {
            if (m_Entities.ContainsKey(guid))
            {
                return m_Entities[guid] as T;
            }
            return default(T);
        }

        //---------------------------------------------------------------------------

        public List<T> Find<T>() where T : class, IEntity
        {
            List<T> entities = new List<T>();
            if (m_Types.ContainsKey(typeof(T)))
            {
                foreach (Guid guid in m_Types[typeof(T)])
                {
                    T entity = Find<T>(guid);
                    if (entity != null) entities.Add(entity);
                }
            }
            return entities;
        }

        //---------------------------------------------------------------------------

        public void UpdateParent(Guid child, Guid parent)
        {
            if (m_HierarchyBottomUp.ContainsKey(child))
            {
                Guid oldParent = m_HierarchyBottomUp[child];
                if (m_HierarchyTopDown[oldParent].Contains(child))
                {
                    m_HierarchyTopDown[oldParent].Remove(child);
                }
                m_HierarchyBottomUp[child] = parent;
            }
            else
            {
                m_HierarchyBottomUp.Add(child, parent);
            }

            if (m_HierarchyTopDown.ContainsKey(parent))
            {
                if (m_HierarchyTopDown[parent] == null)
                {
                    m_HierarchyTopDown[parent] = new List<Guid> { child };
                }
                else
                {
                    m_HierarchyTopDown[parent].Add(child);
                }
            }
            else
            {
                m_HierarchyTopDown.Add(parent, new List<Guid> { child });
            }
        }

        //---------------------------------------------------------------------------

        public Guid GetParent(Guid child)
        {
            if (m_HierarchyBottomUp.ContainsKey(child)) return m_HierarchyBottomUp[child];
            return Guid.Empty;
        }

        //---------------------------------------------------------------------------

        public Guid GetRoot(Guid child)
        {
            while (m_HierarchyBottomUp.ContainsKey(child))
            {
                child = m_HierarchyBottomUp[child];
            }
            return child;
        }

        //---------------------------------------------------------------------------

        public List<IEntity> GetChildren(Guid parent)
        {
            List<IEntity> children = new List<IEntity>();
            if (m_HierarchyTopDown.ContainsKey(parent))
            {
                foreach (Guid child in m_HierarchyTopDown[parent])
                {
                    IEntity entity = Find(child);
                    if (entity != null) children.Add(entity);
                }
            }
            return children;
        }

        public List<T> GetChildren<T>(Guid parent) where T : class, IEntity
        {
            List<T> children = new List<T>();
            if (m_HierarchyTopDown.ContainsKey(parent))
            {
                foreach (Guid child in m_HierarchyTopDown[parent])
                {
                    T entity = Find<T>(child);
                    if (entity != null) children.Add(entity);
                }
            }
            return children;
        }

        //---------------------------------------------------------------------------

        public void Load()
        {
        }

        //---------------------------------------------------------------------------

        public void Save()
        {

        }

        //---------------------------------------------------------------------------

        struct EntityWrapper
        {
            public Type Type { get; private set; }
            public Guid GUID { get; private set; }

            //---------------------------------------------------------------------------

            public EntityWrapper(Type type, Guid guid)
            {
                Type = type;
                GUID = guid;
            }
        }
    }
}
