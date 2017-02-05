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

        public bool IsGravityAffected { get; set; }

        public bool IsResting { get; private set; }
        public bool HasTouchedFloor { get; private set; }

        private Vector3 m_Force;
        private Vector3 m_Gravity;

        private float m_RestingTimer;

        //---------------------------------------------------------------------------

        public PhysicsComponent(Guid entity) : base(entity)
        {
            Inertia = 0.0f;
            Weight = 1.0f;
            Softness = 0.0f;

            IsGravityAffected = true;

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

        public virtual void Tick(float deltaTime)
        {
            TickForce(deltaTime);
        }

        //---------------------------------------------------------------------------

        private void TickForce(float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null)
            {
                if (IsGravityAffected)
                {
                    ApplyGravity(deltaTime);
                }
                
                m_Force *= Inertia;

                if (m_Force.Length() < 1.0f)
                {
                    m_RestingTimer += deltaTime;
                    if (m_RestingTimer >= 0.3f)
                    {
                        IsResting = true;
                    }
                }
                else
                {
                    m_RestingTimer = 0.0f;
                    IsResting = false;
                }

                Vector2 newForce = CollisionManager.Get().CheckCollision(transform, Entity, GetForce().To2D());
                transform.MoveTo(new Vector3(newForce.X, newForce.Y, transform.Location.Z + m_Force.Z));
            }
        }

        //---------------------------------------------------------------------------

        public Vector3 GetForce()
        {
            return m_Force;
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

        protected void ApplyGravity(float deltaTime)
        {
            if (Weight > 0.0f)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    Vector3 location = transform.AbsoluteLocation;

                    if (location.Z + m_Force.Z < 0.0f)
                    {
                        m_Force = new Vector3(m_Force.X, m_Force.Y, m_Force.Z * -Softness);
                        m_Gravity = Vector3.Zero;

                        HasTouchedFloor = true;
                    }
                    else
                    {
                        m_Gravity += PhysicsManager.Get().Gravity * Weight * deltaTime;
                        m_Force += m_Gravity;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void ResetLocation()
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            ICollider collider = CollisionManager.Get().FindCollider(Entity);

            if (transform != null && collider != null)
            {
                collider.ResetLocation(transform.Location.To2D());
            }
        }

        //---------------------------------------------------------------------------

        public virtual void ReceiveInput(GameActionCollection actions, float deltaTime)
        {
            float xMovement = (actions[EGameAction.MOVE_RIGHT].Value - actions[EGameAction.MOVE_LEFT].Value) * deltaTime * 500;
            float yMovement = (actions[EGameAction.MOVE_DOWN].Value - actions[EGameAction.MOVE_UP].Value) * deltaTime * 500;

            Vector3 movement = new Vector3(xMovement, yMovement, 0);
            if (movement.Length() > 0.01f) ApplyForce(new Vector3(xMovement, yMovement, 0));
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
