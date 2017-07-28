using EvershockGame.Code;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Dynamics.Joints;
using VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using System;

namespace EvershockGame.Code.Components
{
    [Serializable]
    [RequireComponent(typeof(TransformComponent))]
    public class PhysicsComponent : Component, ITickableComponent, IInputReceiver
    {
        public static readonly float Unit = 10.0f;

        public float Inertia { get; set; }
        public bool UseAbsoluteMovement { get; set; }

        public bool IsGravityAffected { get; set; }

        public bool IsResting { get; private set; }
        public bool HasTouchedFloor { get; private set; }

        public Body Body { get; private set; }

        private Vector3 m_Force;
        private Vector3 m_Gravity;

        private float m_RestingTimer;

        //---------------------------------------------------------------------------

        public PhysicsComponent(Guid entity) : base(entity)
        {
            Inertia = 0.0f;

            IsGravityAffected = true;

            m_Force = Vector3.Zero;
            m_Gravity = Vector3.Zero;

            Body = BodyFactory.CreateBody(PhysicsManager.Get().World, Vector2.Zero, 0, BodyType.Static, Entity);

            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                Body.Position = transform.AbsoluteLocation.To2D() / Unit;
            }
            Body.IgnoreGravity = true;
            Body.Mass = 0.0f;
            Body.Friction = 0.0f;
        }

        //---------------------------------------------------------------------------

        public void Init(BodyType bodyType, float inertia, float dampening, bool useAbsoluteMovement = false)
        {
            Body.BodyType = bodyType;
            Inertia = inertia;
            Body.Inertia = inertia;
            Body.LinearDamping = dampening;
            UseAbsoluteMovement = useAbsoluteMovement;
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
            TransformComponent parentTransform = GetComponentInParent<TransformComponent>();

            if (transform != null)
            {
                if (IsGravityAffected)
                {
                    ApplyGravity(deltaTime);
                }

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

                if (Body != null && Body.BodyType != BodyType.Static)
                {
                    if (Body.Enabled)
                    {
                        Vector3 pos = new Vector3(Body.Position.X * Unit, Body.Position.Y * Unit, transform.Location.Z + m_Force.Z);
                        if (parentTransform != null)
                        {
                            pos -= parentTransform.AbsoluteLocation;
                        }
                        transform.MoveTo(pos);
                    }
                    else
                    {
                        transform.MoveBy(m_Force);
                    }
                    
                }
                
                m_Force *= Inertia;
                if (UseAbsoluteMovement) Body.LinearVelocity = Vector2.Zero;
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

        public void ApplyForce(Vector3 force, bool ignoreInertia = false)
        {
            m_Force += (ignoreInertia ? force : force * (1.0f - Inertia));
            if (m_Force.Length() > 0)
            {
                float factor = 1.0f - (Body.LinearVelocity.Length() / (m_Force.To2D().Length() / Unit));
                if (factor > 0.0f) Body.ApplyLinearImpulse(m_Force.To2D() / Unit * factor);
            }
        }

        //---------------------------------------------------------------------------

        public void ApplyAbsoluteForce(Vector3 force)
        {
            m_Force = force;
            Body.LinearVelocity = force.To2D() / Unit;
        }

        //---------------------------------------------------------------------------

        protected void ApplyGravity(float deltaTime)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                Vector3 location = transform.AbsoluteLocation;

                if (location.Z + m_Force.Z < 0.0f)
                {
                    m_Force = new Vector3(m_Force.X, m_Force.Y, m_Force.Z);// * -Body.Restitution);
                    m_Gravity = Vector3.Zero;

                    HasTouchedFloor = true;
                }
                else
                {
                    m_Gravity += PhysicsManager.Get().Gravity * deltaTime;
                    m_Force += m_Gravity;
                }
            }
        }

        //---------------------------------------------------------------------------

        public void ResetLocation(Vector3? location = null)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (Body != null && transform != null)
            {
                if (location != null)
                {
                    transform.MoveTo(location.Value);
                }
                Body.Position = transform.AbsoluteLocation.To2D() / Unit;
            }
        }
        
        //---------------------------------------------------------------------------

        public void AddJoint(PhysicsComponent other)
        {
            if (other != null && other.Body != null)
            {
                WeldJoint joint = JointFactory.CreateWeldJoint(PhysicsManager.Get().World, Body, other.Body, Vector2.Zero, Vector2.Zero);
                joint.CollideConnected = false;
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

        public override void OnCleanup()
        {
            PhysicsManager.Get().World.RemoveBody(Body);
        }
    }
}
