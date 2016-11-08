using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components
{
    public delegate void CollisionEnterEventHandler(IEntity source, IEntity target);
    public delegate void CollisionLeaveEventHandler(IEntity source, IEntity target);

    //---------------------------------------------------------------------------

    [Serializable]
    [RequireComponent(typeof(PhysicsComponent))]
    public abstract class ColliderComponent : Component, ICollider, IDrawableComponent
    {
        public event CollisionEnterEventHandler Enter;
        public event CollisionLeaveEventHandler Leave;

        protected Body Body { get; set; }

        public Vector2 Offset { get; protected set; }

        //---------------------------------------------------------------------------

        public ColliderComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public Vector2 Step(Vector2 force)
        {
            if (Body == null) return force;
            if (Body.BodyType != BodyType.Static)
            {
                Body.ApplyLinearImpulse(force * 10.0f, Body.WorldCenter);
            }
            return Body.Position - Offset;
        }

        //---------------------------------------------------------------------------

        protected bool OnCollision(Fixture source, Fixture target, Contact contact)
        {
            IEntity sourceEntity = null;
            IEntity targetEntity = null;

            if (source.UserData != null)
            {
                sourceEntity = EntityManager.Get().Find((Guid)source.UserData);
            }
            if (target.UserData != null)
            {
                targetEntity = EntityManager.Get().Find((Guid)target.UserData);
            }
            OnEnter(sourceEntity, targetEntity);
            return true;
        }

        //---------------------------------------------------------------------------

        protected void OnSeparation(Fixture source, Fixture target)
        {
            IEntity sourceEntity = null;
            IEntity targetEntity = null;

            if (source.UserData != null)
            {
                sourceEntity = EntityManager.Get().Find((Guid)source.UserData);
            }
            if (target.UserData != null)
            {
                targetEntity = EntityManager.Get().Find((Guid)target.UserData);
            }

            OnLeave(sourceEntity, targetEntity);
        }

        //---------------------------------------------------------------------------

        private void OnEnter(IEntity source, IEntity target)
        {
            Enter?.Invoke(source, target);
        }

        //---------------------------------------------------------------------------

        private void OnLeave(IEntity source, IEntity target)
        {
            Leave?.Invoke(source, target);
        }

        //---------------------------------------------------------------------------

        public abstract void Draw(SpriteBatch batch, CameraData data);

        //---------------------------------------------------------------------------

        protected Color GetDebugColor()
        {
            if (Body == null) return Color.White;
            switch (Body.BodyType)
            {
                case BodyType.Dynamic: return new Color(0.067f, 0.812f, 0.129f);
                case BodyType.Static: return new Color(0.812f, 0.067f, 0.153f);
                default: return Color.White;
            }
        }
    }
}
