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

    public enum ECollisionCategory
    {
        None,
        All,
        Player,
        Stage,
        Pickup
    }

    //---------------------------------------------------------------------------

    [Serializable]
    [RequireComponent(typeof(PhysicsComponent))]
    public abstract class ColliderComponent : Component, ICollider, IDrawableComponent
    {
        public static readonly float Unit = 10.0f;

        protected static readonly Dictionary<ECollisionCategory, Category> m_CategoryMapping = new Dictionary<ECollisionCategory, Category>()
        {
            { ECollisionCategory.None, Category.None },
            { ECollisionCategory.All, Category.All },
            { ECollisionCategory.Player, Category.Cat1 },
            { ECollisionCategory.Stage, Category.Cat2 },
            { ECollisionCategory.Pickup, Category.Cat3 }
        };

        public event CollisionEnterEventHandler Enter;
        public event CollisionLeaveEventHandler Leave;

        protected Body Body { get; set; }

        public Vector2 Offset { get; protected set; }
        public ECollisionCategory CollisionCategory { get; private set; }
        public ECollisionCategory CollidesWith { get; private set; }

        //---------------------------------------------------------------------------

        public ColliderComponent(Guid entity) : base(entity) { CollisionCategory = ECollisionCategory.All; }

        //---------------------------------------------------------------------------

        public Vector2 Step(Vector2 force)
        {
            if (Body == null) return force;
            if (Body.BodyType != BodyType.Static)
            {
                Body.LinearVelocity = force / Unit;
            }
            return (Body.Position - Offset) * Unit;
        }

        //---------------------------------------------------------------------------

        public void ResetLocation(Vector2 location)
        {
            if (Body != null)
            {
                Body.Position = location / Unit;
            }
        }

        //---------------------------------------------------------------------------

        public void SetSensor(bool isSensor)
        {
            Body.IsSensor = isSensor;
        }

        //---------------------------------------------------------------------------

        public void SetCollisionCategory(ECollisionCategory category)
        {
            CollisionCategory = category;
            Body.CollisionCategories = m_CategoryMapping[CollisionCategory];
            foreach (Fixture fixture in Body.FixtureList)
            {
                fixture.CollisionCategories = m_CategoryMapping[CollisionCategory];
            }
        }

        //---------------------------------------------------------------------------

        public void SetCollidesWith(ECollisionCategory category)
        {
            CollidesWith = category;
            Body.CollidesWith = m_CategoryMapping[CollidesWith];
            foreach (Fixture fixture in Body.FixtureList)
            {
                fixture.CollidesWith = m_CategoryMapping[CollidesWith];
            }
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

        public abstract void Draw(SpriteBatch batch, CameraData data, float deltaTime);

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
#if DEBUG
        //---------------------------------------------------------------------------

        public void SetCollisionState(bool isEnabled)
        {
            Body.CollisionCategories = isEnabled ? Category.All : Category.None;
        }
#endif
    }
}
