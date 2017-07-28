using EvershockGame.Code;
using EvershockGame.Code.Components;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Collision.Filtering;
using VelcroPhysics.Collision.ContactSystem;

namespace EvershockGame.Code.Components
{
    public delegate void CollisionEnterEventHandler(IEntity source, IEntity target);
    public delegate void CollisionLeaveEventHandler(IEntity source, IEntity target);

    //---------------------------------------------------------------------------

    [Flags]
    public enum ECollisionCategory
    {
        None = 0,
        All = int.MaxValue,
        Player = 1,
        Stage = 2,
        Pickup = 4,
        Bullet = 8
    }

    //---------------------------------------------------------------------------

    [Serializable]
    public abstract class ColliderComponent : Component, ICollider, IDrawableComponent
    {
        public static readonly float Unit = 10.0f;

        protected static readonly Dictionary<ECollisionCategory, Category> m_CategoryMapping = new Dictionary<ECollisionCategory, Category>()
        {
            { ECollisionCategory.None, Category.None },
            { ECollisionCategory.All, Category.All },
            { ECollisionCategory.Player, Category.Cat1 },
            { ECollisionCategory.Stage, Category.Cat2 },
            { ECollisionCategory.Pickup, Category.Cat3 },
            { ECollisionCategory.Bullet, Category.Cat4 }
        };

        public event CollisionEnterEventHandler Enter;
        public event CollisionLeaveEventHandler Leave;

        //protected Body Body { get; set; }
        protected List<Fixture> Fixtures { get; set; }

        public Vector2 Offset { get; protected set; }
        public ECollisionCategory CollisionCategory { get; private set; }
        public ECollisionCategory CollidesWith { get; private set; }

        //---------------------------------------------------------------------------

        public ColliderComponent(Guid entity) : base(entity)
        {
            Fixtures = new List<Fixture>();
            CollisionCategory = ECollisionCategory.All;
        }

        //---------------------------------------------------------------------------

        public void SetSensor(bool isSensor)
        {
            foreach (Fixture fixture in Fixtures)
            {
                fixture.IsSensor = isSensor;
            }
        }

        //---------------------------------------------------------------------------

        public void SetRestitution(float restitution)
        {
            foreach (Fixture fixture in Fixtures)
            {
                fixture.Restitution = restitution;
            }
        }

        //---------------------------------------------------------------------------

        public void SetCollisionCategory(ECollisionCategory category)
        {
            CollisionCategory = category;
            foreach (Fixture fixture in Fixtures)
            {
                fixture.CollisionCategories = (Category)category; //m_CategoryMapping[CollisionCategory];
            }
        }

        //---------------------------------------------------------------------------

        public void SetCollidesWith(ECollisionCategory category)
        {
            CollidesWith = category;
            foreach (Fixture fixture in Fixtures)
            {
                fixture.CollidesWith = (Category)category; //m_CategoryMapping[CollidesWith];
            }
        }

        //---------------------------------------------------------------------------

        protected void OnCollision(Fixture source, Fixture target, Contact contact)
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
        }

        //---------------------------------------------------------------------------

        protected void OnSeparation(Fixture source, Fixture target, Contact contact)
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
            if (Fixtures.Count == 0) return Color.White;
            switch (Fixtures.First().Body.BodyType)
            {
                case BodyType.Dynamic: return new Color(0.067f, 0.812f, 0.129f);
                case BodyType.Static: return new Color(0.812f, 0.067f, 0.153f);
                default: return Color.White;
            }
        }

        //---------------------------------------------------------------------------

        public void ClearFixtures()
        {
            if (Fixtures.Count > 0)
            {
                Body body = Fixtures.First().Body;
                if (body != null)
                {
                    foreach (Fixture fixture in Fixtures)
                    {
                        body.DestroyFixture(fixture);
                    }
                    Fixtures.Clear();
                }
            }
        }

        //---------------------------------------------------------------------------

        protected void DrawLine(SpriteBatch batch, Texture2D tex, Vector2 start, Vector2 end, CameraData data)
        {
            Vector2 location = start.ToLocal(data) + Vector2.One;
            float length = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            batch.Draw(tex, new Rectangle((int)location.X, (int)location.Y, (int)length, 2), tex.Bounds, GetDebugColor(), angle, Vector2.Zero, SpriteEffects.None, 1.0f);
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup()
        {
            ClearFixtures();
        }
    }
}
