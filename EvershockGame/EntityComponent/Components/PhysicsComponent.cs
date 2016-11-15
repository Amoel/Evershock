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

        public void Tick(float deltaTime)
        {
            TickForce(deltaTime);
        }

        //---------------------------------------------------------------------------

        private void TickForce(float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();

            if (transform != null && m_Force != null)
            {
                //ApplyGravity();

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

                if (GetForce().Length() > 0.01f)
                {
                    //CollisionResult result = CollisionManager.Get().CheckCollision(Entity, GetForce());// transform.Location, GetForce());
                    //if (result.ForceAfterCollision.Length() > 0.01f)
                    //{
                    //    transform.MoveBy(result.ForceAfterCollision);
                    //    //if (-result.ForceAfterCollision.Z > transform.Location.Z)
                    //    //{
                    //    //    transform.MoveBy(new Vector3(result.ForceAfterCollision.X, result.ForceAfterCollision.Y, -transform.Location.Z));
                    //    //}
                    //    //else
                    //    //{
                    //    //    transform.MoveBy(result.ForceAfterCollision);
                    //    //}
                    //}
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
