using EntityComponent.Components;
using EntityComponent.Factory;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EntityComponent.Manager
{
    public enum EColliderType
    {
        Solid,
        ForceField,
        Area
    }

    //---------------------------------------------------------------------------

    public enum EColliderMobility
    {
        Static,
        Dynamic
    }

    //---------------------------------------------------------------------------

    public class CollisionManager : BaseManager<CollisionManager>
    {
        private List<Guid> m_Collider;

        public Texture2D CircleTexture { get; set; }
        public Texture2D RectTexture { get; set; }
        public Texture2D PointTexture { get; set; }

        public bool IsDebugViewActive { get; set; }

        //---------------------------------------------------------------------------

        protected CollisionManager() { }

        //---------------------------------------------------------------------------

        protected override void Init()
        {
            m_Collider = new List<Guid>();
        }

        //---------------------------------------------------------------------------

        public void Register(Guid guid)
        {
            if (!m_Collider.Contains(guid))
            {
                m_Collider.Add(guid);
            }
        }

        //---------------------------------------------------------------------------

        public void Unregister(Guid guid)
        {
            if (m_Collider.Contains(guid))
            {
                m_Collider.Remove(guid);
            }
        }

        //---------------------------------------------------------------------------

        public Vector2 CheckCollision(TransformComponent transform, Guid entity, Vector2 force)
        {
            var collider = FindCollider(entity);

            if (collider == null)
            {
                return transform.Location.To2D() + force;
            }
            
            return collider.Step(force);
        }

        //---------------------------------------------------------------------------

        public ICollider FindCollider(Guid guid)
        {
            IEntity entity = EntityManager.Get().Find(guid);
            if (entity != null)
            {
                if (entity.HasComponent<RectColliderComponent>()) return entity.GetComponent<RectColliderComponent>();
                if (entity.HasComponent<CircleColliderComponent>()) return entity.GetComponent<CircleColliderComponent>();
            }
            return null;
        }
    }

    //---------------------------------------------------------------------------

    public class CollisionResult
    {
        public Vector3 ForceBeforeCollision { get; set; }
        public Vector3 ForceAfterCollision { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Point { get; set; }

        public bool IsCollision { get { return !ForceBeforeCollision.Equals(ForceAfterCollision); } }

        //---------------------------------------------------------------------------

        public CollisionResult() { }
    }
}
