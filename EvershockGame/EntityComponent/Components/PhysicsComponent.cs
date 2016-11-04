using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(TransformComponent))]
    public class PhysicsComponent : Component, ITickableComponent
    {
        public float Inertia { get; set; }
        public float Weight { get; set; }
        public float Softness { get; set; }

        private Vector3 m_Force;
        private Vector3 m_Gravity;

        //---------------------------------------------------------------------------

        public PhysicsComponent(Guid entity) : base(entity)
        {
            Inertia = 0.0f;
            Weight = 1.0f;
            Softness = 0.0f;

            m_Force = Vector3.Zero;
            m_Gravity = Vector3.Zero;
        }

        //---------------------------------------------------------------------------

        public void Init(float inertia = 0.0f, float weight = 1.0f, float softness = 0.0f)
        {
            Inertia = inertia;
            Weight = weight;
            Softness = softness;
        }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            CircleColliderComponent circleCollider = GetComponent<CircleColliderComponent>();

            if (circleCollider != null)
            {
                if (circleCollider.Mobility == EColliderMobility.Dynamic)
                {
                    
                }
                TickForce(deltaTime);
            }
            

            //if (force.Length() > 0.01f)
            //{
            //    if (-force.Z > transform.Location.Z)
            //    {
            //        //Vector3 forceAfterCollision = CollisionManager.Get().CheckCollision(Entity, transform.Location, new Vector3(force.X, force.Y, -transform.Location.Z), deltaTime);

            //        transform.MoveBy(new Vector3(force.X, force.Y, -transform.Location.Z));
            //        //transform.MoveBy(new Vector3(forceAfterCollision.X, forceAfterCollision.Y, -transform.Location.Z));
            //        m_Gravity = Vector3.Zero;
            //        if (force.Z < -1.0f) m_Force = force.Reflect(new Vector3(0, 0, 1)) * Softness;
            //    }
            //    else
            //    {
            //        //Vector3 forceAfterCollision = CollisionManager.Get().CheckCollision(Entity, transform.Location, force, deltaTime);

            //        transform.MoveBy(force);
            //        //transform.MoveBy(forceAfterCollision);
            //    }
            //}
        }

        //---------------------------------------------------------------------------

        private void TickForce(float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null && m_Force != null)
            {
                ApplyGravity();

                if (m_Force.Length() < 0.01f)
                {
                    m_Force = Vector3.Zero;
                }
                else
                {
                    m_Force *= Inertia;
                }

                if (GetForce().Length() > 0.01f)
                {
                    Vector3 forceAfterCollision = CollisionManager.Get().CheckCollision(Entity, transform.Location, GetForce());
                    if (forceAfterCollision.Length() > 0.01f)
                    {
                        if (-forceAfterCollision.Z > transform.Location.Z)
                        {
                            transform.MoveBy(new Vector3(forceAfterCollision.X, forceAfterCollision.Y, -transform.Location.Z));
                        }
                        else
                        {
                            transform.MoveBy(forceAfterCollision);
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public Vector3 GetForce()
        {
            return (m_Force + m_Gravity);
        }

        //---------------------------------------------------------------------------

        public void ApplyForce(Vector3 force, bool ignoreInertia = false, float randomness = 0.0f)
        {
            m_Force += (ignoreInertia ? force : force * (1.0f - Inertia));
        }

        //---------------------------------------------------------------------------

        private void ApplyGravity()
        {
            if (Weight > 0.0f)
            {
                m_Gravity += PhysicsManager.Get().Gravity * Weight;
            }
        }
    }
}
