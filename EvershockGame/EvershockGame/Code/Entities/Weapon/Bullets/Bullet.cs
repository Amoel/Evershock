using EvershockGame.Code.Components;
using EvershockGame.Components;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class Bullet : Entity, IDrawableComponent
    {
        public BulletDesc Description { get; private set; }

        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public BulletPhysicsComponent Physics { get { return GetComponent<BulletPhysicsComponent>(); } }

        //---------------------------------------------------------------------------

        public Bullet(string name, Guid parent) : base(name, parent)
        {
            AddComponent<TransformComponent>();
            AddComponent<BulletPhysicsComponent>();
            AddComponent<SpriteComponent>().Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.RedOrb));
            AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.RedOrb), Vector2.Zero, Vector2.One * 2, Color.White, 0.5f);

            CircleColliderComponent collider = AddComponent<CircleColliderComponent>();
            collider.Init(4, BodyType.Dynamic);
            collider.SetSensor(true);
            collider.SetCollisionCategory(ECollisionCategory.Bullet);
            collider.SetCollidesWith(ECollisionCategory.Stage);
            collider.Enter += OnHit;

            AddComponent<DespawnComponent>();
        }

        //---------------------------------------------------------------------------

        public void Init(BulletDesc desc, Vector3 location, Vector2 direction)
        {
            Description = desc;

            Transform.MoveTo(location);
            Physics.Init(Description, location, direction);
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {

        }

        //---------------------------------------------------------------------------

        protected virtual void OnHit(IEntity source, IEntity target)
        {
            if (target is Enemy)
            {

            }
            DespawnComponent despawn = GetComponent<DespawnComponent>();
            if (despawn != null)
            {
                despawn.Trigger();
            }
        }
    }
}
