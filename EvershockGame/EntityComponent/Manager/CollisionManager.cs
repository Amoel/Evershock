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

        public Vector3 CheckCollision(Guid guid, Vector3 location, Vector3 force)
        {
            Vector3 outForce = Vector3.Zero;
            bool isCollision = false;

            foreach (Guid collider in m_Collider)
            {
                if (guid.Equals(collider)) continue;

                IEntity other = EntityManager.Get().Find(collider);
                if (other == null) continue;

                TransformComponent otherTransform = other.GetComponent<TransformComponent>();
                PhysicsComponent otherPhysics = other.GetComponent<PhysicsComponent>();
                if (otherTransform == null || otherPhysics == null) continue;

                //if (Vector3.Distance(location, otherTransform.Location) < 26 + 26)
                //{
                //    isCollision = true;
                //    collisionOffset += Vector3.Normalize(location - otherTransform.Location) * ((26 + 26) - Vector3.Distance(location, otherTransform.Location));
                //}

                Vector3 newForce;
                if (CheckCollision(location, force, otherTransform.Location, otherPhysics.GetForce(), 26, 26, out newForce))
                {
                    outForce += newForce;
                    isCollision = true;
                }
            }
            return (isCollision ? outForce : force);
        }

        //---------------------------------------------------------------------------

        public bool CheckCollision(Vector3 aLocation, Vector3 aForce, Vector3 bLocation, Vector3 bForce, float aRadius, float bRadius, out Vector3 newForce)
        {
            Vector3 force = Vector3.Zero;

            bool isCollision;
            float time = CheckCollision(
                aLocation, 
                bLocation, 
                aForce, 
                bForce, 
                aRadius, 
                bRadius, 
                out isCollision);

            if (!float.IsNaN(time) && time > 0.0f && time <= 1.0f)
            {
                Vector3 collisionA = aLocation + aForce * time;
                Vector3 collisionB = bLocation + bForce * time;
                Vector3 point = (collisionA - collisionB) * (bRadius / (aRadius + bRadius)) + collisionB;

                force = (aForce * time).Reflect2(point - (bLocation + bForce * time));

                Vector3 test = (aLocation + aForce * time) - (bLocation + bForce * time);
                float length = Math.Max(0, (aRadius + bRadius) - test.Length());

                if (length >= 0)
                {
                    force += Vector3.Normalize(test) * length;
                }

                newForce = force;
                return isCollision;
            }
            else
            {
                newForce = aForce;
                return false;
            }
        }

        //---------------------------------------------------------------------------

        private float CheckCollision(Vector3 aPos, Vector3 bPos, Vector3 aForce, Vector3 bForce, float aRadius, float bRadius, out bool isCollision)
        {
            Vector3 deltaPos = aPos - bPos;
            Vector3 deltaForce = aForce - bForce;

            float a = Vector3.Dot(deltaForce, deltaForce);
            float b = 2 * Vector3.Dot(deltaPos, deltaForce);
            float c = Vector3.Dot(deltaPos, deltaPos) - (aRadius + bRadius) * (aRadius + bRadius);

            float discriminant = b * b - 4 * a * c;

            float t;
            if (discriminant < 0.0f)
            {
                t = -b / (2 * a);
                isCollision = false;
            }
            else
            {
                float t0 = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
                float t1 = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);
                t = Math.Min(t0, t1);

                if (t < 0.0f)
                {
                    isCollision = false;
                }
                else
                {
                    isCollision = true;
                }
            }

            if (t < 0)
            {
                t = 0;
            }

            return t;
        }
    }
}
