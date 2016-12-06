using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(TransformComponent))]
    public class PhysicsComponent : Component, ITickableComponent, IInputReceiver
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

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            TickForce(deltaTime);
        }

        //---------------------------------------------------------------------------

        private void TickForce(float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                ApplyGravity(deltaTime);
                
                if (m_Force.Length() < 0.01f)
                {
                    m_Force = Vector3.Zero;
                }
                else
                {
                    m_Force *= Inertia;
                }

                Vector2 newForce = CollisionManager.Get().CheckCollision(Entity, GetForce().To2D());
                transform.MoveTo(new Vector3(newForce.X, newForce.Y, GetForce().Z));
            }
        }

        //---------------------------------------------------------------------------

        public Vector3 GetForce()
        {
            return (m_Force + m_Gravity);
        }

        //---------------------------------------------------------------------------

        public Vector3 GetAbsoluteForce()
        {
            PhysicsComponent parentPhysics = GetComponentInParent<PhysicsComponent>();
            if (parentPhysics != null)
            {
                return GetForce() + parentPhysics.GetAbsoluteForce();
            }
            return GetForce();
        }

        //---------------------------------------------------------------------------

        public void ApplyForce(Vector3 force, bool ignoreInertia = false, float randomness = 0.0f)
        {
            m_Force += (ignoreInertia ? force : force * (1.0f - Inertia));
        }

        //---------------------------------------------------------------------------

        private void ApplyGravity(float deltaTime)
        {
            if (Weight > 0.0f)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    Vector3 location = transform.AbsoluteLocation;
                    if (Math.Abs(location.Z) <= 0.1f)
                    {
                        m_Gravity = Vector3.Zero;
                    }
                    else if (location.Z < 0.0f)
                    {
                        m_Gravity *= -0.8f;
                    }
                    else
                    {
                        m_Gravity += PhysicsManager.Get().Gravity * Weight * deltaTime;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public virtual void ReceiveInput(GameActionCollection actions, float deltaTime)
        {
            float xMovement = (actions[EGameAction.MOVE_RIGHT] - actions[EGameAction.MOVE_LEFT]) * deltaTime * 500 ;
            float yMovement = (actions[EGameAction.MOVE_DOWN] - actions[EGameAction.MOVE_UP]) * deltaTime * 500;

            Vector3 movement = new Vector3(xMovement, yMovement, 0);
            if (movement.Length() > 0.01f) ApplyForce(new Vector3(xMovement, yMovement, 0));
        }
    }
}
